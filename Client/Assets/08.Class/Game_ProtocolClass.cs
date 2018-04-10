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

        public int CS_Info;              // 클라이언트가 서버에게 자신의 위치정보를 보내준다.
        public int CS_Shot_info;       // 클라이언트가 서버에게 Shot 정보를 보내준다.
        public int CS_Check_info;       // 클라이언트가 서버에게 자신의 정보가 맞는지 확인해 준다.
        public int CS_Eat_Item;					// 클라이언트가 서버에게 먹은 아이템 정보를 보내준다.

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
            //----------------------------------
            // 클라이언트에서 서버로
            CS_Info = 1;
            CS_Shot_info = 2;
            CS_Check_info = 3;
            CS_Eat_Item = 4;
            //----------------------------------
        }
    }
}
