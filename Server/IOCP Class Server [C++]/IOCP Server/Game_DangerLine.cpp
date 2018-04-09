#include "Game_DangerLine.h"

void Game_DangerLine::set_level(int value)
{
	if (value == 4) {
		this->level = value;
		this->demage = 2;
		this->scale.x = 2700.0f;
		this->scale.y = 2700.0f;
		this->scale.z = 2700.0f;
		this->time = 240;
		//this->time = 20;
	}
	else if (value == 3) {
		this->level = value;
		this->demage = 3;
		this->scale.x = 1700.0f;
		this->scale.y = 1700.0f;
		this->scale.z = 1700.0f;
		this->time = 180;
		//this->time = 20;
	}
	else if (value == 2) {
		this->level = value;
		this->demage = 4;
		this->scale.x = 800.0f;
		this->scale.y = 800.0f;
		this->scale.z = 800.0f;
		this->time = 120;
		//this->time = 20;
	}
	else if (value == 1) {
		this->level = value;
		this->demage = 5;
		this->scale.x = 300.0f;
		this->scale.y = 300.0f;
		this->scale.z = 300.0f;
		this->time = 60;
		//this->time = 20;
	}
	else if (value == 0) {
		this->level = value;
		this->demage = 7;
		this->scale.x = 50.0f;
		this->scale.y = 50.0f;
		this->scale.z = 50.0f;
		this->time = 15;
	}
}

void Game_DangerLine::set_scale(int value)
{
	this->now_scale.x -= value;
	this->now_scale.y -= value;
	this->now_scale.z -= value;
}

Game_DangerLine::Game_DangerLine()
{
	this->level = 5;
	this->demage = 1;
	this->position.x = 1000.0f;
	this->position.y = 30.0f;
	this->position.z = 1500.0f;
	this->scale.x = 4500.0f;
	this->scale.y = 4500.0f;
	this->scale.z = 4500.0f;
	this->now_scale.x = 4500.0f;
	this->now_scale.y = 4500.0f;
	this->now_scale.z = 4500.0f;
	this->time = 300;
}

Game_DangerLine::~Game_DangerLine()
{
}
