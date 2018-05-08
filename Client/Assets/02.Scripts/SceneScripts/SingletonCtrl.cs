using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using TheLastOne.Game.Network;

public class SingletonCtrl : MonoBehaviour
{

    private int nowModeNumber = -1;     // -1로 한 이유는 로그인 눌렀을시 0으로 변경하기 위하여.
    private int waitTime = -1;
    private int playerStatus = 0;       // 플레이어 게임 상태
    private int playerMoney = 0;    // 플레이어 돈
    private string playerID = "";   // 플레이어 아이디
    private string playerIP = "";   // 플레이어 아이피
    private int survivalPlayer = 0;       // 게임 내 살아있는 플레이어 인원 수 
    private bool startCarRun = false;  // 수송차량 출발 대기
    private bool corutinSocket = true;     // 커넥트 하기전 까지 대기

    //-------------------------------------------------------------------------------------------
    // 네트워크 관련한 부분
    private Socket m_Socket;
    private const string default_iPAdress = "127.0.0.1";
    private const int kPort = 9000;
    private static ManualResetEvent connectDone = new ManualResetEvent(false);
    NetworkCtrl networkCtrl = new NetworkCtrl();
    //-------------------------------------------------------------------------------------------

    private static SingletonCtrl instance_S = null; // 정적 변수

    public static SingletonCtrl Instance_S  // 인스턴스 접근 프로퍼티
    {
        get
        {
            return instance_S;
        }
    }

    public int NowModeNumber                  // 플레이어 현재 모드 접근 프로퍼티
    {
        get
        {
            return nowModeNumber;
        }
        set
        {
            nowModeNumber = value;
        }
    }

    public int PlayerStatus                  // 플레이어 게임 상태 접근 프로퍼티
    {
        get
        {
            return playerStatus;
        }
        set
        {
            playerStatus = value;
            // 상태가 변경 되면 바로 패킷을 보내주자.
            // 인게임 Socket이 열려있지 않으므로, 싱글톤에서 패킷을 보내주자.
            Send_Packet(networkCtrl.Player_Status(playerStatus));
        }
    }

    public int PlayerMoney                  // 플레이어 돈 접근 프로퍼티
    {
        get
        {
            return playerMoney;
        }
        set
        {
            playerMoney = value;
        }
    }

    public string PlayerID                  // 플레이어 아이디 접근 프로퍼티
    {
        get
        {
            return playerID;
        }
        set
        {
            playerID = value;
        }
    }

    public string PlayerIP                 // 플레이어 패스워드 접근 프로퍼티
    {
        get
        {
            return playerIP;
        }
        set
        {
            playerIP = value;
        }
    }

    public int SurvivalPlayer                 // 게임 내 살아있는 플레이어 인원수 접근 프로퍼티
    {
        get
        {
            return survivalPlayer;
        }
        set
        {
            survivalPlayer = value;
        }
    }

    public int LobbyWaitTime           // 로비 대기시간
    {
        get
        {
            return waitTime;
        }
        set
        {
            waitTime = value;
        }
    }

    public bool startCarStatus           // 로비 대기시간
    {
        get
        {
            return startCarRun;
        }
        set
        {
            startCarRun = value;
        }
    }

    public Socket PlayerSocket
    {
        get
        {
            return m_Socket;
        }
    }

    void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            // 서버가 정상적으로 연결이 되었을 경우.

            connectDone.Set();
            if (m_Socket.Connected == true)
            {
                Debug.Log("서버와 정상적으로 연결 되었습니다.");
                RecieveHeader();//start recieve header
            }
            else
            {
                Debug.Log("서버와 연결이 끊어졌습니다.");
            }
        }
        catch (Exception e)
        {
            connectDone.Set();
            Console.WriteLine(e.ToString());
        }
    }

    void RecieveHeader()
    {
        try
        {
            NetworkMessage msg = new NetworkMessage();
            m_Socket.BeginReceive(msg.Receivebyte, 0, msg.LimitReceivebyte, SocketFlags.None, new AsyncCallback(RecieveHeaderCallback), msg);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    void RecieveHeaderCallback(IAsyncResult ar)
    {
        try
        {
            NetworkMessage msg = (NetworkMessage)ar.AsyncState;     // Recieve된 Packet을 받아온다.
            int bytesRead = m_Socket.EndReceive(ar);        // 소켓에서 받아온 사이즈를 확인한다.

            PacketData size_data = networkCtrl.Get_packet_size(msg.Receivebyte);

            int psize = size_data.p_size;
            int ptype = msg.Receivebyte[size_data.type_Pos + 1]; // 패킷 타입

            if (psize == bytesRead)
            {
                // 소켓에서 받은 데이터와 실제 패킷 사이즈가 같을 경우
                networkCtrl.ProcessPacket(psize, ptype, msg.Receivebyte);

                // 패킷 처리가 완료 되었으니 다시 리시브 상태로 돌아간다.
                NetworkMessage new_msg = new NetworkMessage();
                m_Socket.BeginReceive(new_msg.Receivebyte, 0, new_msg.LimitReceivebyte, SocketFlags.None, new AsyncCallback(RecieveHeaderCallback), new_msg);
            }
            else
            {
                // 소켓에서 받은 데이터와 실제 패킷 사이즈가 다를 경우
                msg.sb.Append(Encoding.ASCII.GetString(msg.Receivebyte, 0, bytesRead));
                msg.set_prev(bytesRead);
                // 소켓에서 받은 데이터가 안맞는 경우 패킷이 뒤에 붙어서 오는거 같은 느낌이 든다...
                size_data = networkCtrl.Get_packet_size(msg.Receivebyte);
                byte[] recv_byte = new byte[size_data.p_size + 9];

                for (int i = 0; i < size_data.p_size; ++i)
                {
                    recv_byte[i] = msg.Receivebyte[i];
                }

                networkCtrl.ProcessPacket(psize, ptype, recv_byte);

                m_Socket.BeginReceive(msg.Receivebyte, 0, msg.LimitReceivebyte, SocketFlags.None, new AsyncCallback(RecieveHeaderCallback), msg);
            }
        }
        catch
        {
            NetworkMessage new_msg = new NetworkMessage();
            m_Socket.BeginReceive(new_msg.Receivebyte, 0, new_msg.LimitReceivebyte, SocketFlags.None, new AsyncCallback(RecieveHeaderCallback), new_msg);
        }
    }

    public void Send_Packet(byte[] packet)
    {
        if (m_Socket.Connected == true)
        {
            try
            {
                m_Socket.Send(packet, packet.Length, 0);
            }
            catch (SocketException err)
            {
                Debug.Log("Singleton Socket send or receive error! : " + err.ToString());
            }
        }
    }

    IEnumerator connectSocket()
    {
        do
        {
            if (nowModeNumber == 0)
            {
                // Mode가 0이 될 경우 = 플레이어가 로그인 한 이후
                m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                m_Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
                m_Socket.NoDelay = true;
                if (playerIP == "")
                    m_Socket.BeginConnect(default_iPAdress, kPort, new AsyncCallback(ConnectCallback), m_Socket);
                else
                    m_Socket.BeginConnect(playerIP, kPort, new AsyncCallback(ConnectCallback), m_Socket);

                corutinSocket = false;
            }
            yield return null;
        } while (corutinSocket);
    }

    private void Awake()
    {
        if (instance_S)                     // 인스턴스가 이미 생성 되었는가?
        {
            DestroyImmediate(gameObject);   // 또 만들 필요가 없다 -> 삭제
            return;
        }
        instance_S = this;                  // 유일한 인스턴스로 만듬
        DontDestroyOnLoad(gameObject);      // 씬이 바뀌어도 계속 유지 시킨다
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        Application.runInBackground = true;
        StartCoroutine(connectSocket());
    }

}
