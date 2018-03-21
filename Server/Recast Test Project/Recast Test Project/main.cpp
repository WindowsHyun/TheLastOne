#include <iostream>
#define _USE_MATH_DEFINES
#include <math.h>
#include "Recast.h"
//#include "Sample_SoloMesh.h"
#include "MeshLoaderObj.h"
#include "DetourNavMesh.h"
#include "DetourNavMeshBuilder.h"
#include "InputGeom.h"

#ifdef WIN32
#	define snprintf _snprintf
#	define putenv _putenv
#endif

int main() {
	// 매쉬 데이터 체크
	rcMeshLoaderObj mesh;
	InputGeom* geom = 0;
	rcContext ctx;
	//----------------------------------------
	unsigned char* m_triareas;
	rcHeightfield* m_solid;
	rcCompactHeightfield* m_chf;
	rcContourSet* m_cset;
	rcPolyMesh* m_pmesh;
	rcConfig m_cfg;
	rcPolyMeshDetail* m_dmesh;
	class dtNavMesh* m_navMesh;
	//----------------------------------------
	float m_cellSize = 1.0f;
	float m_cellHeight = 0.2f;
	float m_agentHeight = 2.0f;
	float m_agentRadius = 0.6f;
	float m_agentMaxClimb = 0.9f;
	float m_agentMaxSlope = 45.0f;
	float m_regionMinSize = 8;
	float m_regionMergeSize = 20;
	bool m_monotonePartitioning = false;
	float m_edgeMaxLen = 12.0f;
	float m_edgeMaxError = 1.3f;
	float m_vertsPerPoly = 6.0f;
	float m_detailSampleDist = 6.0f;
	float m_detailSampleMaxError = 1.0f;

	bool m_filterLowHangingObstacles;
	bool m_filterLedgeSpans;
	bool m_filterWalkableLowHeightSpans;
	//------------------------
	// 유니티 기준 플레이어 위치 x, z
	int mx = 1009;
	int my = 1150;
	//------------------------
	// 내가 목표로 하는 위치 x, z
	int tx = 328;
	int ty = 801;
	//------------------------
	float rayStart[3]{ 1009, 29.99451f, 1150 };	// x, y, z
	float rayEnd[3]{ 328, 29.99451f, 801 };		// x, y, z
	float hitTime;

	//std::cout << mesh.load("./navmesh/InGame.obj") << std::endl;

	geom = new InputGeom;
	geom->loadMesh("./navmesh/InGame.obj");

	if (!geom || !geom->getMesh())
	{
		std::cout << RC_LOG_ERROR << "buildNavigation: Input mesh is not specified." << std::endl;
		return false;
	}

	//--------------------------------------------------
	// CleanUp

	//--------------------------------------------------


	const float* bmin = geom->getMeshBoundsMin();
	const float* bmax = geom->getMeshBoundsMax();
	const float* verts = geom->getMesh()->getVerts();
	const int nverts = geom->getMesh()->getVertCount();
	const int* tris = geom->getMesh()->getTris();
	const int ntris = geom->getMesh()->getTriCount();

	memset(&m_cfg, 0, sizeof(m_cfg));
	m_cfg.cs = m_cellSize;
	m_cfg.ch = m_cellHeight;
	m_cfg.walkableSlopeAngle = m_agentMaxSlope;
	m_cfg.walkableHeight = (int)ceilf(m_agentHeight / m_cfg.ch);
	m_cfg.walkableClimb = (int)floorf(m_agentMaxClimb / m_cfg.ch);
	m_cfg.walkableRadius = (int)ceilf(m_agentRadius / m_cfg.cs);
	m_cfg.maxEdgeLen = (int)(m_edgeMaxLen / m_cellSize);
	m_cfg.maxSimplificationError = m_edgeMaxError;
	m_cfg.minRegionArea = (int)rcSqr(m_regionMinSize);		// Note: area = size*size
	m_cfg.mergeRegionArea = (int)rcSqr(m_regionMergeSize);	// Note: area = size*size
	m_cfg.maxVertsPerPoly = (int)m_vertsPerPoly;
	m_cfg.detailSampleDist = m_detailSampleDist < 0.9f ? 0 : m_cellSize * m_detailSampleDist;
	m_cfg.detailSampleMaxError = m_cellHeight * m_detailSampleMaxError;

	rcVcopy(m_cfg.bmin, bmin);
	rcVcopy(m_cfg.bmax, bmax);
	rcCalcGridSize(m_cfg.bmin, m_cfg.bmax, m_cfg.cs, &m_cfg.width, &m_cfg.height);



	//std::cout << "bmin : " << bmin << ", bmax : " << bmax << std::endl;

	m_solid = rcAllocHeightfield();
	if (!m_solid)
	{
		std::cout << RC_LOG_ERROR << "buildNavigation: Out of memory 'solid'." << std::endl;
		return false;
	}
	if (!rcCreateHeightfield(*m_solid, m_cfg.width, m_cfg.height, m_cfg.bmin, m_cfg.bmax, m_cfg.cs, m_cfg.ch))
	{
		std::cout << RC_LOG_ERROR << "buildNavigation: Could not create solid heightfield." << std::endl;
		return false;
	}

	m_triareas = new unsigned char[ntris];
	if (!m_triareas)
	{
		std::cout << RC_LOG_ERROR << "buildNavigation: Out of memory 'm_triareas' (" << ntris << ")." << std::endl;
		return false;
	}
	memset(m_triareas, 0, ntris * sizeof(unsigned char));
	rcMarkWalkableTriangles(m_cfg.walkableSlopeAngle, verts, nverts, tris, ntris, m_triareas);
	if (!rcRasterizeTriangles(verts, nverts, tris, m_triareas, ntris, *m_solid, m_cfg.walkableClimb))
	{
		std::cout << RC_LOG_ERROR << "buildNavigation: Could not rasterize triangles." << std::endl;
		return false;
	}


	rcFilterLowHangingWalkableObstacles(m_cfg.walkableClimb, *m_solid);


	m_chf = rcAllocCompactHeightfield();

	if (!rcErodeWalkableArea(m_cfg.walkableRadius, *m_chf))
	{
		std::cout << RC_LOG_ERROR << "buildNavigation: Could not erode." << std::endl;
		return false;
	}

	//Sample_SoloMesh.cpp 에서  397번째 라인부터 분석을 하면 된다.

	const ConvexVolume* vols = geom->getConvexVolumes();
	for (int i = 0; i < geom->getConvexVolumeCount(); ++i) {
		rcMarkConvexPolyArea(vols[i].verts, vols[i].nverts, vols[i].hmin, vols[i].hmax, (unsigned char)vols[i].area, *m_chf);
	}

	// Partition the walkable surface into simple regions without holes.
	// Monotone partitioning does not need distancefield.
	if (!rcBuildRegionsMonotone( *m_chf, 0, m_cfg.minRegionArea, m_cfg.mergeRegionArea))
	{
		std::cout << RC_LOG_ERROR <<"buildNavigation: Could not build monotone regions." << std::endl;
		return false;
	}


	m_cset = rcAllocContourSet();
	if (!m_cset)
	{
		std::cout << RC_LOG_ERROR << "buildNavigation: Out of memory 'cset'." << std::endl;
		return false;
	}
	if (!rcBuildContours( *m_chf, m_cfg.maxSimplificationError, m_cfg.maxEdgeLen, *m_cset))
	{
		std::cout << RC_LOG_ERROR << "buildNavigation: Could not create contours." << std::endl;
		return false;
	}

	//SampleSoloMesh.cpp 601Page Step6부터 시작하면 됩니다..! 20180321







	m_pmesh = rcAllocPolyMesh();
	if (!m_pmesh)
	{
		std::cout << RC_LOG_ERROR << "buildNavigation: Out of memory 'pmesh'." << std::endl;
		return false;
	}

	m_dmesh = rcAllocPolyMeshDetail();
	if (!m_dmesh)
	{
		std::cout << RC_LOG_ERROR << "buildNavigation: Out of memory 'pmdtl'." << std::endl;
		return false;
	}

	if (m_cfg.maxVertsPerPoly <= 6) //DT_VERTS_PER_POLYGON
	{
		unsigned char* navData = 0;
		int navDataSize = 0;

		// Update poly flags from areas.
		for (int i = 0; i < m_pmesh->npolys; ++i)
		{
			if (m_pmesh->areas[i] == RC_WALKABLE_AREA)
				m_pmesh->areas[i] = 0x01;
		}

		dtNavMeshCreateParams params;
		memset(&params, 0, sizeof(params));
		params.verts = m_pmesh->verts;
		params.vertCount = m_pmesh->nverts;
		params.polys = m_pmesh->polys;
		params.polyAreas = m_pmesh->areas;
		params.polyFlags = m_pmesh->flags;
		params.polyCount = m_pmesh->npolys;
		params.nvp = m_pmesh->nvp;
		params.detailMeshes = m_dmesh->meshes;
		params.detailVerts = m_dmesh->verts;
		params.detailVertsCount = m_dmesh->nverts;
		params.detailTris = m_dmesh->tris;
		params.detailTriCount = m_dmesh->ntris;
		params.offMeshConVerts = geom->getOffMeshConnectionVerts();
		params.offMeshConRad = geom->getOffMeshConnectionRads();
		params.offMeshConDir = geom->getOffMeshConnectionDirs();
		params.offMeshConAreas = geom->getOffMeshConnectionAreas();
		params.offMeshConFlags = geom->getOffMeshConnectionFlags();
		params.offMeshConUserID = geom->getOffMeshConnectionId();
		params.offMeshConCount = geom->getOffMeshConnectionCount();
		params.walkableHeight = m_agentHeight;
		params.walkableRadius = m_agentRadius;
		params.walkableClimb = m_agentMaxClimb;
		rcVcopy(params.bmin, m_pmesh->bmin);
		rcVcopy(params.bmax, m_pmesh->bmax);
		params.cs = m_cfg.cs;
		params.ch = m_cfg.ch;
		params.buildBvTree = true;

		if (!dtCreateNavMeshData(&params, &navData, &navDataSize))
		{
			std::cout << RC_LOG_ERROR << "Could not build Detour navmesh." << std::endl;
			return false;
		}

		m_navMesh = dtAllocNavMesh();
		if (!m_navMesh)
		{
			dtFree(navData);
			std::cout << RC_LOG_ERROR << "Could not create Detour navmesh" << std::endl;
			return false;
		}


	}


	bool hit = geom->raycastMesh(rayStart, rayEnd, hitTime);
	/*
	std::cout << hit << std::endl;

	duDebugDrawTriMesh(&dd, m_geom->getMesh()->getVerts(), m_geom->getMesh()->getVertCount(),
	m_geom->getMesh()->getTris(), m_geom->getMesh()->getNormals(), m_geom->getMesh()->getTriCount(), 0, 1.0f);

	std::cout << geom->getMesh() << std::endl;
	*/
}
