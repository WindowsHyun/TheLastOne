#ifndef __COREHEADER_H__
#define __COREHEADER_H__
#pragma comment(lib, "ws2_32")
#include <unordered_map>
#include <unordered_set> // 성능이 더 좋아진다. [순서가 상관없을경우]
#include <string>
#include <winsock2.h>
#include <random>
#include <iostream>
#include <thread>
#include <mutex>

#include "xyz.h"
#include "Protocol.h"

struct OverlappedEx {
	WSAOVERLAPPED over;
	WSABUF wsabuf;
	unsigned char IOCP_buf[MAX_BUFF_SIZE];
	OPTYPE event_type;
	int target_id;
	int room_id;
	bool room_change = false;
};
#endif