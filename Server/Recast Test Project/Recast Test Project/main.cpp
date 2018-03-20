#include <iostream>
#define _USE_MATH_DEFINES
#include <math.h>
#include "Recast.h"
//#include "Sample_SoloMesh.h"
#include "MeshLoaderObj.h"
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
	//----------------------------------------



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

	std::cout << mesh.load("./navmesh/InGame.obj") << std::endl;

	geom = new InputGeom;
	geom->loadMesh("./navmesh/InGame.obj");

	const float* bmin = geom->getMeshBoundsMin();
	const float* bmax = geom->getMeshBoundsMax();
	const float* verts = geom->getMesh()->getVerts();
	const int nverts = geom->getMesh()->getVertCount();
	const int* tris = geom->getMesh()->getTris();
	const int ntris = geom->getMesh()->getTriCount();

	std::cout << "bmin : " << bmin << ", bmax : " << bmax << std::endl;

	m_solid = rcAllocHeightfield();
	m_triareas = new unsigned char[ntris];
	memset(m_triareas, 0, ntris * sizeof(unsigned char));
	m_chf = rcAllocCompactHeightfield();

	//Sample_SoloMesh.cpp 에서  397번째 라인부터 분석을 하면 된다.

	const ConvexVolume* vols = geom->getConvexVolumes();


	bool hit = geom->raycastMesh(rayStart, rayEnd, hitTime);

	std::cout << hit << std::endl;
	/*
	duDebugDrawTriMesh(&dd, m_geom->getMesh()->getVerts(), m_geom->getMesh()->getVertCount(),
	m_geom->getMesh()->getTris(), m_geom->getMesh()->getNormals(), m_geom->getMesh()->getTriCount(), 0, 1.0f);
	*/
	std::cout << geom->getMesh() << std::endl;

}
