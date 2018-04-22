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
#include <unordered_map>
#include <queue>
#include <thread>
#include <random>
#include <windows.h>
#include <unordered_set> // 성능이 더 좋아진다. [순서가 상관없을경우]
#include <mutex>
#include <string>

#include "Protocol.h"
#include "Flatbuffers_View.h"
#include "Game_Item.h"

using namespace std::chrono;
using namespace Game::TheLastOne; // Flatbuffers를 읽어오자.

struct xyz {
	float x;
	float y;
	float z;
};

class IOCP_Server {
private:
	SOCKET g_socket;
	std::chrono::high_resolution_clock::time_point serverTimer;
	HANDLE g_hiocp;

	void initServer();
	void err_quit(char *msg);							// Error 나올 경우 종료
	void err_display(char *msg, int err_no);		// Error 표시 해주기
	void makeThread();								// 스레드 만들기
	void Worker_Thread();							// 실제 동작 스레드
	void Accept_Thread();								// 클라이언트 받는 스레드
	void Zombie_Thread();							// 좀비 스레드
	void Remove_Client();							// 클라이언트가 종료시 지우는 스레드
	void Shutdown_Server();							// 서버 종료
	void DisconnectClient(int ci);					// 클라이언트 종료
	void ProcessPacket(int ci, char *packet);		// 패킷 처리
	void SendPacket(int type, int cl, void *packet, int psize);		// 패킷 보내기
	void Send_Client_ID(int client_id, int value, bool allClient);	// 클라이언트 에게 패킷 아이디 보내기
	void Send_All_Data(int client, bool allClient);					// 클라이언트에게 모든 클라이언트 위치 보내기
	void Send_All_Time(int kind, int time, int client_id, bool allClient);					// 클라이언트에게 시간을 보내준다.
	void Send_All_Item();		// 클라이언트에게 시간정보를 보내준다.
	void Send_Client_Shot(int shot_client);		// 클라이언트들에게 Shot 정보를 보내준다.
	void Send_DangerLine_info(int demage, xyz pos, xyz scale);

public:
	HANDLE getHandle() { return g_hiocp; }
	IOCP_Server();
	~IOCP_Server();
};


#endif