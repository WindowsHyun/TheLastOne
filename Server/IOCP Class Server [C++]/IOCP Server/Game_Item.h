#ifndef __GAMEITEM_H__
#define __GAMEITEM_H__

#include "IOCP_Server.h"
#include <fstream>

struct xz {
	float x;
	float y;
	float z;
};

class Game_Item {
private:
	std::string item_name;
	xz item_position;
	xz item_rotation;
	bool item_eat;
	int item_Kinds;

public:
	xz get_pos() { return item_position; }
	xz get_rotation() { return item_rotation; }
	bool get_eat() { return item_eat; }
	int get_kind() { return item_Kinds; }
	std::string get_name() { return item_name; }

	void set_eat(bool value) { this->item_eat = value; }
	void set_pos(float x, float z) { this->item_position.x = x; this->item_position.z = z; }
	void set_rotation(float x, float y, float z);
	Game_Item(const float x, const float z, const std::string item_name);
	~Game_Item();
};

void load_item_txt(std::string filepath, std::unordered_map< int, Game_Item> *item);
std::string splitParsing(const std::string content, const std::string start, const std::string end);
#endif