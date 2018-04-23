#include "DetourHelper.h"
#include <iostream>
#include <fstream>
#include <stdio.h>
#include <stdio.h>
#include <cstring>

// Returns a random number [0..1)
static float frand()
{
	return (float)rand() / (float)RAND_MAX;
}

inline bool inRange(const float* v1, const float* v2, const float r, const float h)
{
	const float dx = v2[0] - v1[0];
	const float dy = v2[1] - v1[1];
	const float dz = v2[2] - v1[2];
	return (dx*dx + dz * dz) < r*r && fabsf(dy) < h;
}

static int fixupCorridor(dtPolyRef* path, const int npath, const int maxPath,
	const dtPolyRef* visited, const int nvisited)
{
	int furthestPath = -1;
	int furthestVisited = -1;

	// Find furthest common polygon.
	for (int i = npath - 1; i >= 0; --i)
	{
		bool found = false;
		for (int j = nvisited - 1; j >= 0; --j)
		{
			if (path[i] == visited[j])
			{
				furthestPath = i;
				furthestVisited = j;
				found = true;
			}
		}
		if (found)
			break;
	}

	// If no intersection found just return current path. 
	if (furthestPath == -1 || furthestVisited == -1)
		return npath;

	// Concatenate paths.	

	// Adjust beginning of the buffer to include the visited.
	const int req = nvisited - furthestVisited;
	const int orig = rcMin(furthestPath + 1, npath);
	int size = rcMax(0, npath - orig);
	if (req + size > maxPath)
		size = maxPath - req;
	if (size)
		memmove(path + req, path + orig, size * sizeof(dtPolyRef));

	// Store visited
	for (int i = 0; i < req; ++i)
		path[i] = visited[(nvisited - 1) - i];

	return req + size;
}

// This function checks if the path has a small U-turn, that is,
// a polygon further in the path is adjacent to the first polygon
// in the path. If that happens, a shortcut is taken.
// This can happen if the target (T) location is at tile boundary,
// and we're (S) approaching it parallel to the tile edge.
// The choice at the vertex can be arbitrary, 
//  +---+---+
//  |:::|:::|
//  +-S-+-T-+
//  |:::|   | <-- the step can end up in here, resulting U-turn path.
//  +---+---+
static int fixupShortcuts(dtPolyRef* path, int npath, dtNavMeshQuery* navQuery)
{
	if (npath < 3)
		return npath;

	// Get connected polygons
	static const int maxNeis = 16;
	dtPolyRef neis[maxNeis];
	int nneis = 0;

	const dtMeshTile* tile = 0;
	const dtPoly* poly = 0;
	if (dtStatusFailed(navQuery->getAttachedNavMesh()->getTileAndPolyByRef(path[0], &tile, &poly)))
		return npath;

	for (unsigned int k = poly->firstLink; k != DT_NULL_LINK; k = tile->links[k].next)
	{
		const dtLink* link = &tile->links[k];
		if (link->ref != 0)
		{
			if (nneis < maxNeis)
				neis[nneis++] = link->ref;
		}
	}

	// If any of the neighbour polygons is within the next few polygons
	// in the path, short cut to that polygon directly.
	static const int maxLookAhead = 6;
	int cut = 0;
	for (int i = dtMin(maxLookAhead, npath) - 1; i > 1 && cut == 0; i--) {
		for (int j = 0; j < nneis; j++)
		{
			if (path[i] == neis[j]) {
				cut = i;
				break;
			}
		}
	}
	if (cut > 1)
	{
		int offset = cut - 1;
		npath -= offset;
		for (int i = 1; i < npath; i++)
			path[i] = path[i + offset];
	}

	return npath;
}

static bool getSteerTarget(dtNavMeshQuery* navQuery, const float* startPos, const float* endPos,
	const float minTargetDist,
	const dtPolyRef* path, const int pathSize,
	float* steerPos, unsigned char& steerPosFlag, dtPolyRef& steerPosRef,
	float* outPoints = 0, int* outPointCount = 0)
{
	// Find steer target.
	static const int MAX_STEER_POINTS = 3;
	float steerPath[MAX_STEER_POINTS * 3];
	unsigned char steerPathFlags[MAX_STEER_POINTS];
	dtPolyRef steerPathPolys[MAX_STEER_POINTS];
	int nsteerPath = 0;
	navQuery->findStraightPath(startPos, endPos, path, pathSize,
		steerPath, steerPathFlags, steerPathPolys, &nsteerPath, MAX_STEER_POINTS);
	if (!nsteerPath)
		return false;

	if (outPoints && outPointCount)
	{
		*outPointCount = nsteerPath;
		for (int i = 0; i < nsteerPath; ++i)
			dtVcopy(&outPoints[i * 3], &steerPath[i * 3]);
	}


	// Find vertex far enough to steer to.
	int ns = 0;
	while (ns < nsteerPath)
	{
		// Stop at Off-Mesh link or when point is further than slop away.
		if ((steerPathFlags[ns] & DT_STRAIGHTPATH_OFFMESH_CONNECTION) ||
			!inRange(&steerPath[ns * 3], startPos, minTargetDist, 1000.0f))
			break;
		ns++;
	}
	// Failed to find good point to steer to.
	if (ns >= nsteerPath)
		return false;

	dtVcopy(steerPos, &steerPath[ns * 3]);
	steerPos[1] = startPos[1];
	steerPosFlag = steerPathFlags[ns];
	steerPosRef = steerPathPolys[ns];

	return true;
}


DetourHelper::DetourHelper()
{
	m_pNavMesh = NULL;
	m_pNavQuery = NULL;
	m_filter.setIncludeFlags(0x01); // SAMPLE_POLYFLAGS_WALK
	m_filter.setExcludeFlags(0);
	m_vertCount = 0;
	memset(m_bmin, 0, sizeof(m_bmin));
	memset(m_bmax, 0, sizeof(m_bmax));
}

DetourHelper::~DetourHelper()
{
	UnLoad();
}

static char* parseRow(char* buf, char* bufEnd, char* row, int len)
{
	bool start = true;
	bool done = false;
	int n = 0;
	while (!done && buf < bufEnd)
	{
		char c = *buf;
		buf++;
		// multirow
		switch (c)
		{
		case '\\':
			break;
		case '\n':
			if (start) break;
			done = true;
			break;
		case '\r':
			break;
		case '\t':
		case ' ':
			if (start) break;
			// else falls through
		default:
			start = false;
			row[n++] = c;
			if (n >= len - 1)
				done = true;
			break;
		}
	}
	row[n] = '\0';
	return buf;
}

static int parseFace(char* row, int* data, int n, int vcnt)
{
	int j = 0;
	while (*row != '\0')
	{
		// Skip initial white space
		while (*row != '\0' && (*row == ' ' || *row == '\t'))
			row++;
		char* s = row;
		// Find vertex delimiter and terminated the string there for conversion.
		while (*row != '\0' && *row != ' ' && *row != '\t')
		{
			if (*row == '/') *row = '\0';
			row++;
		}
		if (*s == '\0')
			continue;
		int vi = atoi(s);
		data[j++] = vi < 0 ? vi + vcnt : vi - 1;
		if (j >= n) return j;
	}
	return j;
}

bool DetourHelper::Load(const std::string& filepath)
{
	m_vertices.clear();
	m_indices.clear();
	m_triangles.clear();

	char* buf = 0;
	FILE* fp = fopen(filepath.c_str(), "rb");
	if (!fp)
		return false;
	if (fseek(fp, 0, SEEK_END) != 0)
	{
		fclose(fp);
		return false;
	}
	long bufSize = ftell(fp);
	if (bufSize < 0)
	{
		fclose(fp);
		return false;
	}
	if (fseek(fp, 0, SEEK_SET) != 0)
	{
		fclose(fp);
		return false;
	}
	buf = new char[bufSize];
	if (!buf)
	{
		fclose(fp);
		return false;
	}
	size_t readLen = fread(buf, bufSize, 1, fp);
	fclose(fp);

	if (readLen != 1)
	{
		delete[] buf;
		return false;
	}

	char* src = buf;
	char* srcEnd = buf + bufSize;
	char row[512];
	int face[32];
	float x, y, z;
	int nv;
	int vcap = 0;
	int tcap = 0;

	while (src < srcEnd)
	{
		// Parse one row
		row[0] = '\0';
		src = parseRow(src, srcEnd, row, sizeof(row) / sizeof(char));
		// Skip comments
		if (row[0] == '#') continue;
		if (row[0] == 'v' && row[1] != 'n' && row[1] != 't')
		{
			// Vertex pos
			sscanf(row + 1, "%f %f %f", &x, &y, &z);
			DHVertex vertex{ x,y,z };
			m_vertices.push_back(vertex);
		}

		if (row[0] == 'f')
		{
			// Faces
			nv = parseFace(row + 1, face, 32, m_vertCount);
			for (int i = 2; i < nv; ++i)
			{
				const int a = face[0];
				const int b = face[i - 1];
				const int c = face[i];

				m_indices.push_back(a);
				m_indices.push_back(b);
				m_indices.push_back(c);
			}
		}
	}
	std::cout << "Vertices, Indices push_back Complete!" << std::endl;

	for (int i = 0; i < m_indices.size(); )
	{
		DHTriangle tri;
		for (int j = 0; j < 3; ++j)
		{
			if (m_indices[i] < 0 || m_indices[i] >= m_vertices.size())
			{
				std::cout << "indices fail" << std::endl;
				return false;
			}
			tri.vertices[j] = m_vertices[m_indices[i]];
			tri.indices[j] = m_indices[i];
			++i;
		}
		m_triangles.push_back(tri);
	}
	std::cout << "Triangles push_back Complete!" << std::endl;

	for (int i = 0; i < m_vertices.size(); ++i)
	{
		if (m_vertices[i].x < m_bmin[0]) m_bmin[0] = m_vertices[i].x;
		if (m_vertices[i].y < m_bmin[1]) m_bmin[1] = m_vertices[i].y;
		if (m_vertices[i].z < m_bmin[2]) m_bmin[2] = m_vertices[i].z;
		if (m_vertices[i].x > m_bmax[0]) m_bmax[0] = m_vertices[i].x;
		if (m_vertices[i].y > m_bmax[1]) m_bmax[1] = m_vertices[i].y;
		if (m_vertices[i].z > m_bmax[2]) m_bmax[2] = m_vertices[i].z;
	}

	dtNavMeshCreateParams params;
	memset(&params, 0, sizeof(params));
	params.nvp = 3;
	params.vertCount = (int)m_vertices.size();
	params.polyCount = (int)m_triangles.size();
	params.walkableHeight = 5.0f;
	params.walkableRadius = 0.8f;
	params.walkableClimb = 2.5f;
	params.cs = 0.8f;
	params.ch = 0.10f;
	memcpy(params.bmin, m_bmin, sizeof(float) * 3);
	memcpy(params.bmax, m_bmax, sizeof(float) * 3);

	// 填充顶点
	unsigned short* pVerts = new unsigned short[3 * params.vertCount];
	for (int i = 0; i < params.vertCount; ++i)
	{
		unsigned short x = (unsigned short)round((m_vertices[i].x - m_bmin[0]) / params.cs);
		pVerts[i * 3] = x;
		unsigned short y = (unsigned short)round((m_vertices[i].y - m_bmin[1]) / params.ch);
		pVerts[i * 3 + 1] = y;
		unsigned short z = (unsigned short)round((m_vertices[i].z - m_bmin[2]) / params.cs);
		pVerts[i * 3 + 2] = z;
	}
	params.verts = pVerts;

	// 填充三角型
	unsigned short* pPolys = new unsigned short[params.polyCount * 6];
	for (int i = 0; i < params.polyCount; ++i)
	{
		int* a = m_triangles[i].indices;

		int v = i * 6;
		pPolys[v + 0] = (unsigned short)a[0];
		pPolys[v + 1] = (unsigned short)a[1];
		pPolys[v + 2] = (unsigned short)a[2];
		pPolys[v + 3] = 0xffff; // RC_MESH_NULL_IDX
		pPolys[v + 4] = 0xffff; // RC_MESH_NULL_IDX
		pPolys[v + 5] = 0xffff; // RC_MESH_NULL_IDX

		for (int k = 0; k < params.polyCount; ++k)
		{
			if (k == i) continue;
			int* b = m_triangles[k].indices;
			if ((a[0] == b[0] || a[0] == b[1] || a[0] == b[2]) &&
				(a[1] == b[0] || a[1] == b[1] || a[1] == b[2]))
			{
				pPolys[v + 3] = k;
			}
			if ((a[1] == b[0] || a[1] == b[1] || a[1] == b[2]) &&
				(a[2] == b[0] || a[2] == b[1] || a[2] == b[2]))
			{
				pPolys[v + 4] = k;
			}
			if ((a[2] == b[0] || a[2] == b[1] || a[2] == b[2]) &&
				(a[0] == b[0] || a[0] == b[1] || a[0] == b[2]))
			{
				pPolys[v + 5] = k;
			}
		}
	}
	params.polys = pPolys;
	std::cout << "polyCount Setting Complete!" << std::endl;

	// 填充三角型区域标志
	unsigned char* pAreas = new unsigned char[params.polyCount];
	unsigned short* pPolyFlags = new unsigned short[params.polyCount];
	for (int i = 0; i < params.polyCount; ++i)
	{
		pAreas[i] = 63; // RC_WALKABLE_AREA
		pPolyFlags[i] = 0x01; // SAMPLE_POLYFLAGS_WALK
	}
	params.polyAreas = pAreas;
	params.polyFlags = pPolyFlags;


	unsigned char* navData = NULL;
	int navDataSize = 0;
	if (!dtCreateNavMeshData(&params, &navData, &navDataSize))
	{
		delete (pVerts);
		delete (pPolys);
		delete (pAreas);
		delete (pPolyFlags);
		std::cout << "dtCreateNavMeshData Fail" << std::endl;
		return false;
	}
	delete (pVerts);
	delete (pPolys);
	delete (pAreas);
	delete (pPolyFlags);

	m_pNavMesh = dtAllocNavMesh();
	if (m_pNavMesh == NULL)
	{
		delete navData;
		std::cout << "m_pNavMesh Fail" << std::endl;
		return false;
	}

	dtStatus status;
	status = m_pNavMesh->init(navData, navDataSize, DT_TILE_FREE_DATA);
	if (dtStatusFailed(status))
	{
		delete navData;
		std::cout << "dtStatusFailed Fail" << std::endl;
		return false;
	}


	m_pNavQuery = dtAllocNavMeshQuery();
	if (m_pNavQuery == NULL)
	{
		delete m_pNavMesh;
		std::cout << "m_pNavQuery Fail" << std::endl;
		return false;
	}

	status = m_pNavQuery->init(m_pNavMesh, 2048);
	if (dtStatusFailed(status))
	{
		delete m_pNavMesh;
		std::cout << "dtStatusFailed2 Fail" << std::endl;
		return false;
	}
	m_filePath = filepath;
	m_isLoaded = true;

	return true;
}

void DetourHelper::UnLoad()
{
	if (m_pNavQuery != NULL) delete m_pNavQuery;
	if (m_pNavMesh != NULL) delete m_pNavMesh;
	m_filePath.clear();
	m_isLoaded = false;
}

void DetourHelper::GetBound(DHVertex& min, DHVertex& max)
{
	min.x = m_bmin[0];
	min.y = m_bmin[1];
	min.z = m_bmin[2];
	max.x = m_bmax[0];
	max.y = m_bmax[1];
	max.z = m_bmax[2];
}

bool DetourHelper::IsPointNearNavMesh(float x, float z)
{
	if (!m_isLoaded)
	{
		return false;
	}
	float point[3] = { x, 0, z };
	float t[3] = { 0.1f, 1000, 0.1f };

	dtPolyRef ref;
	float result[3];
	dtStatus status = m_pNavQuery->findNearestPoly(point, t, &m_filter, &ref, result);
	if (!dtStatusSucceed(status))
	{
		return false;
	}
	if (dtAbs(result[0] - point[0]) > 1.0f || dtAbs(result[2] - point[2]) > 1.0f)
	{
		return false;
	}
	return true;
}

bool DetourHelper::GetHeight(float x, float z, float& y)
{
	if (!m_isLoaded)
	{
		return false;
	}
	//dtStatus status;
	float point[3] = { x, 0, z };
	float t[3] = { 0.5f, 1000, 0.5f };

	dtPolyRef ref[50];
	int count = 0;
	m_pNavQuery->queryPolygons(point, t, &m_filter, ref, &count, 50);
	if (count == 0)
	{
		return false;
	}
	for (int i = 0; i < count; ++i)
	{
		dtStatus status = m_pNavQuery->getPolyHeight(ref[i], point, &y);
		if (dtStatusSucceed(status))
		{
			return true;
		}
	}
	return false;
}

bool DetourHelper::FindPath(const DHVertex& start, const DHVertex& end, std::vector<DHVertex>& path, dtPolyRef& startPolyRef, dtPolyRef& endPolyRef)
{
	path.clear();

	if (!m_isLoaded)
	{
		return false;
	}
	if (m_pNavQuery == NULL)
	{
		return false;
	}

	dtStatus status;

	float s[3] = { start.x, 0, start.z };
	float e[3] = { end.x, 0, end.z };
	float t[3] = { 0.5f, 1000, 0.5f };
	float iterPos[3]{ 0 }, targetPos[3]{ 0 };

	dtPolyRef ref[50]{ 0 };
	int count = 0;
	m_pNavQuery->queryPolygons(s, t, &m_filter, ref, &count, 50);
	
	if (count == 0)
	{
		return false;
	}
	bool find = false;
	for (int i = 0; i < count; ++i)
	{
		float y;
		dtStatus status = m_pNavQuery->getPolyHeight(ref[i], s, &y);
		if (dtStatusSucceed(status))
		{
			startPolyRef = ref[i];
			iterPos[0] = s[0];
			iterPos[1] = y;
			iterPos[2] = s[2];
			find = true;
			break;
		}
	}
	if (!find)
	{
		return false;
	}
	status = m_pNavQuery->findNearestPoly(e, t, &m_filter, &endPolyRef, targetPos);
	if (dtStatusFailed(status))
	{
		return false;
	}
	dtPolyRef polys[MAX_POLYS]{ 0 };
	int npolys;
	status = m_pNavQuery->findPath(startPolyRef, endPolyRef, s, e, &m_filter, polys, &npolys, MAX_POLYS);
	if (dtStatusFailed(status))
	{
		return false;
	}
	if (npolys <= 0)
	{
		return false;
	}

	static const int MAX_SMOOTH = 2048;
	static const float STEP_SIZE = 0.5f;
	static const float SLOP = 0.01f;

	int nsmoothPath = 0;
	float smoothPath[MAX_SMOOTH * 3]{ 0 };

	memcpy(&smoothPath[nsmoothPath * 3], iterPos, sizeof(float) * 3);
	nsmoothPath++;
	DHVertex vertex;
	vertex.x = iterPos[0];
	vertex.y = iterPos[1];
	vertex.z = iterPos[2];
	path.push_back(vertex);

	// Move towards target a small advancement at a time until target reached or
	// when ran out of memory to store the path.
	while (npolys && nsmoothPath < MAX_SMOOTH)
	{
		// Find location to steer towards.
		float steerPos[3]{ 0 };
		unsigned char steerPosFlag{0};
		dtPolyRef steerPosRef{ 0 };

		if (!getSteerTarget(m_pNavQuery, iterPos, targetPos, SLOP,
			polys, npolys, steerPos, steerPosFlag, steerPosRef))
		{
			break;
		}
		
		bool endOfPath = (steerPosFlag & DT_STRAIGHTPATH_END) ? true : false;
		bool offMeshConnection = (steerPosFlag & DT_STRAIGHTPATH_OFFMESH_CONNECTION) ? true : false;

		// Find movement delta.
		float delta[3], len;
		dtVsub(delta, steerPos, iterPos);
		len = dtMathSqrtf(dtVdot(delta, delta));
		// If the steer target is end of path or off-mesh link, do not move past the location.
		if ((endOfPath || offMeshConnection) && len < STEP_SIZE)
			len = 1;
		else
			len = STEP_SIZE / len;
		float moveTgt[3];
		dtVmad(moveTgt, iterPos, delta, len);

		// Move
		float result[3];
		dtPolyRef visited[16]{ 0 };
		int nvisited = 0;
		m_pNavQuery->moveAlongSurface(polys[0], iterPos, moveTgt, &m_filter,
			result, visited, &nvisited, 16);

		npolys = fixupCorridor(polys, npolys, MAX_POLYS, visited, nvisited);
		npolys = fixupShortcuts(polys, npolys, m_pNavQuery);

		float h = 0;
		m_pNavQuery->getPolyHeight(polys[0], result, &h);
		result[1] = h;
		dtVcopy(iterPos, result);

		// Handle end of path and off-mesh links when close enough.
		if (endOfPath && inRange(iterPos, steerPos, SLOP, 1.0f))
		{
			// Reached end of path.
			dtVcopy(iterPos, targetPos);
			if (nsmoothPath < MAX_SMOOTH)
			{
				dtVcopy(&smoothPath[nsmoothPath * 3], iterPos);
				nsmoothPath++;
				DHVertex vertex{ 0 };
				vertex.x = iterPos[0];
				vertex.y = iterPos[1];
				vertex.z = iterPos[2];
				path.push_back(vertex);
			}
			break;
		}
		else if (offMeshConnection && inRange(iterPos, steerPos, SLOP, 1.0f))
		{
			// Reached off-mesh connection.
			float startPos[3], endPos[3];

			// Advance the path up to and over the off-mesh connection.
			dtPolyRef prevRef = 0, polyRef = polys[0];
			int npos = 0;
			while (npos < npolys && polyRef != steerPosRef)
			{
				prevRef = polyRef;
				polyRef = polys[npos];
				npos++;
			}
			for (int i = npos; i < npolys; ++i)
				polys[i - npos] = polys[i];
			npolys -= npos;

			// Handle the connection.
			dtStatus status = m_pNavMesh->getOffMeshConnectionPolyEndPoints(prevRef, polyRef, startPos, endPos);
			if (dtStatusSucceed(status))
			{
				if (nsmoothPath < MAX_SMOOTH)
				{
					dtVcopy(&smoothPath[nsmoothPath * 3], startPos);
					nsmoothPath++;
					DHVertex vertex;
					vertex.x = iterPos[0];
					vertex.y = iterPos[1];
					vertex.z = iterPos[2];
					path.push_back(vertex);
					// Hack to make the dotted path not visible during off-mesh connection.
					if (nsmoothPath & 1)
					{
						dtVcopy(&smoothPath[nsmoothPath * 3], startPos);
						nsmoothPath++;
						DHVertex vertex;
						vertex.x = iterPos[0];
						vertex.y = iterPos[1];
						vertex.z = iterPos[2];
						path.push_back(vertex);
					}
				}
				// Move position at the other side of the off-mesh link.
				dtVcopy(iterPos, endPos);
				float eh = 0.0f;
				m_pNavQuery->getPolyHeight(polys[0], iterPos, &eh);
				iterPos[1] = eh;
			}
		}

		// Store results.
		if (nsmoothPath < MAX_SMOOTH)
		{
			dtVcopy(&smoothPath[nsmoothPath * 3], iterPos);
			nsmoothPath++;
			DHVertex vertex;
			vertex.x = iterPos[0];
			vertex.y = iterPos[1];
			vertex.z = iterPos[2];
			path.push_back(vertex);
		}
	}
	return true;
}

bool DetourHelper::FindPath(const DHVertex& start, const DHVertex& end, std::vector<DHVertex>& path)
{
	dtPolyRef startRef{ 0 }, endRef{ 0 };
	return FindPath(start, end, path, startRef, endRef);
}

bool DetourHelper::FindStraightPath(const DHVertex& start, const DHVertex& end, std::vector<DHVertex>& path)
{
	dtPolyRef startRef{ 0 }, endRef{ 0 };
	return FindStraightPath(start, end, path, startRef, endRef);
}

bool DetourHelper::FindStraightPath(const DHVertex& start, const DHVertex& end, std::vector<DHVertex>& path, dtPolyRef& startPolyRef, dtPolyRef& endPolyRef)
{
	path.clear();

	if (!m_isLoaded)
	{
		return false;
	}
	if (m_pNavQuery == NULL)
	{
		return false;
	}

	dtStatus status{ 0 };

	float s[3] = { start.x, 0, start.z };
	float e[3] = { end.x, 0, end.z };
	float t[3] = { 0.5f, 1000, 0.5f };
	float iterPos[3], targetPos[3];

	dtPolyRef ref[50]{ 0 };
	int count = 0;
	m_pNavQuery->queryPolygons(s, t, &m_filter, ref, &count, 50);
	if (count == 0)
	{
		return false;
	}
	bool find = false;
	for (int i = 0; i < count; ++i)
	{
		float y;
		dtStatus status = m_pNavQuery->getPolyHeight(ref[i], s, &y);
		if (dtStatusSucceed(status))
		{
			startPolyRef = ref[i];
			iterPos[0] = s[0];
			iterPos[1] = y;
			iterPos[2] = s[2];
			find = true;
			break;
		}
	}
	if (!find)
	{
		return false;
	}
	status = m_pNavQuery->findNearestPoly(e, t, &m_filter, &endPolyRef, targetPos);
	if (dtStatusFailed(status))
	{
		return false;
	}
	dtPolyRef polys[MAX_POLYS]{ 0 };
	int npolys;
	status = m_pNavQuery->findPath(startPolyRef, endPolyRef, s, e, &m_filter, polys, &npolys, MAX_POLYS);
	if (dtStatusFailed(status))
	{
		return false;
	}
	if (npolys <= 0)
	{
		return false;
	}

	int nstraightPath = 0;
	// In case of partial path, make sure the end point is clamped to the last polygon.
	float endPos[3];
	dtVcopy(endPos, e);
	if (polys[npolys - 1] != endPolyRef)
	{
		m_pNavQuery->closestPointOnPoly(polys[npolys - 1], e, endPos, 0);
	}

	float straightPath[MAX_POLYS * 3];
	unsigned char straightPathFlags[MAX_POLYS];
	dtPolyRef straightPathPolys[MAX_POLYS];
	status = m_pNavQuery->findStraightPath(s, endPos, polys, npolys, straightPath, straightPathFlags, straightPathPolys, &nstraightPath, MAX_POLYS, 0);
	if (dtStatusFailed(status))
	{
		return false;
	}
	if (nstraightPath <= 0)
	{
		return false;
	}

	for (int i = 0; i < nstraightPath * 3; )
	{
		DHVertex vertex;
		vertex.x = straightPath[i++];
		vertex.y = straightPath[i++];
		vertex.z = straightPath[i++];
		path.push_back(vertex);
	}
	return true;
}

