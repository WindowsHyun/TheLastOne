#ifndef __GAMEZOMBIE_H__
#define __GAMEZOMBIE_H__

#include "IOCP_Server.h"

class Game_Zombie {
private:
	int client_id = -1;
	xyz position;
	xyz rotation;
	int hp = -1;
	int animator = 0;

public:
	int get_client_id() { return this->client_id; }																// 클라이언트 아이디 전달
	int get_hp() { return this->hp; };																			// 클라이언트 체력 전달
	int get_animator() { return this->animator; };															// 클라이언트 애니메이션 전달
	Vec3 get_position() { return Vec3(this->position.x, this->position.y, this->position.z); };		// 클라이언트 포지션 전달
	Vec3 get_rotation() { return Vec3(this->rotation.x, this->rotation.y, this->rotation.z); };		// 클라이언트 로테이션 전달


	void set_client_position(const xyz position) { this->position = position; };			// 클라이언트 포지션 저장
	void set_client_rotation(const xyz rotation) { this->rotation = rotation; };			// 클라이언트 로테이션 저장
	void set_client_animator(const int value) { this->animator = value; };					// 클라이언트 애니메이션 저장

	Game_Zombie(const int client_id);
	~Game_Zombie();
};

void init_Zombie(int count, std::unordered_map<int, Game_Zombie>* zombie);
#endif