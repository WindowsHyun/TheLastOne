#ifndef __GAMEDANGERLINE_H__
#define __GAMEDANGERLINE_H__

#include "Core_Header.h"

class Game_DangerLine {
private:
	int level;			// 자기장 레벨
	int demage;		// 자기장 데미지
	xyz position;	// 생성될 위치
	xyz scale;			// 자기장 크기
	xyz now_scale;	// 현재 자기장 크기
	int time;			// 자기장 대기 시간

public:
	void set_level(int value);
	void set_scale(int value);
	int get_level() { return this->level; }
	int get_time() { return this->time; }
	int get_demage() { return this->demage; }
	float get_scale_x() { return this->scale.x; }
	float get_now_scale_x() { return this->now_scale.x; }
	xyz get_pos() { return this->position; }
	xyz get_scale() { return this->now_scale; }

	void init();
	Game_DangerLine();
	~Game_DangerLine();
};


#endif