#ifndef __GAMEDANGERLINE_H__
#define __GAMEDANGERLINE_H__

#include "IOCP_Server.h"

class Game_DangerLine {
private:
	int level;			// 자기장 레벨
	int demage;		// 자기장 데미지
	xyz position;	// 생성될 위치
	xyz scale;			// 자기장 크기


public:
	
	Game_DangerLine();
	~Game_DangerLine();
};


#endif