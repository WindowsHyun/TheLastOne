#ifndef __GAMETIMER_H__
#define __GAMETIMER_H__

#include "IOCP_Server.h"
#include "Game_Client.h"

#include <chrono>
#include <queue>

enum Event_Type { E_initTime, E_RoundTime };

struct Timer_Event {
	int object_id;	// 특정 대상 혹은 시간을 넣는다.
	high_resolution_clock::time_point exec_time; // 이 이벤트가 언제 실행되야 하는가
	Event_Type event; // 여러 이벤트가 있다.
};

class comparison {
	bool reverse;
public:
	comparison() {}
	bool operator() (const Timer_Event first, const Timer_Event second) const {
		return first.exec_time > second.exec_time;
	}
};

class Server_Timer {
private:
	HANDLE g_hiocp;
	std::thread timer_tread;
	std::mutex tq_lock;
	std::chrono::high_resolution_clock::time_point serverTimer;
	std::priority_queue <Timer_Event, std::vector<Timer_Event>, comparison> timer_queue;
	void Timer_Thread();

public:
	void setTimerEvent(Timer_Event t);
	void initTimer(HANDLE g_hiocp);
	

	Server_Timer();
	~Server_Timer();
};


#endif