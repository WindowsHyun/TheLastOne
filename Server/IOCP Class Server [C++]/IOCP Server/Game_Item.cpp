#include "Game_Item.h"

Game_Item::Game_Item(const double x, const double z, const std::string item_name) {

	this->item_position.x = x;
	this->item_position.z = z;
	this->item_name = item_name;
	//strncpy(this->item_name, item_name, 10);
	this->item_eat = false;
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

void load_g_item(std::string filepath, std::unordered_map<int, Game_Item>* item)
{
	int item_num = 0;
	std::ifstream openFile(filepath);
	if (openFile.is_open()) {
		//파일이 정상적일 경우
		std::string line;
		while (!openFile.eof()) {
			// 한줄 씩 읽는다.
			getline(openFile, line);
			item->insert(std::pair<int, Game_Item>(item_num, { atof(splitParsing(line, "x:", "|").c_str()) , atof(splitParsing(line, "z:", "|").c_str()),  splitParsing(line, "name:", "|") }));
			++item_num;
		}
	}
	else {
		std::cout << "No files found..!" << std::endl;
	}
	openFile.close();
}
