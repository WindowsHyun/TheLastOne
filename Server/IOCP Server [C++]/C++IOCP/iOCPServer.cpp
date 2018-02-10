#include "Server.h"
#include "protocol.h"
#include "Flatbuffers_View.h"

HANDLE g_hiocp;
SOCKET g_ssocket;
CLIENT g_clients[MAX_NPC];
std::chrono::high_resolution_clock::time_point serverTimer;
int errnum = 0;

using namespace Game::TheLastOne; // Flatbuffers를 읽어오자.

int main() {
	std::vector <std::thread * > worker_threads;
	init();
	serverTimer = high_resolution_clock::now();

	for (int i = 0; i < 6; ++i) {
		worker_threads.emplace_back(new std::thread{ Worker_Thread });
	}

	std::thread accept_tread{ Accept_Thread };
	accept_tread.join();
	for (auto pth : worker_threads) {
		pth->join();
		delete pth;
	}

	worker_threads.clear();
	Shutdown_Server();
}

void init() {
	std::wcout.imbue(std::locale("korean"));

	WSADATA	wsadata;
	WSAStartup(MAKEWORD(2, 2), &wsadata);

	g_hiocp = CreateIoCompletionPort(INVALID_HANDLE_VALUE, 0, NULL, 0);

	g_ssocket = WSASocket(AF_INET, SOCK_STREAM, IPPROTO_TCP, NULL, 0, WSA_FLAG_OVERLAPPED);

	SOCKADDR_IN ServerAddr;
	ZeroMemory(&ServerAddr, sizeof(SOCKADDR_IN));
	ServerAddr.sin_family = AF_INET;
	ServerAddr.sin_port = htons(SERVERPORT);
	ServerAddr.sin_addr.s_addr = INADDR_ANY;


	bind(g_ssocket, reinterpret_cast<sockaddr *>(&ServerAddr), sizeof(ServerAddr));
	listen(g_ssocket, 5);

	for (int i = 0; i < MAX_Client; ++i)
		g_clients[i].connect = false;

	std::cout << "init Complete..!" << std::endl;
}

void Worker_Thread() {
	while (true) {
		DWORD io_size;
		unsigned long long ci;
		OverlappedEx *over;
		BOOL ret = GetQueuedCompletionStatus(g_hiocp, &io_size, &ci, reinterpret_cast<LPWSAOVERLAPPED *>(&over), INFINITE);

		if (FALSE == ret) {
			int err_no = WSAGetLastError();
			if (err_no == 64)
				DisconnectClient(ci);
			else
				err_display("QOCS : ", WSAGetLastError());

		}
		if (0 == io_size) {
			DisconnectClient(ci);
			continue;
		}

		if (OP_RECV == over->event_type) {
#if (DebugMod == TRUE )
			//printf( "Send Incomplete Error!\n" );
			printf("RECV from Client : %d\n", ci);
			printf("IO_SIZE : %d\n", io_size);
#endif
			unsigned char *buf = g_clients[ci].recv_over.IOCP_buf;

			unsigned psize = g_clients[ci].curr_packet_size;
			unsigned pr_size = g_clients[ci].prev_packet_data;
			while (io_size != 0) {
				if (0 == psize) psize = buf[0] + 4;
				// 0일 경우[바로전 패킷이 처리가 끝나고 새피킷으로 시작해도 된다. / 처음 받는다] 강제로 정해준다.
				if (io_size + pr_size >= psize) {
					// 지금 패킷 완성이 가능하다.
					char packet[MAX_PACKET_SIZE];
					memcpy(packet, g_clients[ci].packet_buf, pr_size);
					memcpy(packet + pr_size, buf, psize - pr_size);
					ProcessPacket(static_cast<int>(ci), packet);
					io_size -= psize - pr_size;
					buf += psize - pr_size;
					psize = 0; pr_size = 0;
				}
				else {
					memcpy(g_clients[ci].packet_buf + pr_size, buf, io_size);
					pr_size += io_size;
					io_size = 0;
				}
			}
			g_clients[ci].curr_packet_size = psize;
			g_clients[ci].prev_packet_data = pr_size;
			DWORD recv_flag = 0;
			WSARecv(g_clients[ci].client_socket, &g_clients[ci].recv_over.wsabuf, 1, NULL, &recv_flag, &g_clients[ci].recv_over.over, NULL);

		}
		else if (OP_SEND == over->event_type) {
			if (io_size != over->wsabuf.len) { // 같아야 되는데 다르므로 
#if (DebugMod == TRUE )
				printf("Send Incomplete Error!\n");
#endif
				closesocket(g_clients[ci].client_socket);
				g_clients[ci].connect = false;
				exit(-1);
			}
			delete over;
		}
		else if (OP_DO_AI == over->event_type) {
			delete over;
		}
		else if (E_PLAYER_MOVE_NOTIFY == over->event_type) {
			delete over;
		}
		else if (OP_Attack_Move == over->event_type) {

			delete over;
		}
		else if (OP_Responder == over->event_type) {
			delete over;
		}
		else {
#if (DebugMod == TRUE )
			printf("Unknown GQCS event!\n");
#endif
			exit(-1);
		}

	}
}

void Accept_Thread() {
	SOCKADDR_IN ClientAddr;
	ZeroMemory(&ClientAddr, sizeof(SOCKADDR_IN));
	ClientAddr.sin_family = AF_INET;
	ClientAddr.sin_port = htons(SERVERPORT);
	ClientAddr.sin_addr.s_addr = INADDR_ANY;
	int addr_size = sizeof(ClientAddr);
	while (true) {
		SOCKET new_client = WSAAccept(g_ssocket, reinterpret_cast<sockaddr *>(&ClientAddr), &addr_size, NULL, NULL);

		if (INVALID_SOCKET == new_client) {
			int err_no = WSAGetLastError();
			err_display("WSAAccept : ", err_no);
		}

		int new_id = -1;

		for (int i = 0; i < MAX_Client; ++i) {
			if (g_clients[i].connect == false) {
				new_id = i;
#if (DebugMod == TRUE )
				printf("New Client : %d\n", new_id);
#endif
				break;
			}
		}

		if (-1 == new_id) {
#if (DebugMod == TRUE )
			printf("설정된 인원 이상으로 접속하여 차단하였습니다..!\n");
#endif
			closesocket(new_client);
			continue;
		}

		//---------------------------------------------------------------------------------------------------------------------------------------------------
		// 들어온 접속 아이디 init 처리
		std::cout << "id : " << new_id << std::endl;
		g_clients[new_id].connect = true;
		g_clients[new_id].client_socket = new_client;
		g_clients[new_id].curr_packet_size = 0;
		g_clients[new_id].prev_packet_data = 0;
		ZeroMemory(&g_clients[new_id].recv_over, sizeof(g_clients[new_id].recv_over));
		g_clients[new_id].recv_over.event_type = OP_RECV;
		g_clients[new_id].recv_over.wsabuf.buf = reinterpret_cast<CHAR *>(g_clients[new_id].recv_over.IOCP_buf);
		g_clients[new_id].recv_over.wsabuf.len = sizeof(g_clients[new_id].recv_over.IOCP_buf);
		g_clients[new_id].position.x = 0;
		g_clients[new_id].position.y = 0;
		g_clients[new_id].position.z = 0;
		g_clients[new_id].rotation.x = 0;
		g_clients[new_id].rotation.y = 0;
		g_clients[new_id].rotation.z = 0;
		g_clients[new_id].hp = 100;

		Send_Client_ID(new_id);		// 클라이언트에게 자신의 아이디를 보내준다.
		//---------------------------------------------------------------------------------------------------------------------------------------------------
		DWORD recv_flag = 0;
		CreateIoCompletionPort(reinterpret_cast<HANDLE>(new_client), g_hiocp, new_id, 0);
		int ret = WSARecv(new_client, &g_clients[new_id].recv_over.wsabuf, 1, NULL, &recv_flag, &g_clients[new_id].recv_over.over, NULL);

		if (0 != ret) {
			int error_no = WSAGetLastError();
			if (WSA_IO_PENDING != error_no) {
				err_display("RecvPacket:WSARecv", error_no);
				//while ( true );
			}
		}

	}

}

void Shutdown_Server() {
	closesocket(g_ssocket);
	CloseHandle(g_hiocp);
	WSACleanup();
}

void DisconnectClient(int ci) {
	closesocket(g_clients[ci].client_socket);
	g_clients[ci].connect = false;

	std::cout << "Disconnect Client : " << ci << std::endl;
}

void ProcessPacket(int ci, char *packet) {
	char get_packet[MAX_PACKET_SIZE];
	for (int i = 4; i < MAX_PACKET_SIZE; ++i)
		get_packet[i - 4] = packet[i];

	try {
		auto client_View = GetClientView(get_packet);
		g_clients[ci].position.x = client_View->position()->x();
		g_clients[ci].position.y = client_View->position()->y();
		g_clients[ci].position.z = client_View->position()->z();
		g_clients[ci].rotation.x = client_View->rotation()->x();
		g_clients[ci].rotation.y = client_View->rotation()->y();
		g_clients[ci].rotation.z = client_View->rotation()->z();

		//std::cout << g_clients[ci].view.x << ", " << g_clients[ci].view.y << ", " << g_clients[ci].view.z << std::endl;


		/*
		// 임시로 클라 0에게 1의 데이터를 보낸다.
		for (int i = 1; i < 10; ++i) {
			g_clients[ci + i].hp = i;
			g_clients[ci + i].client_xyz.x = client_View->position()->x() + 10 + ( i  * 2);
			g_clients[ci + i].client_xyz.y = client_View->position()->y();
			g_clients[ci + i].client_xyz.z = client_View->position()->z();
			g_clients[ci + i].view.x = client_View->rotation()->x();
			g_clients[ci + i].view.y = client_View->rotation()->y();
			g_clients[ci + i].view.z = client_View->rotation()->z();
		}
		//Send_Flat_Packet( ci, ci+1 );
		for (int i = 1; i < 10; ++i) {
			Send_Position(ci, i);
		}
		*/

		/*
		for (int i = 0; i < 10; ++i) {
			if (ci == i)
				continue;
			Send_Position(ci, i);
		}
		*/
		Send_All_Data(ci);

	}
	catch (DWORD dwError) {
		errnum++;
		std::cout << "Error : " << dwError << "Count : " << errnum << std::endl;
	}

	/*
	// 첫번째 패킷정보를 읽어서 어떤 패킷인지 분석하는것은 하지 않는다.
	// Flatbuffers를 사용하면서 구조가 달라졌다.
	switch ( packet[1] ) {
	case CS_UP:
		break;
	case CS_DOWN:
		break;
	case CS_LEFT:
		break;
	case CS_RIGHT:
		break;
	case CS_Attack:
		break;
	case CS_Move:
		break;
	default:
#if (DebugMod == TRUE )
		//printf( "Unknown Packet Type from Client : %d -> %d\n", ci, packet[1] );

#endif
		break;
		//exit( -1 );
	}
	*/
}
