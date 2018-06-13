#include "Game_Client.h"

void Game_Client::init()
{
	this->client_id = -1;
	this->room_id = -1;
	this->position.x = 0;
	this->position.y = 0;
	this->position.z = 0;
	this->rotation.x = 0;
	this->rotation.y = 0;
	this->rotation.z = 0;
	this->hp = Player_HP;
	this->armour = 0;
	this->animator = 0;
	this->connect = false;
	this->weaponState = 0;
	this->remove_client = false;
	this->dangerLineIn = true;
	ZeroMemory(&this->recv_over, sizeof(this->recv_over));
	this->recv_over.event_type = OP_RECV;
	this->recv_over.wsabuf.buf = reinterpret_cast<CHAR *>(this->recv_over.IOCP_buf);
	this->recv_over.wsabuf.len = sizeof(this->recv_over.IOCP_buf);
	this->prev_packet_data = 0;
	this->curr_packet_size = 0;
}

xyz Game_Client::get_position()
{
	return this->position;
}

xyz Game_Client::get_rotation()
{
	return this->rotation;
}

xyz Game_Client::get_car_rotation()
{
	return xyz(this->car_rotation.x, this->car_rotation.y, this->car_rotation.z);
}

Game_Client::Game_Client(const SOCKET sock, const int client_id, const char * game_id , const int room_id)
{
	this->connect = true;
	this->remove_client = false;
	this->client_socket = sock;
	this->client_id = client_id;
	this->room_id = room_id;
	this->nickName = game_id;
	/*strncpy(this->nickName, game_id, 10);*/
	this->position.x = 0;
	this->position.y = 0;
	this->position.z = 0;
	this->rotation.x = 0;
	this->rotation.y = 0;
	this->rotation.z = 0;
	this->car_rotation.x = 0;
	this->car_rotation.y = 0;
	this->car_rotation.z = 0;
	this->car_kmh = 0.0f;
	this->horizontal = 0.0f;
	this->vertical = 0.0f;
	this->inCar = -1;
	this->hp = Player_HP;
	this->armour = 0;
	this->animator = 0;
	this->weaponState = -1;
	this->dangerLineIn = true;
	this->client_die = false;
	this->playerStatus = 0;
	ZeroMemory(&this->recv_over, sizeof(this->recv_over));
	ZeroMemory(&this->packet_buf, sizeof(this->packet_buf));
	this->recv_over.event_type = OP_RECV;
	this->recv_over.room_id = this->room_id;
	this->recv_over.room_change = false;
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
	this->room_id = g_cl.room_id;
	this->nickName = g_cl.nickName;
	//strncpy(this->nickName, g_cl.nickName, 10);
	this->remove_client = g_cl.remove_client;
	this->position.x = g_cl.position.x;
	this->position.y = g_cl.position.y;
	this->position.z = g_cl.position.z;
	this->rotation.x = g_cl.rotation.x;
	this->rotation.y = g_cl.rotation.y;
	this->rotation.z = g_cl.rotation.z;
	this->car_rotation.x = g_cl.car_rotation.x;
	this->car_rotation.y = g_cl.car_rotation.y;
	this->car_rotation.z = g_cl.car_rotation.z;
	this->horizontal = g_cl.horizontal;
	this->vertical = g_cl.vertical;
	this->inCar = g_cl.inCar;
	this->hp = g_cl.hp;
	this->armour = g_cl.armour;
	this->animator = g_cl.animator;
	this->weaponState = g_cl.weaponState;
	this->dangerLineIn = g_cl.dangerLineIn;
	this->playerStatus = g_cl.playerStatus;
	this->client_die = g_cl.client_die;
	ZeroMemory(&this->recv_over, sizeof(this->recv_over));
	ZeroMemory(&this->packet_buf, sizeof(this->packet_buf));
	this->recv_over.event_type = OP_RECV;
	this->recv_over.room_id = g_cl.recv_over.room_id;
	this->recv_over.target_id = g_cl.recv_over.target_id;
	this->recv_over.room_change = g_cl.recv_over.room_change;
	this->recv_over.wsabuf.buf = reinterpret_cast<CHAR *>(this->recv_over.IOCP_buf);
	this->recv_over.wsabuf.len = sizeof(this->recv_over.IOCP_buf);
	this->prev_packet_data = 0;
	this->curr_packet_size = 0;

}

Game_Client::~Game_Client()
{
}