#include "Game_Item.h"

void Game_Item::set_rotation(float x, float y, float z)
{
	this->item_rotation.x = x;
	this->item_rotation.y = y;
	this->item_rotation.z = z;
}

Game_Item::Game_Item(const float x, const float y, const float z, const std::string item_name) {
	this->item_position.x = x;
	this->item_position.y = y;
	this->item_position.z = z;

	this->item_rotation.x = 0;
	this->item_rotation.y = 0;
	this->item_rotation.z = 0;

	this->item_name = item_name;
	this->item_eat = false;
	this->riding_car = false;
	if (item_name == "UAZ")
		this->hp = UAZ_HP;
	else if (item_name == "JEEP")
		this->hp = JEEP_HP;
	else
		this->hp = 0;
	if (item_name == "UAZ" || item_name == "JEEP")
		this->item_Kinds = Kind_Car;
	else
		this->item_Kinds = Kind_Item;
	this->Car_Kmh = 0.0f;
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
			item->insert(std::pair<int, Game_Item>(item_num, { (float)atof(splitParsing(line, "x:", "|").c_str()), (float)atof(splitParsing(line, "y:", "|").c_str()) , (float)atof(splitParsing(line, "z:", "|").c_str()),  splitParsing(line, "name:", "|") }));
			if (splitParsing(line, "name:", "|") == "UAZ" || splitParsing(line, "name:", "|") == "JEEP") {
				// UAZ의 경우 rotation값을 추가로 넣어준다.
				auto iter = item->find(item_num);
				iter->second.set_rotation((float)atof(splitParsing(line, "rx:", "|").c_str()), (float)atof(splitParsing(line, "ry:", "|").c_str()), (float)atof(splitParsing(line, "rz:", "|").c_str()));
			}
			++item_num;
		}
	}
	else {
		std::cout << "No files found..!" << std::endl;
	}
	openFile.close();
}
