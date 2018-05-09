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
	float dist = 0.0f;
	for (auto zombie : g_zombie) {

		if (zombie.second.get_target() != -1 && g_clients.find(zombie.second.get_target()) == g_clients.end()) {
			// 플레이어가 강제 종료 할 경우 좀비 초기화
			zombie.second.set_distance(Zombie_Dist);
			zombie.second.set_target(-1);
		}
		if (zombie.second.get_hp() <= 0 && zombie.second.get_live() != false) {
			// 좀비의 체력이 0일경우
			g_zombie.find(zombie.first)->second.set_live(false);
			g_zombie.find(zombie.first)->second.set_target(-1);
		}

		for (auto player : g_clients) {
			dist = DistanceToPoint(player.second.get_position(), zombie.second.get_position());

			if (dist <= zombie.second.get_distance() && dist <= Zombie_Dist && player.second.get_hp() > 0) {
				// 최근 측정한 거리가 더 멀경우 가까운 거리를 지정해준다.
				g_zombie.find(zombie.first)->second.set_distance(dist);
				g_zombie.find(zombie.first)->second.set_target(player.second.get_client_id());
			}
			if (dist >= Zombie_Dist) {
				// 플레이어가 존재하지만 거리가 멀어져 있을 경우 좀비 Target을 초기화
				zombie.second.set_distance(Zombie_Dist);
				g_zombie.find(zombie.first)->second.set_target(-1);
			}
		}
		// 플레이어를 모두 확인한 이후 zombie dist는 초기화 해준다.
		zombie.second.set_distance(Zombie_Dist);
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
