#ifndef __GAMEITEM_H__
#define __GAMEITEM_H__

#include "IOCP_Server.h"
#include <fstream>

struct xz {
	double x;
	double z;
};

class Game_Item {
private:
	std::string item_name;
	xz item_position;
	bool item_eat;

public:
	double get_x() { return item_position.x; }
	double get_z() { return item_position.z; }
	bool get_eat() { return item_eat; }
	std::string get_name() { return item_name; }

	void set_eat(bool value) { this->item_eat = value; }
	Game_Item(const double x, const double z, const std::string item_name);
	~Game_Item();
};

void load_item_txt(std::string filepath, std::unordered_map< int, Game_Item> *item);
std::string splitParsing(const std::string content, const std::string start, const std::string end);
#endif