#ifndef __GAMECOLLISIONCHECK_H__
#define __GAMECOLLISIONCHECK_H__

#include "IOCP_Server.h"

class Game_CollisionCheck {
private:
	float startX, startY;
	float endX, endY;

public:
	float get_startX() { return this->startX; }
	float get_startY() { return this->startY; }
	float get_endX() { return this->endX; }
	float get_endY() { return this->endY; }

	Game_CollisionCheck(float startX, float startY, float endX, float endY);
	~Game_CollisionCheck();
};

void CollisionCheck(xyz player, std::unordered_map< int, Game_CollisionCheck> *collision);
void load_CollisionCheck_txt(std::string filepath, std::unordered_map< int, Game_CollisionCheck> *collision);
#endif