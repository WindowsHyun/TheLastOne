#include <iostream>
#pragma comment(lib, "detourhelper.lib")
#include "dllexport.h"

bool mapLoaded = false;

int main() {
	auto iter = dh_create();
	//mapLoaded = dh_load(iter, "./navmesh_demo.obj");
	mapLoaded = dh_load(iter, "./InGameScene.navmesh");

	DHVertex start{ 1007.09f, 29.99451f, 344.01f };
	DHVertex end{ 1060.153f, 29.99451f, 1114.969f };
	std::vector<DHVertex> path;
	int size =0;

	size = dh_findpath(iter, &start, &end, &path, &size);

	for (auto in : path) {
		std::cout << in.x << ", " << in.y << ", " << in.z << std::endl;
	}
	
}