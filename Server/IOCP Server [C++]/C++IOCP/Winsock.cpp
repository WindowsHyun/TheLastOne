#include "Server.h"
#include "protocol.h"
#include "Flatbuffers_View.h"

using namespace Game::Hyun; // Flatbuffers를 읽어오자.

void err_quit( char *msg ) {
	LPVOID lpMsgBuf;
	FormatMessage(
		FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM,
		NULL, WSAGetLastError(),
		MAKELANGID( LANG_NEUTRAL, SUBLANG_DEFAULT ),
		(LPTSTR)&lpMsgBuf, 0, NULL );
	printf( "Error : %s\n", msg );
	LocalFree( lpMsgBuf );
	exit( 1 );
}

void err_display( char *msg, int err_no ) {
	LPVOID lpMsgBuf;
	FormatMessage(
		FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM,
		NULL, err_no,
		MAKELANGID( LANG_NEUTRAL, SUBLANG_DEFAULT ),
		(LPTSTR)&lpMsgBuf, 0, NULL );
	printf( "[%s] %s\n", msg, (char *)lpMsgBuf );
	LocalFree( lpMsgBuf );
}

void SendPacket( int cl, void *packet ) {
	if ( g_clients[cl].connect == true ) {
		int psize = reinterpret_cast<unsigned char *>(packet)[0];
		int ptype = reinterpret_cast<unsigned char *>(packet)[1];
		OverlappedEx *over = new OverlappedEx;
		ZeroMemory( &over->over, sizeof( over->over ) );
		over->event_type = OP_SEND;
		memcpy( over->IOCP_buf, packet, psize );
		over->wsabuf.buf = reinterpret_cast<CHAR *>(over->IOCP_buf);
		over->wsabuf.len = psize;
		int res = WSASend( g_clients[cl].client_socket, &over->wsabuf, 1, NULL, 0, &over->over, NULL );
		if ( 0 != res ) {
			int error_no = WSAGetLastError();
			if ( WSA_IO_PENDING != error_no ) {
				err_display( "SendPacket:WSASend", error_no );
				DisconnectClient( cl );
			}
		}
	}
}

void Send_Test_Packet( int cl, void *packet, int psize ) {
	if ( g_clients[cl].connect == true ) {
		//int ptype = reinterpret_cast<unsigned char *>(packet)[1];
		OverlappedEx *over = new OverlappedEx;
		ZeroMemory( &over->over, sizeof( over->over ) );
		over->event_type = OP_SEND;
		char p_size[MAX_PACKET_SIZE]{ int( psize ) };
		//char buf[MAX_PACKET_SIZE];
		// 패킷 사이즈를 미리 합쳐서 보내줘야한다.
		//for ( int i = 4; i <= psize; ++i ) {
		//	p_size[i] = (char *)packet[i-4];
		//}
		//char t_buf[MAX_BUFF_SIZE];
		//memcpy( t_buf, packet, psize );
		//strcat( p_size, t_buf );

		
		memcpy( over->IOCP_buf, packet, psize );

		for ( int i = 4; i < psize+4; ++i ) {
			p_size[i] = over->IOCP_buf[i - 4];
		}

		//strcat( p_size, reinterpret_cast<CHAR *>(over->IOCP_buf) );
		//sprintf( buf, "%c%s", p_size, over->IOCP_buf );

		//std::cout << p_size << std::endl;


		over->wsabuf.buf = reinterpret_cast<CHAR *>(p_size);
		over->wsabuf.len = psize+4;
		int res = WSASend( g_clients[cl].client_socket, &over->wsabuf, 1, NULL, 0, &over->over, NULL );
		if ( 0 != res ) {
			int error_no = WSAGetLastError();
			if ( WSA_IO_PENDING != error_no ) {
				err_display( "SendPacket:WSASend", error_no );
				DisconnectClient( cl );
			}
		}
	}
}

void Send_Flat_Packet(int client, int object){
	flatbuffers::FlatBufferBuilder builder;
	auto pos = Vec3( g_clients[client].client_xyz.x + 1.1f, g_clients[client].client_xyz.y, g_clients[client].client_xyz.z );
	auto view = Vec3( 30.16f, 20.123f, 10.0f );
	auto orc = CreateClient( builder, &pos, &view );
	builder.Finish( orc ); // Serialize the root of the object.

	//char * buf = new char[BUFSIZE];

	//memcpy( buf, builder.GetBufferPointer(), builder.GetSize() );

	Send_Test_Packet( client, builder.GetBufferPointer(), builder.GetSize() );
}

void SendPutPlayerPacket( int client, int object ) {
	sc_packet_put_player packet;
	packet.id = object;
	packet.size = sizeof( packet );
	packet.type = SC_PUT_PLAYER;
	//packet.x = g_clients[object].x;
	//packet.y = g_clients[object].y;
	SendPacket( client, &packet );
}

void SendPositionPacket( int client, int object ) {
	sc_packet_pos packet;
	packet.id = object;
	packet.size = sizeof( packet );
	packet.type = SC_POS;
	//packet.x = g_clients[object].x;
	//packet.y = g_clients[object].y;
	SendPacket( client, &packet );
}

void SendRemovePlayerPacket( int client, int object ) {
	sc_packet_remove_player packet;
	packet.id = object;
	packet.size = sizeof( packet );
	packet.type = SC_REMOVE_PLAYER;

	SendPacket( client, &packet );
}
