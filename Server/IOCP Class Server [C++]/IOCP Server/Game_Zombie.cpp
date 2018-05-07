#include "Game_Zombie.h"

Game_Zombie::Game_Zombie(const int client_id)
{
	//< 1단계. 시드 설정
	std::random_device rn;
	std::mt19937_64 rnd(rn());
	std::uniform_int_distribution<int> xRange(300, 1700);
	std::uniform_int_distribution<int> zRange(260, 2750);

	this->client_id = client_id;
	this->hp = Zombie_HP;
	this->position.x = (float)xRange(rnd);
	this->position.y = 29.99451f;
	this->position.z = (float)zRange(rnd);
	this->rotation.x = 0;
	this->rotation.y = 0;
	this->rotation.z = 0;
	this->target_Player = -1;
	this->distance = Zombie_Dist;		// 초기값은 가장 높은 값으로 지정해준다.
	this->live = true;
}

Game_Zombie::~Game_Zombie()
{
}

void init_Zombie(int count, std::unordered_map<int, Game_Zombie>* zombie)
{
	for (int i = 0; i < count; ++i) {
		zombie->insert(std::pair<int, Game_Zombie>(i, { i }));
	}
}
