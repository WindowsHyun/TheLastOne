#include <iostream>
#define _USE_MATH_DEFINES
#include <math.h>
#include "Recast.h"
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

	//------------------------
	// 유니티 기준 플레이어 위치 x, z
	int mx = 1009;
	int my = 1150;
	//------------------------
	// 내가 목표로 하는 위치 x, z
	int tx = 328;
	int ty = 801;
	//------------------------

	std::cout << mesh.load("./navmesh/InGame.obj") << std::endl;

	geom = new InputGeom;
	geom->loadMesh("./navmesh/InGame.obj");


}
