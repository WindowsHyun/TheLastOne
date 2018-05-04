#include "IOCP_Server.h"
#include "Game_Client.h"
#include "Timer.h"
#include "Game_DangerLine.h"
#include "Game_CollisionCheck.h"
#include "Game_Zombie.h"

std::unordered_map< int, Game_Client>::iterator get_client_iter(int ci);
std::unordered_map< int, Game_Client> g_clients;
std::unordered_map< int, Game_Item>::iterator get_item_iter(int ci);
std::unordered_map< int, Game_Item> g_item;
std::unordered_map< int, Game_Zombie>::iterator get_zombie_iter(int ci);
std::unordered_map< int, Game_Zombie> g_zombie;
void player_To_Zombie(std::unordered_map<int, Game_Zombie> &zombie, std::unordered_map<int, Game_Client> &player);

//std::unordered_map< int, Game_CollisionCheck> g_collision;
std::queue<int> remove_client_id;
Game_DangerLine DangerLine;
Server_Timer Timer;

IOCP_Server::IOCP_Server()
{
	std::wcout.imbue(std::locale("korean"));
	initServer();
	makeThread();
}

IOCP_Server::~IOCP_Server()
{
	Shutdown_Server();
}

void IOCP_Server::initServer()
{
	serverTimer = high_resolution_clock::now();
	WSADATA	wsadata;
	WSAStartup(MAKEWORD(2, 2), &wsadata);
	g_hiocp = CreateIoCompletionPort(INVALID_HANDLE_VALUE, 0, NULL, 0);
	g_socket = WSASocket(AF_INET, SOCK_STREAM, IPPROTO_TCP, NULL, 0, WSA_FLAG_OVERLAPPED);

	SOCKADDR_IN ServerAddr;
	ZeroMemory(&ServerAddr, sizeof(SOCKADDR_IN));
	ServerAddr.sin_family = AF_INET;
	ServerAddr.sin_port = htons(SERVERPORT);
	ServerAddr.sin_addr.s_addr = INADDR_ANY;

	bind(g_socket, reinterpret_cast<sockaddr *>(&ServerAddr), sizeof(ServerAddr));
	listen(g_socket, 5);

	int option = TRUE;
	setsockopt(g_socket, IPPROTO_TCP, TCP_NODELAY, (const char*)&option, sizeof(option));	// 네이글 알고리즘 OFF


	for (int i = 0; i < MAX_Client; ++i) {
		// 클라이언트 고유번호 넣어 주기.
		remove_client_id.push(i);
	}
	// 게임 아이템 정보 g_item에 넣어주기.
	load_item_txt("./Game_Item_Collection.txt", &g_item);

	// 좀비 캐릭터 생성하기
	init_Zombie(Create_Zombie, &g_zombie);

	// 충돌체크 넣기
	//load_CollisionCheck_txt("./Game_CollisionCheck.txt", &g_collision);

	std::cout << "init Complete..!" << std::endl;
}

void IOCP_Server::err_quit(char * msg)
{
	LPVOID lpMsgBuf;
	FormatMessage(
		FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM,
		NULL, WSAGetLastError(),
		MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
		(LPTSTR)&lpMsgBuf, 0, NULL);
	printf("Error : %s\n", msg);
	LocalFree(lpMsgBuf);
	exit(1);
}

void IOCP_Server::err_display(char * msg, int err_no)
{
	LPVOID lpMsgBuf;
	FormatMessage(
		FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM,
		NULL, err_no,
		MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
		(LPTSTR)&lpMsgBuf, 0, NULL);
	std::cout << "[" << msg << "]" << (char *)lpMsgBuf << std::endl;
	LocalFree(lpMsgBuf);
}

void IOCP_Server::makeThread()
{
	std::vector <std::thread * > worker_threads;
	for (int i = 0; i < 6; ++i) {
		worker_threads.emplace_back(new std::thread{ &IOCP_Server::Worker_Thread, this });
	}

	std::thread accept_tread{ &IOCP_Server::Accept_Thread, this };

	//std::thread zombie_thread{ &IOCP_Server::Zombie_Thread, this };

	Timer.initTimer(g_hiocp);		// 타이머 스레드를 만들어 준다.

	accept_tread.join();
	//zombie_thread.join();
	for (auto pth : worker_threads) {
		pth->join();
		delete pth;
	}

	worker_threads.clear();
}

void IOCP_Server::Worker_Thread()
{
	while (true) {
		DWORD io_size;
		unsigned long long ci;
		OverlappedEx *over;
		BOOL ret = GetQueuedCompletionStatus(g_hiocp, &io_size, &ci, reinterpret_cast<LPWSAOVERLAPPED *>(&over), INFINITE);

		auto client = get_client_iter((int)ci);

		if (FALSE == ret) {
			int err_no = WSAGetLastError();
			if (err_no == 64)
				DisconnectClient((int)ci);
			else
				err_display((char *)"QOCS : ", WSAGetLastError());
		}

		if (0 == io_size) {
			DisconnectClient((int)ci);
			continue;
		}

		if (OP_RECV == over->event_type) {
#if (DebugMod == TRUE )
			//std::cout << "RECV from Client : " << ci << std::endl;
			//std::cout << "IO_SIZE : " << io_size << std::endl;
#endif
			unsigned char *buf = client->second.recv_over.IOCP_buf;

			unsigned psize = client->second.get_curr_packet();
			unsigned pr_size = client->second.get_prev_packet();
			char packet_data[8]{ 0 };
			while (io_size != 0) {
				if (0 == psize) {
					for (int i = 0; i < 8; ++i) {
						if (buf[i] != 124)
							packet_data[i] = buf[i];
						else
							break;
					}
					psize = atoi(packet_data) + 8;
				}
				// 0일 경우[바로전 패킷이 처리가 끝나고 새피킷으로 시작해도 된다. / 처음 받는다] 강제로 정해준다.
				if (io_size + pr_size >= psize) {
					// 지금 패킷 완성이 가능하다.
					char packet[MAX_PACKET_SIZE];
					memcpy(packet, client->second.packet_buf, pr_size);
					memcpy(packet + pr_size, buf, psize - pr_size);

					if (psize > 19)
						ProcessPacket(static_cast<int>(ci), packet);
					io_size -= psize - pr_size;
					buf += psize - pr_size;
					psize = 0; pr_size = 0;
				}
				else {
					memcpy(client->second.packet_buf + pr_size, buf, io_size);
					pr_size += io_size;
					io_size = 0;
				}
			}
			client->second.set_curr_packet(psize);
			client->second.set_prev_packet(pr_size);
			DWORD recv_flag = 0;
			WSARecv(client->second.get_Socket(), &client->second.recv_over.wsabuf, 1, NULL, &recv_flag, &client->second.recv_over.over, NULL);
		}
		else if (OP_SEND == over->event_type) {
			if (io_size != over->wsabuf.len) { // 같아야 되는데 다르므로 
#if (DebugMod == TRUE )
				std::cout << "Send Incomplete Error!" << std::endl;
#endif
				closesocket(client->second.get_Socket());
				client->second.init();
				exit(-1);
			}
			delete over;
		}
		else if (OP_DangerLine == over->event_type) {
			Send_All_Time(T_DangerLine, (int)ci, -1, true);
			if (ci <= 0) {
				// 초기 대기시간이 만료 되었을 경우
				DangerLine.set_level(DangerLine.get_level() - 1);
				Send_DangerLine_info(DangerLine.get_demage(), DangerLine.get_pos(), DangerLine.get_scale());
				if (DangerLine.get_level() != -1) {
					Timer_Event t = { (int)DangerLine.get_scale_x() , high_resolution_clock::now() + 100ms, E_MoveDangerLine };
					std::cout << DangerLine.get_scale_x() << std::endl;
					Timer.setTimerEvent(t);
				}

			}
			else {
				// ci-1 을 한 이유는 1초씩 내리기 위하여.
				Send_DangerLine_info(DangerLine.get_demage(), DangerLine.get_pos(), DangerLine.get_scale());
				Timer_Event t = { (int)ci - 1, high_resolution_clock::now() + 1s, E_DangerLine };
				Timer.setTimerEvent(t);
			}
		}
		else if (OP_MoveDangerLine == over->event_type) {
			DangerLine.set_scale(10);
			Send_DangerLine_info(DangerLine.get_demage(), DangerLine.get_pos(), DangerLine.get_scale());

			if (DangerLine.get_now_scale_x() <= ci) {
				Timer_Event t = { DangerLine.get_time() , high_resolution_clock::now() + 1s, E_DangerLine };	// 자기장 시작 전 대기시간
				Timer.setTimerEvent(t);
			}
			else {
				Timer_Event t = { (int)ci , high_resolution_clock::now() + 100ms, E_MoveDangerLine };
				Timer.setTimerEvent(t);
			}

		}
		else if (OP_RemoveClient == over->event_type) {
			// 1초마다 한번씩 종료된 클라이언트를 지워 준다.
			Remove_Client();
			Timer_Event t = { 1, high_resolution_clock::now() + 1s, E_Remove_Client };
			Timer.setTimerEvent(t);
		}
		else if (OP_DangerLineDamage == over->event_type) {
			// 자기장 데미지는 1초마다 한번씩 데미지를 준다.
			Attack_DangerLine_Damge();
			Timer_Event t = { 1, high_resolution_clock::now() + 1s, E_DangerLineDamage };
			Timer.setTimerEvent(t);
		}
		else {
#if (DebugMod == TRUE )
			std::cout << "Unknown GQCS event!" << std::endl;
#endif
			exit(-1);
		}


	}
}

void IOCP_Server::Accept_Thread()
{
	SOCKADDR_IN ClientAddr;
	ZeroMemory(&ClientAddr, sizeof(SOCKADDR_IN));
	ClientAddr.sin_family = AF_INET;
	ClientAddr.sin_port = htons(SERVERPORT);
	ClientAddr.sin_addr.s_addr = INADDR_ANY;
	int addr_size = sizeof(ClientAddr);
	while (true) {
		SOCKET new_client = WSAAccept(g_socket, reinterpret_cast<sockaddr *>(&ClientAddr), &addr_size, NULL, NULL);

		if (INVALID_SOCKET == new_client) {
			int err_no = WSAGetLastError();
			err_display((char *)"WSAAccept : ", err_no);
		}

		int new_id;

		// 스택에서 값을 가져온다.
		new_id = remove_client_id.front();
		remove_client_id.pop();
#if (DebugMod == TRUE )
		std::cout << "New Client : " << new_id << std::endl;
#endif

		if (g_clients.size() >= MAX_Client) {
#if (DebugMod == TRUE )
			std::cout << "설정된 인원 이상으로 접속하여 차단하였습니다..!" << std::endl;
#endif
			closesocket(new_client);
			continue;
		}
		//---------------------------------------------------------------------------------------------------------------------------------------------------
		// 들어온 접속 아이디 init 처리
		g_clients.insert(std::pair< int, Game_Client>(new_id, { new_client, new_id, (char *)"TheLastOne" }));	// Map에 넣어준다.
		Send_Client_ID(new_id, SC_ID, false);		// 클라이언트에게 자신의 아이디를 보내준다.
		//---------------------------------------------------------------------------------------------------------------------------------------------------
		// map에 들어간 클라이언트를 찾는다.
		auto client = get_client_iter(new_id);
		//---------------------------------------------------------------------------------------------------------------------------------------------------

		DWORD recv_flag = 0;
		CreateIoCompletionPort(reinterpret_cast<HANDLE>(new_client), g_hiocp, new_id, 0);

		int ret = WSARecv(new_client, &client->second.recv_over.wsabuf, 1, NULL, &recv_flag, &client->second.recv_over.over, NULL);

		if (0 != ret) {
			int error_no = WSAGetLastError();
			if (WSA_IO_PENDING != error_no) {
				err_display((char*)"RecvPacket:WSARecv", error_no);
				//while ( true );
			}
		}

	}	// while(true)
}

void IOCP_Server::Remove_Client()
{
	for (auto iter = g_clients.begin(); iter != g_clients.end();) {
		if (iter->second.get_Connect() == false && iter->second.get_Remove() == true) {
#if (DebugMod == TRUE )
			std::cout << "Disconnect Client : " << iter->second.get_client_id() << std::endl;
#endif
			Send_Client_ID(iter->second.get_client_id(), SC_REMOVE_PLAYER, true);
			closesocket(iter->second.get_Socket());
			remove_client_id.push(iter->second.get_client_id());	// 종료된 클라이언트 번호를 스택에 넣어준다.
			g_clients.erase(iter++);		// Map 에서 해당 클라이언트를 삭제한다.
		}
		else {
			++iter;
		}
	}
}

void IOCP_Server::Shutdown_Server()
{
	closesocket(g_socket);
	CloseHandle(g_hiocp);
	WSACleanup();
}

void IOCP_Server::DisconnectClient(int ci)
{
	// Disconnect시 Remove_Client에서 지울 수 있게 값을 변경 해준다.
	auto client = get_client_iter(ci);
	client->second.set_client_Connect(false);
	client->second.set_client_Remove(true);
}

void IOCP_Server::ProcessPacket(int ci, char * packet)
{
	int errnum = 0;

	// 패킷 사이즈를 찾아주자.
	char packet_data[8]{ 0 };
	int i = 0;
	for (i = 0; i < 8; ++i) {
		if (packet[i] != 124)
			packet_data[i] = packet[i];
		else
			break;
	}
	int packet_size = atoi(packet_data) + 8;


	//char get_packet[MAX_PACKET_SIZE];
	char *get_packet = new char[packet_size] {0};
	bool all_Client_Packet = false;	// 모든 클라이언트에게 보낼 때는 True
	for (int i = 8; i < packet_size; ++i)
		get_packet[i - 8] = packet[i];
	/*
	Packet 사이즈를 확인한 후 받아야 한다.
	그렇지 않으면 Packet 사이즈 초과를 하여 확인을 할 수가 없다.
	*/

	//memcpy(real_packet, packet, sizeof(packet));

	auto client = get_client_iter(ci);

	try {
		switch (packet[i + 1]) {
		case CS_Info:
		{
			auto client_View = GetClientView(get_packet);
			xyz packet_position{ client_View->position()->x() , client_View->position()->y() , client_View->position()->z() };
			xyz packet_rotation{ client_View->rotation()->x() , client_View->rotation()->y() , client_View->rotation()->z() };
			client->second.set_client_position(packet_position);
			client->second.set_client_rotation(packet_rotation);
			client->second.set_client_animator(client_View->animator());
			client->second.set_client_weapon(client_View->nowWeapon());
			client->second.set_horizontal(client_View->Horizontal());
			client->second.set_vertical(client_View->Vertical());
			client->second.set_inCar(client_View->inCar());
			client->second.set_client_DangerLine(client_View->dangerLineIn());
			if (client_View->inCar() != -1) {
				// 차량을 실제로 탑승 하고 있을 경우.
				auto item = get_item_iter(client_View->inCar());
				item->second.set_pos(client_View->position()->x(), client_View->position()->y(), client_View->position()->z());
				xyz car_rotation{ client_View->carrotation()->x() , client_View->carrotation()->y() , client_View->carrotation()->z() };
				client->second.set_client_car_rotation(car_rotation);
				item->second.set_rotation(client_View->carrotation()->x(), client_View->carrotation()->y(), client_View->carrotation()->z());
			}
		}
		break;

		case CS_Shot_info:
		{
			auto client_Shot_View = GetClient_packetView(get_packet);
			auto client = get_client_iter(client_Shot_View->id());
			Send_Client_Shot(client->first);
		}
		break;
		case CS_Check_info:
		{
			auto client_Check_info = GetClient_packetView(get_packet);
			if (ci != client_Check_info->id()) {
				/*
				클라이언트의 고유번호와 서버의 고유번호가 다를 경우
				값이 다르다 라는 결과를 표출하고 다시한번 클라이언트 아이디를 보내줘야 한다.
				현재 문제오류가 없어서 일단 임시 보류.
				*/
				std::cout << "클라이언트 의 값 : " << client_Check_info->id() << std::endl;
				std::cout << "실제 값 : " << ci << std::endl;
				Send_Client_ID(ci, SC_ID, false);
			}
		}
		break;
		case CS_Eat_Item:
		{
			auto client_Check_info = GetClient_packetView(get_packet);
			auto iter = get_item_iter(client_Check_info->id());
			iter->second.set_eat(true);
		}
		break;
		case CS_Zombie_info:
		{
			auto client_Check_info = getZombie_CollectionView(get_packet);
			//std::cout << sizeof(get_packet) << std::endl;
			auto packet_zombie = client_Check_info->data();

			for (unsigned int i = 0; i < packet_zombie->size(); ++i) {
				auto iter = get_zombie_iter(packet_zombie->Get(i)->id());
				iter->second.set_animator(packet_zombie->Get(i)->animator());
				xyz packet_position{ packet_zombie->Get(i)->position()->x() , packet_zombie->Get(i)->position()->y() , packet_zombie->Get(i)->position()->z() };
				//std::cout << packet_zombie->Get(i)->id() << " : " << packet_zombie->Get(i)->position()->x() << ", " << packet_zombie->Get(i)->position()->y() << ", " << packet_zombie->Get(i)->position()->z() << std::endl;
				iter->second.set_zombie_position(packet_position);
				xyz packet_rotation{ packet_zombie->Get(i)->rotation()->x() , packet_zombie->Get(i)->rotation()->y() , packet_zombie->Get(i)->rotation()->z() };
				iter->second.set_zombie_rotation(packet_rotation);
			}
		}
		break;
		case CS_Object_HP:
		{
			auto client_Check_info = getGame_HP_SetView(get_packet);
			if (client_Check_info->kind() == Kind_Player) {
				auto iter = get_client_iter(client_Check_info->id());
				iter->second.set_hp(client_Check_info->hp());
				iter->second.set_armour(client_Check_info->armour());
			}
			else if (client_Check_info->kind() == Kind_Zombie) {
				auto iter = get_zombie_iter(client_Check_info->id());
				iter->second.set_hp(client_Check_info->hp());
			}
			else if (client_Check_info->kind() == Kind_Car) {
				auto iter = get_item_iter(client_Check_info->id());
				iter->second.set_hp(client_Check_info->hp());
				std::cout << iter->second.get_hp() << std::endl;
			}
		}
		break;
		case CS_Car_Riding:
		{
			auto client_Check_info = GetClient_packetView(get_packet);
			auto iter = get_item_iter(client_Check_info->id());
			iter->second.set_riding(true);
		}
		break;
		case CS_Car_Rode:
		{
			auto client_Check_info = GetClient_packetView(get_packet);
			auto iter = get_item_iter(client_Check_info->id());
			iter->second.set_riding(false);
		}
		break;
		}


		Send_All_Player(ci);
		Send_Hide_Player(ci);

		Send_All_Zombie(ci);
		Send_Hide_Zombie(ci);

		player_To_Zombie(g_zombie, g_clients);

		Send_All_Item(ci);

	}
	catch (DWORD dwError) {
		errnum++;
		std::cout << "Error : " << dwError << "Count : " << errnum << std::endl;
	}

}

void IOCP_Server::SendPacket(int type, int cl, void * packet, int psize)
{
	auto client = get_client_iter(cl);

	if (client != g_clients.end()) {
		// client가 map 안에 정상적으로 있을 경우에만 작업을 한다.
		if (client->second.get_Socket() != NULL) {
			//int ptype = reinterpret_cast<unsigned char *>(packet)[1];
			OverlappedEx *over = new OverlappedEx;
			ZeroMemory(&over->over, sizeof(over->over));
			over->event_type = OP_SEND;
			char p_size[MAX_PACKET_SIZE]{ 0 };

			// 클라이언트에게 패킷 전송시 <패킷크기 | 패킷 타입> 으로 전송을 한다.
			itoa(psize + 8, p_size, 10);
			int buf_len = (int)strlen(p_size);
			p_size[buf_len] = '|';
			p_size[buf_len + 1] = int(type);

			// 패킷 사이즈를 미리 합쳐서 보내줘야한다.
			memcpy(over->IOCP_buf, packet, psize);

			for (int i = 8; i < psize + 8; ++i) {
				p_size[i] = over->IOCP_buf[i - 8];
			}

			over->wsabuf.buf = reinterpret_cast<CHAR *>(p_size);
			over->wsabuf.len = psize + 8;
			int res = WSASend(client->second.get_Socket(), &over->wsabuf, 1, NULL, 0, &over->over, NULL);
			if (0 != res) {
				int error_no = WSAGetLastError();
				if (WSA_IO_PENDING != error_no) {
					//err_display((char *)"SendPacket:WSASend", error_no);
					DisconnectClient(cl);
				}
			}
		}
	}
}

void IOCP_Server::Send_Client_ID(int client_id, int value, bool allClient)
{
	try {
		// value = 아이디를 추가 or 삭제 할때 사용.
		flatbuffers::FlatBufferBuilder builder;
		auto Client_id = client_id;
		auto orc = CreateClient_Packet(builder, Client_id);
		builder.Finish(orc); // Serialize the root of the object.

		if (allClient == true) {
			// 모든 클라이언트 에게 나갔다는 것을 보내준다..!
			for (auto iter : g_clients) {
				if (iter.second.get_Connect() != true)
					continue;
				SendPacket(value, iter.second.get_client_id(), builder.GetBufferPointer(), builder.GetSize());
			}
		}
		else {
			SendPacket(value, client_id, builder.GetBufferPointer(), builder.GetSize());
		}
	}
	catch (int exceptionCode) {
		std::cout << "Error : " << exceptionCode << std::endl;
	}
}

void IOCP_Server::Send_All_Player(int client)
{
	auto client_iter = get_client_iter(client);
	flatbuffers::FlatBufferBuilder builder;
	std::vector<flatbuffers::Offset<Client_info>> Individual_client;		// 개인 데이터
	for (auto iter : g_clients) {
		if (iter.second.get_Connect() != true || Distance(client_iter->second.get_client_id(), iter.second.get_client_id(), Player_Dist, Kind_Player) == false)
			// 클라이언트가 연결 안되어 있으면 제외 한다.
			continue;

		auto id = iter.second.get_client_id();
		auto name = builder.CreateString(iter.second.nickName);
		auto hp = iter.second.get_hp();
		auto armour = iter.second.get_armour();
		auto animator = iter.second.get_animator();
		float horizontal = iter.second.get_horizontal();
		float vertical = iter.second.get_vertical();
		auto xyz = iter.second.get_position();
		auto rotation = iter.second.get_rotation();
		auto weaponState = iter.second.get_weapon();
		auto inCar = iter.second.get_inCar();
		auto car_rotation = iter.second.get_car_rotation();
		auto client_data = CreateClient_info(builder, id, hp, armour, animator, horizontal, vertical, inCar, name, &xyz, &rotation, &car_rotation, false, weaponState);
		// client_data 라는 테이블에 클라이언트 데이터가 들어가 있다.

		Individual_client.push_back(client_data);	// Vector에 넣었다.
		// Individual_client 라는 전체 테이블에 client_data를 넣어주자.
	}

	auto Full_client_data = builder.CreateVector(Individual_client);		// 이제 Vector로 묶어서 전송할 데이터로 만들어주자.
	auto orc = CreateClient_Collection(builder, Full_client_data);		// 실제로 보내는 테이블 명은 Client_Data
	builder.Finish(orc); // Serialize the root of the object.

	SendPacket(SC_Client_Data, client, builder.GetBufferPointer(), builder.GetSize());
}

void IOCP_Server::Send_All_Zombie(int client)
{
	auto client_iter = get_client_iter(client);

	flatbuffers::FlatBufferBuilder builder;
	std::vector<flatbuffers::Offset<Zombie_info>> Individual_zombie;		// 개인 데이터

	for (auto zombie : g_zombie) {
		if (Distance(client_iter->second.get_client_id(), zombie.second.get_client_id(), Zombie_Dist, Kind_Zombie) == false)
			// 클라이언트가 연결 안되어 있으면 제외 한다.
			continue;

		auto z_id = zombie.first;
		auto z_hp = zombie.second.get_hp();
		auto z_ani = zombie.second.get_animator();
		auto z_target = zombie.second.get_target();
		auto z_pos = zombie.second.get_position();
		auto z_rot = zombie.second.get_rotation();
		auto zombie_data = CreateZombie_info(builder, z_id, z_hp, z_ani, z_target, &z_pos, &z_rot);

		Individual_zombie.push_back(zombie_data);
	}

	auto All_zombie_data = builder.CreateVector(Individual_zombie);		// 이제 Vector로 묶어서 전송할 데이터로 만들어주자.
	auto orc = CreateZombie_Collection(builder, All_zombie_data);		// 실제로 보내는 테이블 명은 Client_Data
	builder.Finish(orc); // Serialize the root of the object.

	SendPacket(SC_Zombie_Info, client, builder.GetBufferPointer(), builder.GetSize());
}

void IOCP_Server::Send_All_Time(int kind, int time, int client_id, bool allClient)
{
	flatbuffers::FlatBufferBuilder builder;
	auto f_kind = kind;
	auto f_time = time;
	auto orc = CreateGame_Timer(builder, f_kind, f_time);
	builder.Finish(orc); // Serialize the root of the object.

	if (allClient == true) {
		// 모든 클라이언트 에게 나갔다는 것을 보내준다..!
		for (auto iter : g_clients) {
			if (iter.second.get_Connect() != true)
				continue;
			SendPacket(SC_Server_Time, iter.second.get_client_id(), builder.GetBufferPointer(), builder.GetSize());
		}
	}
	else {
		SendPacket(SC_Server_Time, client_id, builder.GetBufferPointer(), builder.GetSize());
	}
}

void IOCP_Server::Send_All_Item(int ci)
{
	flatbuffers::FlatBufferBuilder builder;
	std::vector<flatbuffers::Offset<Gameitem>> Individual_client;		// 개인 데이터

	for (auto iter : g_item) {
		//if (iter.second.get_eat() == true)	// 아이템을 이미 먹은경우 패스 한다.
		//	continue;
		if (Distance(ci, iter.first, Player_Dist, Kind_Item) == false && iter.second.get_name() != "UAZ")
			continue;

		auto id = iter.first;
		auto name = builder.CreateString(iter.second.get_name());
		Vec3 pos = { iter.second.get_pos().x, iter.second.get_pos().y,  iter.second.get_pos().z };
		Vec3 rotation = { iter.second.get_rotation().x, iter.second.get_rotation().y,  iter.second.get_rotation().z };
		auto eat = iter.second.get_eat();
		auto riding = iter.second.get_riding();
		auto hp = iter.second.get_hp();
		auto kind = iter.second.get_kind();
		auto client_data = CreateGameitem(builder, id, name, &pos, &rotation, eat, riding, hp, kind);

		Individual_client.push_back(client_data);	// Vector에 넣었다.
	}

	auto Full_client_data = builder.CreateVector(Individual_client);		// 이제 Vector로 묶어서 전송할 데이터로 만들어주자.
	auto orc = CreateGame_Items(builder, Full_client_data);		// 실제로 보내는 테이블 명은 Client_Data
	builder.Finish(orc); // Serialize the root of the object.

	for (auto iter : g_clients) {
		if (iter.second.get_Connect() != true)
			continue;
		SendPacket(SC_Server_Item, iter.second.get_client_id(), builder.GetBufferPointer(), builder.GetSize());
	}

}

void IOCP_Server::Send_Client_Shot(int shot_client)
{
	auto client_iter = get_client_iter(shot_client);
	flatbuffers::FlatBufferBuilder builder;
	auto id = shot_client;
	auto orc = CreateClient_Packet(builder, id);
	builder.Finish(orc); // Serialize the root of the object.

	for (auto iter : g_clients) {
		if (iter.second.get_Connect() != true || Distance(client_iter->second.get_client_id(), iter.second.get_client_id(), Player_Dist, Kind_Player) == false)
			continue;
		SendPacket(SC_Shot_Client, iter.second.get_client_id(), builder.GetBufferPointer(), builder.GetSize());
	}

}

void IOCP_Server::Send_DangerLine_info(int demage, xyz pos, xyz scale)
{
	flatbuffers::FlatBufferBuilder builder;
	auto dem = demage;
	auto position = new Vec3(pos.x, pos.y, pos.z);
	auto sscale = new Vec3(scale.x, scale.y, scale.z);
	auto orc = CreateGameDangerLine(builder, dem, position, sscale);
	builder.Finish(orc); // Serialize the root of the object.

	for (auto iter : g_clients) {
		if (iter.second.get_Connect() != true)
			continue;
		SendPacket(SC_DangerLine, iter.second.get_client_id(), builder.GetBufferPointer(), builder.GetSize());
	}


}

void IOCP_Server::Send_Hide_Player(int client)
{
	auto client_iter = get_client_iter(client);

	for (auto iter : g_clients) {
		if (iter.second.get_Connect() != true)
			continue;
		if (Distance(client_iter->second.get_client_id(), iter.second.get_client_id(), Player_Dist, Kind_Player) == true)
			continue;		// 범위내에 있을경우 넘긴다.

		flatbuffers::FlatBufferBuilder builder;
		auto Client_id = iter.second.get_client_id();
		auto orc = CreateClient_Packet(builder, Client_id);
		builder.Finish(orc); // Serialize the root of the object.

		SendPacket(SC_REMOVE_PLAYER, client_iter->second.get_client_id(), builder.GetBufferPointer(), builder.GetSize());
	}
}

void IOCP_Server::Send_Hide_Zombie(int client)
{
	auto client_iter = get_client_iter(client);

	for (auto iter : g_zombie) {
		if (Distance(client_iter->second.get_client_id(), iter.second.get_client_id(), Zombie_Dist, Kind_Zombie) == true)
			continue;		// 범위내에 있을경우 넘긴다.

		flatbuffers::FlatBufferBuilder builder;
		auto Client_id = iter.second.get_client_id();
		auto orc = CreateClient_Packet(builder, Client_id);
		builder.Finish(orc); // Serialize the root of the object.

		SendPacket(SC_Remove_Zombie, client_iter->second.get_client_id(), builder.GetBufferPointer(), builder.GetSize());
	}
}

void IOCP_Server::Attack_DangerLine_Damge()
{
	for (auto iter : g_clients) {
		if (iter.second.get_Connect() != true)
			continue;
		if (iter.second.get_DangerLine() == false) {
			// 자기장 밖에 있을 경우.
			auto client_iter = get_client_iter(iter.first);
			int my_hp = client_iter->second.get_hp();
			client_iter->second.set_hp(my_hp - DangerLine.get_demage());
		}
	}

}

std::unordered_map< int, Game_Client>::iterator get_client_iter(int ci) {
	return g_clients.find(ci);
}

std::unordered_map< int, Game_Item>::iterator get_item_iter(int ci) {
	return g_item.find(ci);
}

std::unordered_map<int, Game_Zombie>::iterator get_zombie_iter(int ci)
{
	return g_zombie.find(ci);
}

bool Distance(int me, int  you, int Radius, int kind) {
	auto iter_me = g_clients.find(me);
	auto iter_you = g_clients.find(you);
	auto iter_zombie = g_zombie.find(you);
	auto iter_item = g_item.find(you);

	if (kind == Kind_Player)
		return (iter_me->second.get_pos().x - iter_you->second.get_pos().x) * (iter_me->second.get_pos().x - iter_you->second.get_pos().x) +
		(iter_me->second.get_pos().z - iter_you->second.get_pos().z) * (iter_me->second.get_pos().z - iter_you->second.get_pos().z) <= Radius * Radius;
	else if (kind == Kind_Zombie)
		return (iter_me->second.get_pos().x - iter_zombie->second.get_pos().x) * (iter_me->second.get_pos().x - iter_zombie->second.get_pos().x) +
		(iter_me->second.get_pos().z - iter_zombie->second.get_pos().z) * (iter_me->second.get_pos().z - iter_zombie->second.get_pos().z) <= Radius * Radius;
	else if (kind == Kind_Item)
		return (iter_me->second.get_pos().x - iter_item->second.get_pos().x) * (iter_me->second.get_pos().x - iter_item->second.get_pos().x) +
		(iter_me->second.get_pos().z - iter_item->second.get_pos().z) * (iter_me->second.get_pos().z - iter_item->second.get_pos().z) <= Radius * Radius;

	return false;
}

float DistanceToPoint(float player_x, float player_z, float zombie_x, float zombie_z)
{
	// 캐릭터 간의 거리 구하기.
	return (float)sqrt(pow(player_x - zombie_x, 2) + pow(player_z - zombie_z, 2));
}

void player_To_Zombie(std::unordered_map<int, Game_Zombie> &zombie, std::unordered_map<int, Game_Client> &player)
{
	for (auto z : zombie) {
		// TargetPlayer가 실제 서버에 존재하는지 확인하자.
		if (player.find(z.second.get_target()) != player.end()) {
			// 플레이어가 존재한다.
			auto iter = player.find(z.second.get_target());
			float check_dist = DistanceToPoint(iter->second.get_pos().x, iter->second.get_pos().z, z.second.get_pos().x, z.second.get_pos().z);
			if (check_dist >= Zombie_Dist) {
				// 플레이어가 존재하지만 거리가 멀어져 있을 경우 좀비 Target을 초기화
				iter->second.set_limit_zombie(-1);
				zombie.find(z.first)->second.set_target(-1);
			}
			if (iter->second.get_hp() <= 0) {
				// 플레이어 체력이 0 이하일 경우
				iter->second.set_limit_zombie(-1);
				zombie.find(z.first)->second.set_target(-1);
			}
			if (z.second.get_hp() <= 0 && z.second.get_live() != false) {
				// 좀비의 체력이 0일경우
				zombie.find(z.first)->second.set_live(false);
				iter->second.set_limit_zombie(-1);
				//std::cout << iter->first << ", " << iter->second.get_limit_zombie() << std::endl;
				zombie.find(z.first)->second.set_target(-1);
			}
			else {
				// 아직 근처일 경우 넘긴다.
				continue;
			}
		}
		else {
			int player_num = -1;
			float dist = Zombie_Dist;
			for (auto p : player) {
				float check_dist = DistanceToPoint(p.second.get_pos().x, p.second.get_pos().z, z.second.get_pos().x, z.second.get_pos().z);
				if (dist >= check_dist) {
					dist = check_dist;
					player_num = p.first;
				}
			}
			if (player_num == -1)
				continue;
			if (player.find(player_num)->second.get_limit_zombie() < Limit_Zombie) {
				player.find(player_num)->second.set_limit_zombie(1);
				zombie.find(z.first)->second.set_target(player_num);
			}
		}
	}
}
