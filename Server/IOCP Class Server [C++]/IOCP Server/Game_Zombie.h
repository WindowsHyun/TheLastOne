#ifndef __GAMEZOMBIE_H__
#define __GAMEZOMBIE_H__

#include "Core_Header.h"

class Game_Zombie {
private:
	int client_id = -1;
	xyz position;
	xyz rotation;
	int hp = -1;
	int animator = 0;
	int target_Player = -1;
	bool live = false;

public:
	bool get_live() { return this->live; }																		// 좀비 생사 여부 전달
	int get_client_id() { return this->client_id; }																// 좀비 아이디 전달
	int get_hp() { return this->hp; };																			// 좀비 체력 전달
	int get_animator() { return this->animator; };															// 좀비 애니메이션 전달
	int get_target() { return this->target_Player; };															// 좀비 타켓 플레이어 전달
	xyz get_pos() { return this->position; };
	xyz get_position() { return xyz(this->position.x, this->position.y, this->position.z); };		// 좀비 포지션 전달
	xyz get_rotation() { return xyz(this->rotation.x, this->rotation.y, this->rotation.z); };		// 좀비 로테이션 전달


	void set_live(const bool value) { this->live = value; };										// 좀비 생사 여부 저장
	void set_target(const int value) { this->target_Player = value; };						// 좀비 타켓 플레이어 저장
	void set_animator(const int value) { this->animator = value; };							// 좀비 애니메이션 저장
	void set_hp(const int value) { this->hp = value; };											// 좀비 체력 저장
	void set_zombie_position(const xyz position) { this->position = position; };			// 좀비 포지션 저장
	void set_zombie_rotation(const xyz rotation) { this->rotation = rotation; };			// 좀비 로테이션 저장
	void set_zombie_animator(const int value) { this->animator = value; };					// 좀비 애니메이션 저장

	Game_Zombie(const int client_id);
	~Game_Zombie();
};
void init_Zombie(int count, std::unordered_map<int, Game_Zombie>* zombie);
#endif