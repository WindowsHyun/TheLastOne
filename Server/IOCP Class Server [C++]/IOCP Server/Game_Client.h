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

struct xyz {
	float x;
	float y;
	float z;
};

class Game_Client {
private:
	int client_id = -1;
	xyz position;
	xyz rotation;
	int hp = -1;
	int animator = 0;
	bool connect;
	bool remove_client;
	bool shotting = false;

	SOCKET client_socket;
	int prev_packet_data; // 이전 처리되지 않는 패킷이 얼마냐
	int curr_packet_size; // 지금 처리하고 있는 패킷이 얼마냐
	std::unordered_set<int> view_list; //걍 set보다 훨씬빠르다!
	std::mutex vl_lock;

public:
	char nickName[10];																							// 클라이언트 닉네임
	OverlappedEx recv_over;
	unsigned char packet_buf[MAX_PACKET_SIZE];

	void init();

	int get_client_id() { return this->client_id; }																// 클라이언트 아이디 전달
	int get_hp() { return this->hp; };																			// 클라이언트 체력 전달
	int get_animator() { return this->animator; };															// 클라이언트 애니메이션 전달
	int get_shotting() { return this->shotting; };																// 클라이언트 Shot 전달
	bool get_Connect() { return this->connect; };															// 클라이언트 연결 여부 전달
	bool get_Remove() { return this->remove_client; };													// 클라이언트 삭제 여부 전달
	int get_curr_packet() { return this->curr_packet_size; };												// 클라이언트 패킷 사이즈 전달
	int get_prev_packet() { return this->prev_packet_data; };												// 클라이언트 패킷 사이즈 전달
	Vec3 get_position() { return Vec3(this->position.x, this->position.y, this->position.z); };		// 클라이언트 포지션 전달
	Vec3 get_rotation() { return Vec3(this->rotation.x, this->rotation.y, this->rotation.z); };		// 클라이언트 로테이션 전달
	SOCKET get_Socket() { return this->client_socket; };													// 클라이언트 소켓 전달
	OverlappedEx get_over() { return this->recv_over; };													// Overlapped 구조체 전달




	void set_prev_packet(const int size) { this->prev_packet_data = size; };				// 클라이언트 패킷 사이즈 저장
	void set_curr_packet(const int size) { this->curr_packet_size = size; };				// 클라이언트 패킷 사이즈 저장
	void set_client_position(const xyz position) { this->position = position; };			// 클라이언트 포지션 저장
	void set_client_rotation(const xyz rotation) { this->rotation = rotation; };			// 클라이언트 로테이션 저장
	void set_client_animator(const int value) { this->animator = value; };					// 클라이언트 애니메이션 저장
	void set_client_shotting(const bool value) { this->shotting = value; };				// 클라이언트 Shot 저장
	void set_client_Connect(const bool value) { this->connect = value; };				// 클라이언트 Connect 저장
	void set_client_Remove(const bool value) { this->remove_client = value; };				// 클라이언트 Remove 저장

	Game_Client(const SOCKET sock, const int client_id, const char * game_id);
	Game_Client(const Game_Client& g_cl);
	~Game_Client();
};


#endif