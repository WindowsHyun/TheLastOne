#ifndef __SERVER_H__
#define __SERVER_H__

#define _WINSOCK_DEPRECATED_NO_WARNINGS
#define _CRT_SECURE_NO_WARNINGS

/* flatbuffers 에서의 min, max 오류 해결 방법 */
#define _WIN32_WINNT _WIN32_WINNT_XP
#define WIN32_LEAN_AND_MEAN
#define NOMINMAX
/* http://bspfp.pe.kr/archives/591 */

#pragma comment(lib, "ws2_32")

#include <winsock2.h>
#include <stdio.h>
#include <iostream>
#include <vector>
#include <thread>
#include <mutex>
#include <queue>
#include <unordered_set> // 성능이 더 좋아진다. [순서가 상관없을경우]
#include <random>
#include <chrono>
#include <windows.h>


using namespace std::chrono;

//---------------------------------------------------------------------------------------------
// 소켓 설정
#define SERVERPORT 9000
#define BUFSIZE    1024
#define MAX_BUFF_SIZE   4000
#define MAX_PACKET_SIZE  255
#define MAX_Client 999
//---------------------------------------------------------------------------------------------
// 게임 설정
#define DebugMod FALSE
#define Game_Width 300														// 가로
#define Game_Height 300														// 세로
#define VIEW_RADIUS   15														// Viwe 거리
#define NPC_RADIUS   10														// Viwe 거리
#define NPC_START  1000
#define MAX_NPC 1790
#define MAX_STR_SIZE  100
//---------------------------------------------------------------------------------------------

void err_quit( char *msg );
void err_display( char *msg, int err_no );

void init();
void Worker_Thread();
void Accept_Thread();
void Shutdown_Server();
void SendPutPlayerPacket( int client, int object );
void SendPacket( int cl, void *packet );
void DisconnectClient( int ci );
void ProcessPacket( int ci, char *packet );
void SendPositionPacket( int client, int object );
void SendRemovePlayerPacket( int client, int object );
void Send_Flat_Packet( int client, int object );


enum OPTYPE { OP_SEND, OP_RECV, OP_DO_AI, E_PLAYER_MOVE_NOTIFY, OP_Attack_Move, OP_Responder };
enum Event_Type { E_MOVE, E_Attack_Move, E_Responder };

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

struct CLIENT {
	xyz client_xyz;
	xyz view;
	char game_id[10]; // 클라에서 받아온 게임아이디를 저장
	int hp;
	bool connect;

	SOCKET client_socket;
	OverlappedEx recv_over;
	unsigned char packet_buf[MAX_PACKET_SIZE];
	int prev_packet_data; // 이전 처리되지 않는 패킷이 얼마냐
	int curr_packet_size; // 지금 처리하고 있는 패킷이 얼마냐
	std::unordered_set<int> view_list; //걍 set보다 훨씬빠르다!
	std::mutex vl_lock;
};

extern CLIENT g_clients[MAX_NPC];


#endif