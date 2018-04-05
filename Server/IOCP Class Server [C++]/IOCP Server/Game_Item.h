#ifndef __GAMEITEM_H__
#define __GAMEITEM_H__

#include "IOCP_Server.h"

struct xz {
	float x;
	float z;
};

class Game_Item {
private:
	std::string item_name;
	xz item_position;
	bool item_eat;

public:
	Game_Item(const float x, const float z, const std::string item_name);
	Game_Item(Game_Item& g_cl);
	~Game_Item();

	bool operator()(const Game_Item *comp1, const Game_Item *comp2) const {
		return comp1->item_position.x < comp2->item_position.x;
	}
};


#endif