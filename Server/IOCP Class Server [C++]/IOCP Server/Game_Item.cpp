#include "Game_Item.h"

Game_Item::Game_Item(const float x, const float z, const std::string item_name)
{
	this->item_position.x = x;
	this->item_position.z = z;
	this->item_name = item_name;
	this->item_eat = false;
}

Game_Item::Game_Item(Game_Item & g_cl)
{
	this->item_eat = g_cl.item_eat;
	this->item_name = g_cl.item_name;
	//this->item_position = g_cl.item_position;
}

Game_Item::~Game_Item()
{
}
