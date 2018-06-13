#include "Room_Manager.h"

Room_Manager::Room_Manager(const int id, const int mapType)
{
	this->id = id;
	this->gameStatus = LobbyStatus;
	//this->g_hiocp = g_hiocp;
	this->mapType = mapType;
	this->get_room().set_mapType(mapType);
	//::cout << "Room : " << this->id << std::endl;
}

Room_Manager::Room_Manager(const Room_Manager & g_r)
{
	this->id = g_r.id;
	this->mapType = g_r.mapType;
	this->gameStatus = g_r.gameStatus;
	this->inGame = g_r.inGame;
	//this->g_hiocp = g_r.g_hiocp;
}

Room_Manager::~Room_Manager()
{
}
