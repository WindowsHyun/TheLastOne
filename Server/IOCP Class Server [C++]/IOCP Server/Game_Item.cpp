#include "Game_Item.h"

void Game_Item::set_rotation(float x, float y, float z)
{
	this->item_rotation.x = x; 
	this->item_rotation.y = y; 
	this->item_rotation.z = z;
}

Game_Item::Game_Item(const float x, const float z, const std::string item_name) {

	this->item_position.x = x;
	this->item_position.y = 0;
	this->item_position.z = z;

	this->item_rotation.x = 0;
	this->item_rotation.y = 0;
	this->item_rotation.z = 0;

	this->item_name = item_name;
	this->item_eat = false;
	if (item_name == "UAZ")
		this->item_Kinds = Kind_Car;
	else
		this->item_Kinds = Kind_Item;
}

Game_Item::~Game_Item()
{
}

std::string splitParsing(const std::string content, const std::string start, const std::string end) {
	int startPos, endPos;

	startPos = (int)content.find(start) + (int)start.length();
	endPos = (int)content.find(end) - startPos;

	return content.substr(startPos, endPos);
}

void load_item_txt(std::string filepath, std::unordered_map<int, Game_Item>* item)
{
	int item_num = 0;
	std::ifstream openFile(filepath);
	if (openFile.is_open()) {
		//파일이 정상적일 경우
		std::string line;
		while (!openFile.eof()) {
			// 한줄 씩 읽는다.
			getline(openFile, line);
			item->insert(std::pair<int, Game_Item>(item_num, { (float)atof(splitParsing(line, "x:", "|").c_str()) , (float)atof(splitParsing(line, "z:", "|").c_str()),  splitParsing(line, "name:", "|") }));
			++item_num;
		}
	}
	else {
		std::cout << "No files found..!" << std::endl;
	}
	openFile.close();
}
