#include "Game_Client.h"

void Game_Client::init()
{
	this->client_id = -1;
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
	this->remove_client = false;
	ZeroMemory(&this->recv_over, sizeof(this->recv_over));
	this->recv_over.event_type = OP_RECV;
	this->recv_over.wsabuf.buf = reinterpret_cast<CHAR *>(this->recv_over.IOCP_buf);
	this->recv_over.wsabuf.len = sizeof(this->recv_over.IOCP_buf);
	this->prev_packet_data = 0;
	this->curr_packet_size = 0;
}

Game_Client::Game_Client(const SOCKET sock, const int client_id, const char * game_id)
{
	this->connect = true;
	this->remove_client = false;
	this->client_socket = sock;
	this->client_id = client_id;
	strncpy(this->nickName, game_id, 10);
	this->position.x = 0;
	this->position.y = 0;
	this->position.z = 0;
	this->rotation.x = 0;
	this->rotation.y = 0;
	this->rotation.z = 0;
	this->hp = 100;
	this->animator = 0;
	this->shotting = false;
	ZeroMemory(&this->recv_over, sizeof(this->recv_over));
	ZeroMemory(&this->packet_buf, sizeof(this->packet_buf));
	this->recv_over.event_type = OP_RECV;
	this->recv_over.wsabuf.buf = reinterpret_cast<CHAR *>(this->recv_over.IOCP_buf);
	this->recv_over.wsabuf.len = sizeof(this->recv_over.IOCP_buf);
	this->prev_packet_data = 0;
	this->curr_packet_size = 0;
}

Game_Client::Game_Client(const Game_Client & g_cl)
{
	this->connect = true;
	this->client_socket = g_cl.client_socket;
	this->client_id = g_cl.client_id;
	strncpy(this->nickName, g_cl.nickName, 10);
	this->remove_client = g_cl.remove_client;
	this->position.x = g_cl.position.x;
	this->position.y = g_cl.position.y;
	this->position.z = g_cl.position.z;
	this->rotation.x = g_cl.rotation.x;
	this->rotation.y = g_cl.rotation.y;
	this->rotation.z = g_cl.rotation.z;
	this->hp = g_cl.hp;
	this->animator = g_cl.animator;
	this->shotting = g_cl.shotting;
	ZeroMemory(&this->recv_over, sizeof(this->recv_over));
	ZeroMemory(&this->packet_buf, sizeof(this->packet_buf));
	this->recv_over.event_type = OP_RECV;
	this->recv_over.wsabuf.buf = reinterpret_cast<CHAR *>(this->recv_over.IOCP_buf);
	this->recv_over.wsabuf.len = sizeof(this->recv_over.IOCP_buf);
	this->prev_packet_data = 0;
	this->curr_packet_size = 0;

}

Game_Client::~Game_Client()
{
}
