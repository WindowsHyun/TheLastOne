#ifndef __INGAME_H__
#define __INGAME_H__

#include "Game_Client.h"
#include "Game_Item.h"
#include "Game_Zombie.h"
#include "Game_DangerLine.h"


class Game_Room {
private:
	int mapType = -1;
	std::unordered_map< int, Game_Client> g_clients;
	std::unordered_map< int, Game_Item> g_item;
	std::unordered_map< int, Game_Zombie> g_zombie;
	Game_DangerLine dangerLine;
	bool playGame;

public:
	bool get_playGame() { return this->playGame; }
	std::unordered_map< int, Game_Client> & get_client() { return this->g_clients; }		// 클라이언트 데이터 전달
	std::unordered_map< int, Game_Item> & get_item() { return this->g_item; }		// 아이템 데이터 전달
	std::unordered_map< int, Game_Zombie> & get_zombie() { return this->g_zombie; }	// 좀비 데이터 전달
	Game_DangerLine & get_dangerLine() { return this->dangerLine; }		// DangerLine 데이터 전달


	void set_playGame(bool value) { this->playGame = value; }
	std::unordered_map< int, Game_Client>::iterator get_client_iter(int ci);
	std::unordered_map< int, Game_Item>::iterator get_item_iter(int ci);
	std::unordered_map< int, Game_Zombie>::iterator get_zombie_iter(int ci);
	void player_To_Zombie();
	int check_ReadyClients();
	void room_init();
	int get_mapType();
	void set_mapType(int mapType);

	Game_Room();
	~Game_Room();
};


#endif