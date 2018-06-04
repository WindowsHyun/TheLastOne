#include "Game_Room.h"

std::unordered_map<int, Game_Client>::iterator Game_Room::get_client_iter(int ci)
{
	if (g_clients.find(ci) == g_clients.end()) {
		return g_clients.end();
	}
	else {
		return g_clients.find(ci);
	}
}

std::unordered_map<int, Game_Item>::iterator Game_Room::get_item_iter(int ci)
{
	return g_item.find(ci);
}

std::unordered_map<int, Game_Zombie>::iterator Game_Room::get_zombie_iter(int ci)
{
	return g_zombie.find(ci);
}

float DistanceToPoint(const xyz player, const xyz zombie)
{
	// 플레이어와 좀비간의 거리 구하기.
	return (float)sqrt(pow(player.x - zombie.x, 2) + pow(player.z - zombie.z, 2));
}

void Game_Room::player_To_Zombie()
{
	// 클라이언트에서 좀비의 target을 설정한다.
	// 서버에서는 좀비와 target된 플레이어 거리가 멀어진 경우에만 target을 -1로 변경해 준다.
	float dist = 0.0f;
	for (auto zombie : g_zombie) {
		if (zombie.second.get_live() == false)
			// 좀비가 죽어 있을 경우 넘긴다.
			continue;
		if (zombie.second.get_hp() <= 0 && zombie.second.get_live() != false) {
			// 좀비의 체력이 0일경우
			g_zombie.find(zombie.first)->second.set_live(false);
			g_zombie.find(zombie.first)->second.set_target(-1);
		}
		if (zombie.second.get_target() == -1)
			// 좀비의 Target이 정해져 있지 않으면 넘긴다.
			continue;

		if (g_clients.find(zombie.second.get_target()) == g_clients.end())
			// 플레이어가 없을 경우.
			continue;

		dist = DistanceToPoint(g_clients.find(zombie.second.get_target())->second.get_position(), zombie.second.get_position());

		if (dist >= Zombie_Dist) {
			// 플레이어가 존재하지만 거리가 멀어져 있을 경우 좀비 Target을 초기화
			zombie.second.set_distance(Zombie_Dist);
			g_zombie.find(zombie.first)->second.set_target(-1);
		}
		else if (g_clients.find(zombie.second.get_target())->second.get_hp() < 0) {
			// 플레이어가 체력이 0 이하로 내려갔을 경우.
			zombie.second.set_distance(Zombie_Dist);
			g_zombie.find(zombie.first)->second.set_target(-1);
		}

	}


}

int Game_Room::check_ReadyClients()
{
	int readyClients = 0;
	for (auto iter : g_clients) {
		if (iter.second.get_playerStatus() == ReadyStatus) {
			++readyClients;
		}
	}
	return readyClients;
}

void Game_Room::room_init()
{
	g_item.clear();
	g_zombie.clear();
	g_clients.clear();
	dangerLine.init();

	this->playGame = false;

	// 게임 아이템 정보 g_item에 넣어주기.
	load_item_txt("./Game_Item_Collection.txt", &g_item);

	// 좀비 캐릭터 생성하기
	init_Zombie(Create_Zombie, &g_zombie);
}

void Game_Room::set_mapType(int mapType)
{
	this->mapType = mapType;
}

Game_Room::Game_Room()
{
	// 게임 아이템 정보 g_item에 넣어주기.
	load_item_txt("./Game_Item_Collection.txt", &g_item);

	// 좀비 캐릭터 생성하기
	init_Zombie(Create_Zombie, &g_zombie);

	this->playGame = false;
	/*for (auto iter : g_zombie) {
		std::cout << iter.first << " : " << iter.second.get_pos().x << ", " << iter.second.get_pos().y << ", " << iter.second.get_pos().z << std::endl;
	}*/

}

Game_Room::~Game_Room()
{
}
