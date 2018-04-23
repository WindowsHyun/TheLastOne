#pragma once

#include <string>
#include <vector>
#include "DetourCommon.h"
#include "DetourNavMesh.h"
#include "DetourNavMeshBuilder.h"
#include "DetourNavMeshQuery.h"


enum SamplePolyFlags
{
	SAMPLE_POLYFLAGS_WALK = 0x01,		// Ability to walk (ground, grass, road)
	SAMPLE_POLYFLAGS_SWIM = 0x02,		// Ability to swim (water).
	SAMPLE_POLYFLAGS_DOOR = 0x04,		// Ability to move through doors.
	SAMPLE_POLYFLAGS_JUMP = 0x08,		// Ability to jump.
	SAMPLE_POLYFLAGS_DISABLED = 0x10,		// Disabled polygon
	SAMPLE_POLYFLAGS_ALL = 0xffff	// All abilities.
};

enum SamplePolyAreas
{
	SAMPLE_POLYAREA_GROUND,
	SAMPLE_POLYAREA_WATER,
	SAMPLE_POLYAREA_ROAD,
	SAMPLE_POLYAREA_DOOR,
	SAMPLE_POLYAREA_GRASS,
	SAMPLE_POLYAREA_JUMP,
};

// 顶点结构
struct DHVertex
{
	float x; // x坐标
	float y; // y坐标
	float z; // z坐标
};
// 三角型结构
struct DHTriangle
{
	DHVertex vertices[3]; // 顶点
	int indices[3]; // 索引
};

class DetourHelper
{
public:
	// 
	DetourHelper();
	//
	~DetourHelper();
	// 加载地图navmesh数据
	bool Load(const std::string& filepath); 
	// 卸载navmesh数据
	void UnLoad(); 
	// 查询给定xz平面点是否在navmesh上（非精确）
	bool IsPointNearNavMesh(float x, float z); 
	// 寻路（得到连续路径点）
	bool FindPath(const DHVertex& start, const DHVertex& end, std::vector<DHVertex>& path); 
	// 寻路（得到连续路径点）
	bool FindPath(const DHVertex& start, const DHVertex& end, std::vector<DHVertex>& path, dtPolyRef& startPolyRef, dtPolyRef& endPolyRef); 
	// 寻路（得到拐点）
	bool FindStraightPath(const DHVertex& start, const DHVertex& end, std::vector<DHVertex>& path); 
	// 寻路（得到拐点）
	bool FindStraightPath(const DHVertex& start, const DHVertex& end, std::vector<DHVertex>& path, dtPolyRef& startPolyRef, dtPolyRef& endPolyRef); 
	// 获取到xz平面上一点在navmesh上的高度（精确），如果该点不在navmesh上，那么返回false
	bool GetHeight(float x, float z, float& y); 
	// 获得包围盒最小点和最大点数据
	void GetBound(DHVertex& min, DHVertex& max); 
	// 获取到三角型列表
	const std::vector<DHTriangle>& GetTriangles() { return m_triangles; } 
	// 获取到顶点列表
	const std::vector<DHVertex>& GetVertices() { return m_vertices; } 
	// 获取到索引列表
	const std::vector<int>& GetIndices() { return m_indices; } 

private:
	std::vector<DHVertex> m_vertices;
	std::vector<int> m_indices;
	std::vector<DHTriangle> m_triangles;
	float m_bmin[3];
	float m_bmax[3];
	dtNavMesh* m_pNavMesh;
	dtNavMeshQuery* m_pNavQuery;
	dtQueryFilter m_filter;
	static const int MAX_POLYS = 256;
	bool m_isLoaded;
	int m_vertCount;
	std::string m_filePath;
};

/// Returns the minimum of two values.
///  @param[in]		a	Value A
///  @param[in]		b	Value B
///  @return The minimum of the two values.
template<class T> inline T rcMin(T a, T b) { return a < b ? a : b; }

/// Returns the maximum of two values.
///  @param[in]		a	Value A
///  @param[in]		b	Value B
///  @return The maximum of the two values.
template<class T> inline T rcMax(T a, T b) { return a > b ? a : b; }