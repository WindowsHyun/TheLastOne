#include "Room_Manager.h"

Room_Manager::Room_Manager(const int id, const HANDLE g_hiocp)
{
	this->id = id;
	this->gameStatus = LobbyStatus;
	this->g_hiocp = g_hiocp;
	//::cout << "Room : " << this->id << std::endl;
}

Room_Manager::Room_Manager(const Room_Manager & g_r)
{
	this->id = g_r.id;
	this->gameStatus = g_r.gameStatus;
	this->g_hiocp = g_r.g_hiocp;
}

Room_Manager::~Room_Manager()
{
}
