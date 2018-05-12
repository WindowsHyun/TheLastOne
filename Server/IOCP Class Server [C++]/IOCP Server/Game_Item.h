#ifndef __GAMEITEM_H__
#define __GAMEITEM_H__

#include "Core_Header.h"
#include <fstream>

//
//struct xz {
//	float x;
//	float y;
//	float z;
//};

class Game_Item {
private:
	std::string item_name;
	xyz item_position;
	xyz item_rotation;
	bool item_eat;
	bool riding_car;
	int hp;
	int item_Kinds;
	float Car_Kmh;

public:
	xyz get_position() { return this->item_position; }
	xyz get_rotation() { return this->item_rotation; }
	float get_Kmh() { return this->Car_Kmh; }
	bool get_eat() { return this->item_eat; }
	bool get_riding() { return this->riding_car; }
	int get_kind() { return this->item_Kinds; }
	int get_hp() { return this->hp; }
	std::string get_name() { return this->item_name; }

	void set_Kmh(float value) { this->Car_Kmh = value; }
	void set_eat(bool value) { this->item_eat = value; }
	void set_hp(int value) { this->hp = value; }
	void set_riding(bool value) { this->riding_car = value; }
	void set_pos(float x, float y, float z) { this->item_position.x = x;  this->item_position.y = y; this->item_position.z = z; }
	void set_rotation(float x, float y, float z);
	Game_Item(const float x, const float y, const float z, const std::string item_name);
	~Game_Item();
};

void load_item_txt(std::string filepath, std::unordered_map< int, Game_Item> *item);
std::string splitParsing(const std::string content, const std::string start, const std::string end);
#endif