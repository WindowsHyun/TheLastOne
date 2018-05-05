#include "Game_Zombie.h"

Game_Zombie::Game_Zombie(const int client_id)
{
	//< 1단계. 시드 설정
	std::random_device rn;
	std::mt19937_64 rnd(rn());
	std::uniform_int_distribution<int> xRange(100, 2000);
	std::uniform_int_distribution<int> zRange(120, 500);

	this->client_id = client_id;
	this->hp = Zombie_HP;
	//this->position.x = (float)(rand() % 2000) + 100;
	this->position.x = 1007.09f;
	this->position.y = 29.99451f;
	this->position.z = (float)zRange(rnd);
	//this->position.z = 347.01f;
	this->rotation.x = 0;
	this->rotation.y = 0;
	this->rotation.z = 0;
	this->target_Player = -1;
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
