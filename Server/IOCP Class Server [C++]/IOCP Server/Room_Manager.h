#ifndef __GAMEROOM_H__
#define __GAMEROOM_H__
#include "Game_Room.h"


class Room_Manager {
private:
	int id;		// 룸 ID
	int mapType;
	int gameStatus;
	HANDLE g_hiocp;
	Game_Room inGame;		// 실제 게임 데이터.	

public:
	Game_Room & get_room() { return this->inGame; }		// 실제 게임룸을 전달.
	int get_status() { return this->gameStatus; }			// 룸에 대한 상태를 전달.
	int get_id() { return this->id; }		// 룸 아이디를 전달.
	int get_mapType() { return this->mapType; }

	void set_mapType(int value) { this->mapType = value; }
	void set_status(int value) { this->gameStatus = value; }		// 룸에 대한 상태를 저장.
	Room_Manager(const int id, const int mapType, const HANDLE g_hiocp);
	Room_Manager(const Room_Manager& g_r);
	~Room_Manager();
};
#endif