#include "Game_Client.h"

void Game_Client::init()
{
	this->position.x = 0;
	this->position.y = 0;
	this->position.z = 0;
	this->rotation.x = 0;
	this->rotation.y = 0;
	this->rotation.z = 0;
	this->hp = 100;
	this->animator = 0;
	this->connect = false;
	this->shotting = false;
	ZeroMemory(&this->recv_over, sizeof(this->recv_over));
	this->recv_over.event_type = OP_RECV;
	this->recv_over.wsabuf.buf = reinterpret_cast<CHAR *>(this->recv_over.IOCP_buf);
	this->recv_over.wsabuf.len = sizeof(this->recv_over.IOCP_buf);
	this->prev_packet_data = 0;
	this->curr_packet_size = 0;
}

void Game_Client::set_Client(SOCKET sock, char * game_id)
{
	this->connect = true;
	this->client_socket = sock;
	strncpy(this->nickName, game_id, 10);
}

Game_Client::Game_Client()
{
}

Game_Client::~Game_Client()
{
}
