using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheLastOne.GameClass
{
    public class Game_ProtocolClass
    {
        // 서버가 클라이언트에게 보내는 이벤트 타입
        public int SC_ID;                          // 클라이언트 아이디를 보낸다.
        public int SC_PUT_PLAYER;            // 클라이언트 추가
        public int SC_REMOVE_PLAYER;     // 클라이언트 삭제
        public int SC_Client_Data;             // 클라이언트 모든 데이터
        public int SC_Server_Time;             // 서버 타이머
        public int SC_Server_Item;             // 서버 아이템
        public int SC_Shot_Client;              // 클라이언트 Shot 정보
        public int SC_DangerLine;				// 클라이언트 DangerLine 정보 전송
        public int SC_Zombie_Info;               // 클라이언트에게 좀비 위치를 전달해 준다.
        public int SC_Remove_Zombie;            // 좀비 삭제
        public int SC_Lobby_Time;               // 클라이언트 에게 로비 대기시간을 보내준다.
        public int SC_StartCar_Play;			// 클라이언트 에게 시작 차량을 움직이라고 보내준다.

        public int CS_Info;              // 클라이언트가 서버에게 자신의 위치정보를 보내준다.
        public int CS_Shot_info;       // 클라이언트가 서버에게 Shot 정보를 보내준다.
        public int CS_Check_info;       // 클라이언트가 서버에게 자신의 정보가 맞는지 확인해 준다.
        public int CS_Eat_Item;					// 클라이언트가 서버에게 먹은 아이템 정보를 보내준다.
        public int CS_Zombie_info;      // 클라이언트가 서버에게 좀비 데이터를 전달해 준다.
        public int CS_Object_HP;        // 클라이언트가 서버에게 HP 데이터를 전달해 준다.
        public int CS_Car_Riding; 		// 클라이언트가 서버에게 차량에 탑승했다고 전달해 준다.
        public int CS_Car_Rode;             // 클라이언트가 서버에게 차량에 하차했다고 전달해 준다.
        public int CS_Player_Status;            // 클라이언트가 서버에게 자신의 상태를 전달한다.
        public int SC_Survival_Count;			// 클라이언트 에게 총 인원을 보내준다.
        public int Kind_Item;           // 아이템
        public int Kind_Car;            // 자동차
        public int Kind_Zombie;     // 좀비
        public int Kind_Player;     // 플레이어

        public int LoginStatus;     // 로그인 상태
        public int LobbyStatus;     // 로비 상태
        public int ReadyStatus;     // 로비에서 Reday 상태
        public int inGameStatus;    // 게임 상태
        public int playGameStatus;  // 모든 플레이어가 게임 상태이다.


        public Game_ProtocolClass()
        {
            //----------------------------------
            // 서버에서 클라이언트로
            SC_ID = 1;
            SC_PUT_PLAYER = 2;
            SC_REMOVE_PLAYER = 3;
            SC_Client_Data = 4;
            SC_Server_Time = 5;
            SC_Server_Item = 6;
            SC_Shot_Client = 7;
            SC_DangerLine = 8;
            SC_Zombie_Info = 9;
            SC_Remove_Zombie = 10;
            SC_Lobby_Time = 11;
            SC_StartCar_Play = 12;
            SC_Survival_Count = 13;
            //----------------------------------
            // 클라이언트에서 서버로
            CS_Info = 1;
            CS_Shot_info = 2;
            CS_Check_info = 3;
            CS_Eat_Item = 4;
            CS_Zombie_info = 5;
            CS_Object_HP = 6;
            CS_Car_Riding = 7;
            CS_Car_Rode = 8;
            CS_Player_Status = 9;
            //----------------------------------
            // 아이템 종류
            Kind_Item = 0;
            Kind_Car = 1;
            Kind_Player = 2;
            Kind_Zombie = 3;
            //----------------------------------
            LoginStatus = 0;
            LobbyStatus = 1;
            ReadyStatus = 2;
            inGameStatus = 3;
            playGameStatus = 4;
        }
    }
}
