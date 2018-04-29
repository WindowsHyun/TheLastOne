#ifndef __GAMECLIENT_H__
#define __GAMECLIENT_H__

#include "IOCP_Server.h"

struct OverlappedEx {
	WSAOVERLAPPED over;
	WSABUF wsabuf;
	unsigned char IOCP_buf[MAX_BUFF_SIZE];
	OPTYPE event_type;
	int target_id;
};

class Game_Client {
private:
	int client_id = -1;
	xyz position;
	xyz rotation;
	xyz car_rotation;
	int hp = -1;
	int armour = 0;
	int animator = 0;
	int weaponState = 0;
	int limit_Zombie = 0;
	int inCar = -1;
	float horizontal = 0.0f;
	float vertical = 0.0f;
	bool connect;
	bool remove_client;
	SOCKET client_socket;
	int prev_packet_data; // 이전 처리되지 않는 패킷이 얼마냐
	int curr_packet_size; // 지금 처리하고 있는 패킷이 얼마냐
	

public:
	char nickName[10];																							// 클라이언트 닉네임
	OverlappedEx recv_over;
	unsigned char packet_buf[MAX_PACKET_SIZE];

	void init();
	int get_inCar() { return this->inCar; }								// 클라이언트 무슨 차량 탑승 값 전달
	float get_vertical() { return (float)this->vertical; }				// 클라이언트 애니메이션 값 전달
	float get_horizontal() { return (float)this->horizontal; }		// 클라이언트 애니메이션 값 전달
	int get_limit_zombie() { return this->limit_Zombie; }			// 클라이언트 좀비 최대치 전달
	int get_client_id() { return this->client_id; }						// 클라이언트 아이디 전달
	int get_hp() { return this->hp; };									// 클라이언트 체력 전달
	int get_armour() { return this->armour; };						// 클라이언트 아머 전달
	int get_animator() { return this->animator; };					// 클라이언트 애니메이션 전달
	int get_weapon() { return this->weaponState; };				// 클라이언트 WeaponState 전달
	bool get_Connect() { return this->connect; };					// 클라이언트 연결 여부 전달
	bool get_Remove() { return this->remove_client; };			// 클라이언트 삭제 여부 전달
	int get_curr_packet() { return this->curr_packet_size; };		// 클라이언트 패킷 사이즈 전달
	int get_prev_packet() { return this->prev_packet_data; };		// 클라이언트 패킷 사이즈 전달
	xyz get_pos() { return this->position; };
	Vec3 get_position();													// 클라이언트 포지션 전달
	Vec3 get_rotation();													// 클라이언트 로테이션 전달
	Vec3 get_car_rotation();												// 클라이언트 로테이션 전달
	SOCKET get_Socket() { return this->client_socket; };			// 클라이언트 소켓 전달
	OverlappedEx get_over() { return this->recv_over; };			// Overlapped 구조체 전달

	void set_prev_packet(const int size) { this->prev_packet_data = size; };				// 클라이언트 패킷 사이즈 저장
	void set_curr_packet(const int size) { this->curr_packet_size = size; };				// 클라이언트 패킷 사이즈 저장
	void set_client_position(const xyz position) { this->position = position; };			// 클라이언트 포지션 저장
	void set_client_rotation(const xyz rotation) { this->rotation = rotation; };			// 클라이언트 로테이션 저장
	void set_client_car_rotation(const xyz rotation) { this->car_rotation = rotation; };			// 클라이언트 차량 로테이션 저장
	void set_client_animator(const int value) { this->animator = value; };					// 클라이언트 애니메이션 저장
	void set_client_weapon(const int value) { this->weaponState = value; };				// 클라이언트 WeaponState 저장
	void set_client_Connect(const bool value) { this->connect = value; };				// 클라이언트 Connect 저장
	void set_client_Remove(const bool value) { this->remove_client = value; };				// 클라이언트 Remove 저장
	void set_limit_zombie(const int value) { this->limit_Zombie += value; }				// 클라이언트 좀비 최대치 저장
	void set_vertical(float value) { this->vertical = (float)value; }
	void set_horizontal(float value) {  this->horizontal = (float)value; }
	void set_inCar(int value) { this->inCar = value; }												// 클라이언트 무슨 차량 탑승 값 저장
	void set_hp(int value) { this->hp = value; };														// 클라이언트 체력 저장
	void set_armour(int value) { this->armour = value; };											// 클라이언트 아머 저장

	Game_Client(const SOCKET sock, const int client_id, const char * game_id);
	Game_Client(const Game_Client& g_cl);
	~Game_Client();
};

bool Distance(int me, int  you, int Radius, int kind);

#endif