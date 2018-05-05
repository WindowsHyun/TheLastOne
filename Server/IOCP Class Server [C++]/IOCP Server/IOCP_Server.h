#ifndef __IOCPSERVER_H__
#define __IOCPSERVER_H__

#define _WINSOCK_DEPRECATED_NO_WARNINGS
#define _CRT_SECURE_NO_WARNINGS						// scanf 빌드 오류
#define _CRT_NONSTDC_NO_DEPRECATE					// itoa 빌드 오류
/* flatbuffers 에서의 min, max 오류 해결 방법 */
#define _WIN32_WINNT _WIN32_WINNT_XP
#define WIN32_LEAN_AND_MEAN
#define NOMINMAX
/* http://bspfp.pe.kr/archives/591 */

#pragma comment(lib, "ws2_32")
#include <winsock2.h>
#include <iostream>
#include <vector>
#include <queue>
#include <random>
#include <windows.h>
#include <string>
#include <cmath>


#include "Protocol.h"
#include "Flatbuffers_View.h"
#include "Room_Manager.h"
#include "Timer.h"


using namespace Game::TheLastOne; // Flatbuffers를 읽어오자.

class IOCP_Server {
private:
	SOCKET g_socket;
	std::chrono::high_resolution_clock::time_point serverTimer;
	HANDLE g_hiocp;
	std::vector<Room_Manager> GameRoom;		// 게임 룸을 Vector로 선언
	std::queue<int> remove_client_id;					// 클라이언트 아이디를	Queue에 넣는다.
	//std::unordered_map< int, int> ci_room;			// 클라이언트 아이디와 룸 정보를 가지고 있는다. [아이디, 룸정보]
	Server_Timer Timer;									// 서버 타이머

	void initServer();
	void err_quit(char *msg);							// Error 나올 경우 종료
	void err_display(char *msg, int err_no);		// Error 표시 해주기
	void makeThread();								// 스레드 만들기
	void Worker_Thread();							// 실제 동작 스레드
	void Accept_Thread();								// 클라이언트 받는 스레드
	void Remove_Client();								// 클라이언트가 종료시 데이터 지우기
	void Shutdown_Server();							// 서버 종료
	void DisconnectClient(const int room_id, const int ci);					// 클라이언트 종료
	void ProcessPacket(const int room_id, const int ci, const char *packet);		// 패킷 처리
	void SendPacket(const int type, const int room_id, const int ci, const void *packet, const int psize);		// 패킷 보내기
	void Send_Client_ID(const int room_id, const int client_id, const int value, const bool allClient);	// 클라이언트 에게 패킷 아이디 보내기
	void Send_All_Player(const int room_id, const int client);					// 클라이언트에게 모든 클라이언트 위치 보내기
	void Send_Hide_Player(const int room_id, const int client);		// 클라이언트 범위를 벗어날 경우 지워준다.
	void Send_All_Zombie(const int room_id, const int client);							// 클라이언트에게 모든 좀비 위치 보내기
	void Send_Hide_Zombie(const int room_id, const int client);		// 클라이언트 범위를 벗어날 경우 지워준다.
	void Send_All_Item(const int room_id, const int ci);		// 클라이언트에게 시간정보를 보내준다.
	void Send_Client_Shot(const int room_id, const int shot_client);		// 클라이언트들에게 Shot 정보를 보내준다.
	void Send_All_Time(const int room_id, const int type, const int kind, const int time, const int client_id, const bool allClient);	 // 클라이언트에게 시간을 보내준다.
	//void Send_DangerLine_info(int demage, xyz pos, xyz scale);
	//void Attack_DangerLine_Damge();			// 자기장 데미지를 플레이어에게 준다.

	bool Distance(const int room_id, const int me, const int  you, const int Radius, const int kind);
public:
	HANDLE getHandle() { return g_hiocp; }
	IOCP_Server();
	~IOCP_Server();
};


#endif