#ifndef __PROTOCOL_H__
#define __PROTOCOL_H__

enum TimerType { T_DangerLine, T_LobbyReday };
enum OPTYPE {
	OP_SEND, OP_RECV, OP_DangerLine, OP_RemoveClient,
	OP_MoveDangerLine, OP_DangerLineDamage, OP_LobbyWait, OP_LobbyReday,
	OP_StartCarWait
};
enum Event_Type {
	E_DangerLine, E_RoundTime, E_Remove_Client, E_MoveDangerLine,
	E_DangerLineDamage, E_LobbyWait, E_LobbyReday,
	E_StartCarWait
};

// 소켓 설정
#define SERVERPORT 9000
#define BUFSIZE    1024
#define MAX_BUFF_SIZE   4000
#define MAX_PACKET_SIZE  4000
#define MAX_Client 50

// 게임 설정
#define DebugMod TRUE
#define GameRoomLimit 2		// 최대 방 개수
#define Total_Max_Client MAX_Client * GameRoomLimit		// 모든 방에 최대 들어 올 수 있는 인원
#define Minimum_Players 2		// 최소 레디 인원
#define GamePlayWait 5			// 게임 시작전 대기 시간
#define noPlayer -1				// 플레이어 번호가 필요 없을 경우

// 서버에서 클라이언트에게 보내는 패킷
#define SC_ID           1					// 클라이언트 아이디를 보낸다.
#define SC_PUT_PLAYER    2			// 클라이언트 추가
#define SC_REMOVE_PLAYER 3		// 클라이언트 삭제
#define SC_Client_Data	4				// 클라이언트 모든 데이터
#define SC_Server_Time	5				// 서버 타이머
#define SC_Server_Item	6				// 서버 아이템
#define SC_Shot_Client	7				// 클라이언트 Shot 정보
#define SC_DangerLine	8				// 클라이언트 DangerLine 정보 전송
#define SC_Zombie_Info 9				// 클라이언트에게 좀비 위치를 전달해 준다.
#define SC_Remove_Zombie 10		// 좀비 삭제
#define SC_Lobby_Time 11				// 클라이언트 에게 로비 대기시간을 보내준다.
#define SC_StartCar_Play 12			// 클라이언트 에게 시작 차량을 움직이라고 보내준다.


// 클라이언트가 서버에게 보내는 패킷
#define CS_Info           1					// 클라이언트가 서버에게 자신의 위치정보를 보내준다.
#define CS_Shot_info    2					// 클라이언트가 서버에게 Shot 정보를 보내준다.
#define CS_Check_info  3					// 클라이언트가 서버에게 자신의 정보가 맞는지 확인해 준다.
#define CS_Eat_Item	   4					// 클라이언트가 서버에게 먹은 아이템 정보를 보내준다.
#define CS_Zombie_info 5					// 클라이언트가 서버에게 좀비 데이터를 전달해 준다.
#define CS_Object_HP 6						// 클라이언트가 서버에게 HP 데이터를 전달해 준다.
#define CS_Car_Riding 7						// 클라이언트가 서버에게 차량에 탑승했다고 전달해 준다.
#define CS_Car_Rode 8						// 클라이언트가 서버에게 차량에 하차했다고 전달해 준다.
#define CS_Player_Status 9					// 클라이언트가 서버에게 자신의 상태를 전달한다.

// 자기장 시간
#define DangerLine_init 10
#define DangerLine_Level4 240
#define DangerLine_Level3 240
#define DangerLine_Level2 240
#define DangerLine_Level1 240
#define DangerLine_Level0 240

// 플레이어 볼 수 있는 거리
#define Player_Dist 500

// 좀비가 볼 수 있는 거리
#define Zombie_Dist 200
//#define Limit_Zombie 15
#define Create_Zombie 50

// 아이템, 플레이어 종류
#define Kind_Item 0
#define Kind_Car 1
#define Kind_Player 2
#define Kind_Zombie 3

// 체력 설정
#define Car_HP 200
#define Player_HP 100000
#define Zombie_HP 100

// 게임 상태 설정
#define LoginStatus 0
#define LobbyStatus 1
#define ReadyStatus 2
#define inGameStatus 3
#define playGameStatus 4

#endif