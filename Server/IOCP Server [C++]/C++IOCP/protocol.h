#ifndef __PROTOCOL_H__
#define __PROTOCOL_H__

// 서버에서 클라이언트에게 보내는 패킷
#define SC_ID           1					// 클라이언트 아이디를 보낸다.
#define SC_PUT_PLAYER    2			// 클라이언트 추가
#define SC_REMOVE_PLAYER 3		// 클라이언트 삭제
#define SC_Client_Data	4				// 클라이언트 모든 데이터

// 클라이언트가 서버에게 보내는 패킷
#define CS_Info           1					// 클라이언트가 서버에게 자신의 위치정보를 보내준다.
#define CS_Shot_info    2					// 클라이언트가 서버에게 Shot 정보를 보내준다.


// BYTE를 255까지만 인식하여 int형인 BOOL로 수정

struct sc_packet_put_player {
	BYTE size;
	BYTE type;
	WORD id;
	BOOL x;
	BOOL y;
	BOOL direction;
	BOOL movement;
};

struct sc_packet_pos {
	BYTE size;
	BYTE type;
	WORD id;
	BOOL x;
	BOOL y;
	BOOL direction;
	BOOL movement;
};

struct sc_packet_remove_player {
	BYTE size;
	BYTE type;
	WORD id;
};

struct cs_packet_chat {
	BYTE size;
	BYTE type;
	WCHAR message[MAX_STR_SIZE];
};

struct sc_packet_chat {
	BYTE size;
	BYTE type;
	WORD id;
	WCHAR message[MAX_STR_SIZE];
};

struct cs_packet_Move {
	BYTE size;
	BYTE type;
	BYTE x;
	BYTE y;
};

#endif