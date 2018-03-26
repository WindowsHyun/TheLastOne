#ifndef __PROTOCOL_H__
#define __PROTOCOL_H__

enum TimerType { T_InitTime };

// 서버에서 클라이언트에게 보내는 패킷
#define SC_ID           1					// 클라이언트 아이디를 보낸다.
#define SC_PUT_PLAYER    2			// 클라이언트 추가
#define SC_REMOVE_PLAYER 3		// 클라이언트 삭제
#define SC_Client_Data	4				// 클라이언트 모든 데이터
#define SC_Server_Time	5				// 서버 타이머

// 클라이언트가 서버에게 보내는 패킷
#define CS_Info           1					// 클라이언트가 서버에게 자신의 위치정보를 보내준다.
#define CS_Shot_info    2					// 클라이언트가 서버에게 Shot 정보를 보내준다.
#define CS_Check_info  3					// 클라이언트가 서버에게 자신의 정보가 맞는지 확인해 준다.

#endif