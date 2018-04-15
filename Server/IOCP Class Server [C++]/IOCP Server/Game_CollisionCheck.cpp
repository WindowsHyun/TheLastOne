#include "Game_CollisionCheck.h"

Game_CollisionCheck::Game_CollisionCheck(float startX, float startY, float endX, float endY)
{
	this->startX = startX;
	this->startY = startY;
	this->endX = endX;
	this->endY = endY;
}

Game_CollisionCheck::~Game_CollisionCheck()
{
}


void CollisionCheck(xyz player, std::unordered_map<int, Game_CollisionCheck>* collision)
{
	for (auto iter : *collision) {

		// rc1 을 플레이어
		RECT rc1;
		rc1.left = player.x - 0.6;
		rc1.right = player.x + 0.6;
		rc1.top = player.z + 0.6;
		rc1.bottom = player.z - 0.6;
		// rc2 를 오브젝트
		RECT rc2;
		rc2.left = iter.second.get_startX();
		rc2.right = iter.second.get_endX();
		rc2.top = iter.second.get_endY();
		rc2.bottom = iter.second.get_startY();

		if (rc1.left  <=	rc2.right	&&
			rc1.right >= rc2.left && 
			rc1.top	>=	rc2.bottom	&&
			rc1.bottom	<=	rc2.top)
		{
			//충돌시
			std::cout << iter.second.get_startX() << " ~ " << iter.second.get_endX() << " | " << iter.second.get_startY() << " ~ " << iter.second.get_endY() << std::endl;
			std::cout << player.x - 1 << " ~ " << player.x + 1 << " | " << player.z - 1 << " ~ " << player.z + 1 << std::endl;
			std::cout << "충돌..!" << std::endl << std::endl;
			break;
		}



		//if (player.x + 1.5	<	iter.second.get_endX() && player.z - 1.5	<	iter.second.get_endY() && player.x -1.5	>	iter.second.get_startX() && player.z +1.5 >	 iter.second.get_startY())
		//{
		//	
		//}

		//if (rc1.left	<	rc2.right	&& rc1.top	<	rc2.bottom	&& rc1.right	>	rc2.left	&& rc1.bottom	>	rc2.top) { 충돌시에 실행할 명령; }


		/*if (iter.second.get_startX() <= (player.x + 1.7)  &&  (player.x +1.7) <= iter.second.get_endX()) {
			if (iter.second.get_startY() <= (player.z + 1.7) && ( player.z + 1.7) <= iter.second.get_endY()) {

				std::cout << iter.second.get_startX() << " ~ " << iter.second.get_endX() << std::endl;
				std::cout << iter.second.get_startY() << " ~ " << iter.second.get_endY() << std::endl;
				std::cout << "충돌..!" << std::endl<< std::endl;
				break;
			}
		}*/

	}

}

void load_CollisionCheck_txt(std::string filepath, std::unordered_map<int, Game_CollisionCheck>* collision)
{
	int collision_num = 0;
	double box_X, box_Y;
	double start_X, end_X, start_Y, end_Y;
	std::ifstream openFile(filepath);
	if (openFile.is_open()) {
		//파일이 정상적일 경우
		std::string line;
		while (!openFile.eof()) {
			// 한줄 씩 읽는다.
			getline(openFile, line);
			
			box_X = atof(splitParsing(line, "posx:", "|").c_str()) - (abs(atof(splitParsing(line, "centerx:", "|").c_str()) * atof(splitParsing(line, "scalex:", "|").c_str())));
			start_X = box_X - ((atof(splitParsing(line, "sizex:", "|").c_str()) * atof(splitParsing(line, "scalex:", "|").c_str())) * 0.5f);
			end_X = box_X + ((atof(splitParsing(line, "sizex:", "|").c_str()) * atof(splitParsing(line, "scalex:", "|").c_str())) * 0.5f);

			box_Y = atof(splitParsing(line, "posz:", "|").c_str()) - (abs(atof(splitParsing(line, "centery:", "|").c_str()) * atof(splitParsing(line, "scaley:", "|").c_str())));
			start_Y = box_Y - ((atof(splitParsing(line, "sizey:", "|").c_str()) * atof(splitParsing(line, "scaley:", "|").c_str())) * 0.5f);
			end_Y = box_Y + ((atof(splitParsing(line, "sizey:", "|").c_str()) * atof(splitParsing(line, "scaley:", "|").c_str())) * 0.5f);

			std::cout << start_X << " ~ " << end_X << " | " << start_Y << " ~ " << end_Y << std::endl;

			collision->insert(std::pair<int, Game_CollisionCheck>(collision_num, {(float)start_X, (float)start_Y, (float)end_X, (float)end_Y }));
			++collision_num;
		}
	}
	else {
		std::cout << "No files found..!" << std::endl;
	}
	openFile.close();
}
