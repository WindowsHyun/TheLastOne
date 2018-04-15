#include "DetourHelper.h"
#include <string>
#include <vector>
#include <iostream>
#include "dllexport.h"

using namespace std;

extern "C" {

	void* dh_create() {
		auto ptr = new DetourHelper();
#if _DEBUG
		cout << "dh_create: " << ptr << endl;
#endif
		return ptr;
	}
	void dh_delete(void* ug) {
#if _DEBUG
		cout << "dh_delete: " << ug << endl;
#endif
		delete ug;
	}

	bool dh_load(void* ug, const char* path) {
		auto obj = (DetourHelper*)ug;
		string filename(path);
		auto res = obj->Load(filename);
#if _DEBUG
		cout << "dh_load:" << res << endl;
#endif
		return res;
	}
	void dh_unload(const void* ug) {
		auto obj = (DetourHelper*)ug;
		obj->UnLoad();
#if _DEBUG
		cout << "dh_unload" << endl;
#endif
	}

	void dh_get_triangles(const void* ug, void* &start, int* length) {
		auto obj = (DetourHelper*)ug;
		*length = obj->GetTriangles().size();
		start = (void*)obj->GetTriangles().data();
#if _DEBUG
		cout << "triangles:" << *length << endl;
		cout << "dh_get_triangles" << endl;
#endif
	}
	void dh_get_vectices(const void* ug, void* &start, int* length) {
		auto obj = (DetourHelper*)ug;
		*length = obj->GetVertices().size();
		start = (void*)obj->GetVertices().data();
#if _DEBUG
		cout << "dh_get_vectices:" << *length << "\tstart:" << start << endl;
#endif
	}
	void dh_get_indices(const void* ug, void* &start, int* length) {
		auto obj = (DetourHelper*)ug;
		*length = obj->GetIndices().size();
		start = (void*)obj->GetIndices().data();
#if _DEBUG
		cout << "dh_get_indices:" << *length << "\tstart:" << start << endl;
#endif
	}
	bool dh_findpath(const void* ug, void* start, void* end, void* vertices, int* length) {
		auto obj = (DetourHelper*)ug;
		vector<DHVertex>* path = (vector<DHVertex>*) vertices;
		if (path == nullptr) return false;
		path->clear();
		DHVertex v1 = *(DHVertex*)start;
		DHVertex v2 = *(DHVertex*)end;
		bool success = obj->FindPath(v1, v2, *path);
		*length = path->size();
#if _DEBUG
		cout << "dh_findpath: size:" << *length << endl;
#endif
		return success;
	}

	bool dh_findpath_cpp(const void* ug, void* start, void* end, vector<DHVertex>* path) {
		auto obj = (DetourHelper*)ug;
		if (path == nullptr) return false;
		path->clear();
		DHVertex v1 = *(DHVertex*)start;
		DHVertex v2 = *(DHVertex*)end;
		bool success = obj->FindPath(v1, v2, *path);
#if _DEBUG
		cout << "dh_findpath: size:" << path->size() << endl;
#endif
		return success;
	}

	void* dh_createpath() {
		auto p = new vector<DHVertex>();
		auto ret = (void*)p;
#if _DEBUG
		cout << "dh_createpath:" << ret << endl;
#endif
		return ret;
	}
	void dh_deletepath(void* path) {
		delete (vector<DHVertex>*)path;
	}

	void* dh_getpathdata(void* path) {
		auto v = (vector<DHVertex>*)path;
		return v->data();
	}

	bool dh_getheight(void* ug, float x, float z, float& y) {
		auto obj = (DetourHelper*)ug;
		return obj->GetHeight(x, z, y);
	}

	bool dh_is_point_near_navmesh(const void * ug, float x, float z)
	{
		auto obj = (DetourHelper*)ug;
		return obj->IsPointNearNavMesh(x, z);
	}

	bool dh_find_straight_path(const void * ug, void * start, void * end, void * vertices, int * length)
	{
		auto obj = (DetourHelper*)ug;
		vector<DHVertex>* path = (vector<DHVertex>*) vertices;
		if (path == nullptr) return false;
		path->clear();
		DHVertex v1 = *(DHVertex*)start;
		DHVertex v2 = *(DHVertex*)end;
		bool success = obj->FindStraightPath(v1, v2, *path);
		*length = path->size();
#if _DEBUG
		cout << "dh_find_straight_path: size:" << *length << endl;
#endif
		return success;
	}

	bool dh_find_straight_path_cpp(void * ug, const DHVertex & start, const DHVertex & end, std::vector<DHVertex>& path)
	{
		auto obj = (DetourHelper*)ug;
		bool success = obj->FindStraightPath(start, end, path);
#if _DEBUG
		cout << "dh_find_straight_path_cpp: size:" << path.size() << endl;
#endif // _DEBUG

		return success;
	}

}