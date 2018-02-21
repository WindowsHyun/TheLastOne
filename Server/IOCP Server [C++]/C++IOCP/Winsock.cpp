#include "Server.h"
#include "protocol.h"
#include "Flatbuffers_View.h"

using namespace Game::TheLastOne; // Flatbuffers를 읽어오자.

//CLIENT g_clients[MAX_NPC];

void err_quit(char *msg) {
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

void err_display(char *msg, int err_no) {
	LPVOID lpMsgBuf;
	FormatMessage(
		FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM,
		NULL, err_no,
		MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
		(LPTSTR)&lpMsgBuf, 0, NULL);
	printf("[%s] %s\n", msg, (char *)lpMsgBuf);
	LocalFree(lpMsgBuf);
}

void SendPacket(int type, int cl, void *packet, int psize) {
	if (g_clients[cl].connect == true) {
		//int ptype = reinterpret_cast<unsigned char *>(packet)[1];
		OverlappedEx *over = new OverlappedEx;
		ZeroMemory(&over->over, sizeof(over->over));
		over->event_type = OP_SEND;
		char p_size[MAX_PACKET_SIZE]{ 0 };

		// 클라이언트에게 패킷 전송시 <패킷크기 | 패킷 타입> 으로 전송을 한다.
		itoa(psize + 8, p_size, 10);
		int buf_len = strlen(p_size);
		p_size[buf_len] = '|';
		p_size[buf_len+1] = int(type);
		


		// 패킷 사이즈를 미리 합쳐서 보내줘야한다.
		memcpy(over->IOCP_buf, packet, psize);

		for (int i = 8; i < psize + 8; ++i) {
			p_size[i] = over->IOCP_buf[i - 8];
		}

		//strcat( p_size, reinterpret_cast<CHAR *>(over->IOCP_buf) );
		//sprintf( buf, "%c%s", p_size, over->IOCP_buf );

		over->wsabuf.buf = reinterpret_cast<CHAR *>(p_size);
		over->wsabuf.len = psize + 8;
		int res = WSASend(g_clients[cl].client_socket, &over->wsabuf, 1, NULL, 0, &over->over, NULL);
		if (0 != res) {
			int error_no = WSAGetLastError();
			if (WSA_IO_PENDING != error_no) {
				err_display("SendPacket:WSASend", error_no);
				DisconnectClient(cl);
			}
		}
	}
}

void Send_Client_ID(int client_id) {
	flatbuffers::FlatBufferBuilder builder;
	auto Client_id = client_id;
	auto orc = CreateClient_id(builder, Client_id);
	builder.Finish(orc); // Serialize the root of the object.
	SendPacket(SC_ID, client_id, builder.GetBufferPointer(), builder.GetSize());
}


void Send_Position(int client, int object) {
	// client = 자기자신, object = 상대방
	flatbuffers::FlatBufferBuilder builder;
	auto id = object;
	auto name = builder.CreateString(g_clients[object].game_id);
	auto hp = g_clients[object].hp;
	auto shot = g_clients[object].shotting;
	auto xyz = Vec3(g_clients[object].position.x, g_clients[object].position.y, g_clients[object].position.z);
	auto rotation = Vec3(g_clients[object].rotation.x, g_clients[object].rotation.y, g_clients[object].rotation.z);
	auto orc = CreateClient_info(builder, id, hp, shot, name, &xyz, &rotation);
	builder.Finish(orc); // Serialize the root of the object.
	SendPacket(SC_PUT_PLAYER, client, builder.GetBufferPointer(), builder.GetSize());
}

void Send_All_Data(int client, bool shot) {
	flatbuffers::FlatBufferBuilder builder;

	std::vector<flatbuffers::Offset<Client_info>> Individual_client;		// 개인 데이터

	for (int i = 0; i < MAX_Client; ++i) {
		if (g_clients[i].connect != true)
			continue;
		auto id = i;
		auto name = builder.CreateString(g_clients[i].game_id);
		auto hp = g_clients[i].hp;
		auto shot = g_clients[i].shotting;
		auto xyz = Vec3(g_clients[i].position.x, g_clients[i].position.y, g_clients[i].position.z);
		auto rotation = Vec3(g_clients[i].rotation.x, g_clients[i].rotation.y, g_clients[i].rotation.z);
		auto client_data = CreateClient_info(builder, id, hp, shot, name, &xyz, &rotation);
		// client_data 라는 테이블에 클라이언트 데이터가 들어가 있다.

		Individual_client.push_back(client_data);	// Vector에 넣었다.
		// Individual_client 라는 전체 테이블에 client_data를 넣어주자.
	}

	auto Full_client_data = builder.CreateVector(Individual_client);		// 이제 Vector로 묶어서 전송할 데이터로 만들어주자.

	auto orc = CreateAll_information(builder, Full_client_data);		// 실제로 보내는 테이블 명은 Client_Data
	builder.Finish(orc); // Serialize the root of the object.
	//std::cout << builder.GetSize() << std::endl;
	if (shot == true) {
		// Shot 을 할때만 모든 클라이언트에다가 전송을 한다.
		for (int i = 0; i < MAX_Client; ++i) {
			if (g_clients[i].connect != true)
				continue;
			SendPacket(SC_Client_Data, i, builder.GetBufferPointer(), builder.GetSize());
		}
	}
	else {
		SendPacket(SC_Client_Data, client, builder.GetBufferPointer(), builder.GetSize());
	}
}
