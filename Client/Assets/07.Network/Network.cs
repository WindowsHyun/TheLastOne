using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//---------------------------------------------------------------
// Network
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading; // ManualResetEvent
using System.Runtime.InteropServices; // sizeof

using System;
using System.ComponentModel;
using System.Linq;
using FlatBuffers;
using Game.TheLastOne; // Client, Vec3 을 불러오기 위해
                       //---------------------------------------------------------------

public struct Client_Data
{
    public int id;
    public Vector3 position;  // 클라이언트 위치
    public Vector3 rotation;    // 클라이언트 보는 방향
    public bool prefab;    // 클라이언트 접속
    public bool connect;    // 클라이언트 접속
    public string name;     // 클라이언트 닉네임
    public GameObject Player;

    public Client_Data(Vector3 position, Vector3 rotation)
    {
        this.id = -1;
        this.position = position;
        this.rotation = rotation;
        this.connect = false;
        this.prefab = false;
        this.Player = null;
        this.name = "";
    }
}

public class Network : MonoBehaviour
{
    public static Socket m_Socket;
    public GameObject Player;
    public GameObject PrefabPlayer;

    Vector3 Player_Position;
    Vector3 Player_Rotation;

    public string iPAdress = "127.0.0.1";
    public const int kPort = 9000;

    public bool Connect = false;
    private const int MaxClient = 50;    // 최대 동접자수
    public static Client_Data[] client_data = new Client_Data[MaxClient];      // 클라이언트 데이터 저장할 구조체
    public int Client_imei = 0;         // 자신의 클라이언트 아이디
    internal static ManualResetEvent recvDone = new ManualResetEvent(false);    //  recv 이벤트 제어


    public int LimitReceivebyte = 2000;                     // Receive Data Length. (byte)
    private byte[] Receivebyte = new byte[2000];    // Receive data by this array to save.
    private string ReceiveString;                     // Receive bytes to Change string. 

    FlatBufferBuilder fbb = new FlatBufferBuilder(1);

    IEnumerator startPrefab()
    {
        do
        {
            for (int i = 0; i < MaxClient; ++i)
            {
                if (Client_imei == i)
                    continue;
                if (client_data[i].connect == true && client_data[i].prefab == false)
                {
                    client_data[i].Player = Instantiate(PrefabPlayer, client_data[i].position, Quaternion.identity);
                    client_data[i].prefab = true;
                }
                else if (client_data[i].prefab == true)
                {
                    client_data[i].Player.transform.position = client_data[i].position;

                    var rotationX = client_data[i].rotation.x;
                    var rotationY = client_data[i].rotation.y;
                    var rotationZ = client_data[i].rotation.z;
                    //Debug.Log("  :  "+ rotationX + ", " + rotationY + ", " + rotationZ);
                    client_data[i].Player.transform.rotation = Quaternion.Euler(rotationX, rotationY, rotationZ);
                }
            }

            yield return null;
        } while (true);


        yield return null;
    }


    void ProcessPacket(int size, int type, byte[] recvPacket)
    {
        if (type == 1)
        {
            // 클라이언트 아이디를 가져온다.
            byte[] t_buf = new byte[size + 1];
            System.Buffer.BlockCopy(recvPacket, 2, t_buf, 0, size); // 사이즈를 제외한 실제 패킷값을 복사한다.
            ByteBuffer revc_buf = new ByteBuffer(t_buf); // ByteBuffer로 byte[]로 복사한다.
            var Get_ServerData = Client_id.GetRootAsClient_id(revc_buf);
            Client_imei = Get_ServerData.Id;
            Debug.Log("클라이언트 아이디 : " + Client_imei);
        }
        else if (type == 2)
        {
            // 클라이언트 하나에 대한 데이터가 들어온다.
            byte[] t_buf = new byte[size + 1];
            System.Buffer.BlockCopy(recvPacket, 2, t_buf, 0, size); // 사이즈를 제외한 실제 패킷값을 복사한다.
            ByteBuffer revc_buf = new ByteBuffer(t_buf); // ByteBuffer로 byte[]로 복사한다.

            var data = new Offset<Client_info>[MaxClient];
            var Get_ServerData = Client_info.GetRootAsClient_info(revc_buf);


            client_data[Get_ServerData.Id].position = new Vector3(Get_ServerData.Position.Value.X, Get_ServerData.Position.Value.Y, Get_ServerData.Position.Value.Z);
            client_data[Get_ServerData.Id].rotation = new Vector3(Get_ServerData.Rotation.Value.X, Get_ServerData.Rotation.Value.Y, Get_ServerData.Rotation.Value.Z);
            client_data[Get_ServerData.Id].name = Get_ServerData.Name;

            if (client_data[Get_ServerData.Id].connect != true && Client_imei != Get_ServerData.Id)
            {
                // 클라이언트가 처음 들어와서 프리팹이 없을경우 
                client_data[Get_ServerData.Id].connect = true;
            }
        }
        else if (type == 4)
        {
            // 클라이언트 모든 데이터가 들어온다.
            byte[] t_buf = new byte[size + 1];
            System.Buffer.BlockCopy(recvPacket, 2, t_buf, 0, size); // 사이즈를 제외한 실제 패킷값을 복사한다.
            ByteBuffer revc_buf = new ByteBuffer(t_buf); // ByteBuffer로 byte[]로 복사한다.
            var Get_ServerData = All_information.GetRootAsAll_information(revc_buf);

            for (int i = 0; i < Get_ServerData.DataLength; i++)
            {
                var client_id = Get_ServerData.Data(i).Value.Id;
                var position_x = Get_ServerData.Data(i).Value.Position.Value.X;
                var position_y = Get_ServerData.Data(i).Value.Position.Value.Y;
                var position_z = Get_ServerData.Data(i).Value.Position.Value.Z;
                client_data[client_id].position = new Vector3(position_x, position_y, position_z);

                var rotation_x = Get_ServerData.Data(i).Value.Rotation.Value.X;
                var rotation_y = Get_ServerData.Data(i).Value.Rotation.Value.Y;
                var rotation_z = Get_ServerData.Data(i).Value.Rotation.Value.Z;
                client_data[client_id].rotation = new Vector3(rotation_x, rotation_y, rotation_z);

                client_data[client_id].name = Get_ServerData.Data(i).Value.Name;

                if (client_data[client_id].connect != true && Client_imei != client_id)
                {
                    // 클라이언트가 처음 들어와서 프리팹이 없을경우 
                    client_data[client_id].connect = true;
                }

            }
        }



    }

    void Awake()
    {
        Application.runInBackground = true; // 백그라운드에서도 Network는 작동해야한다.
        //=======================================================
        // Socket create.
        m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        m_Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 10000);
        m_Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 10000);

        //=======================================================
        // Socket connect.
        try
        {
            IPAddress ipAddr = System.Net.IPAddress.Parse(iPAdress);
            IPEndPoint ipEndPoint = new System.Net.IPEndPoint(ipAddr, kPort);
            m_Socket.Connect(ipEndPoint);
            m_Socket.BeginReceive(Receivebyte, 0, LimitReceivebyte, 0, DataReceived, m_Socket);
            StartCoroutine(startPrefab());
        }
        catch (SocketException SCE)
        {
            Debug.Log("Socket connect error! : " + SCE.ToString());
            return;
        }

        //=======================================================
    }

    void DataReceived(IAsyncResult ar)
    {
        //int psize = Receivebyte[0]; // 패킷 사이즈
        int psize = m_Socket.EndReceive(ar);
        int ptype = Receivebyte[0]; // 패킷 타입
        Debug.Log("총 사이즈 : " + psize + ", 패킷 타입 : " + ptype);
        ProcessPacket(psize, ptype, Receivebyte);

        m_Socket.BeginReceive(Receivebyte, 0, LimitReceivebyte, 0, DataReceived, m_Socket);
    }

    void Send_Packet(byte[] packet)
    {
        try
        {
            m_Socket.Send(packet, packet.Length, 0);
        }
        catch (SocketException err)
        {
            Debug.Log("Socket send or receive error! : " + err.ToString());
        }
    }

    void Send_POS(Vector3 Player, Vector3 PlayerRotation)
    {
        //var offset = fbb.CreateString("WindowsHyun"); // String 문자열이 있을경우 미리 생성해라.
        fbb.Clear(); // 클리어를 안해주고 시작하면 계속 누적해서 데이터가 들어간다.
        Client_info.StartClient_info(fbb);
        //Client.AddName(fbb, offset); // string 사용
        Client_info.AddPosition(fbb, Vec3.CreateVec3(fbb, Player.x, Player.y, Player.z));
        Client_info.AddRotation(fbb, Vec3.CreateVec3(fbb, PlayerRotation.x, PlayerRotation.y, PlayerRotation.z));
        var endOffset = Client_info.EndClient_info(fbb);
        fbb.Finish(endOffset.Value);



        byte[] packet = fbb.SizedByteArray();
        //Debug.Log(packet.Length);
        byte[] packet_len = BitConverter.GetBytes(packet.Length);
        //Debug.Log(packet_len.Length);
        byte[] real_packet = new byte[packet_len.Length + packet.Length];

        System.Buffer.BlockCopy(packet_len, 0, real_packet, 0, packet_len.Length);
        System.Buffer.BlockCopy(packet, 0, real_packet, packet_len.Length, packet.Length);

        Send_Packet(real_packet);
        //Debug.Log(real_packet.Length);
        //Debug.Log("-----------------------");
        /*
        Array.Clear(packet, 0, packet.Length);
        Array.Clear(packet_len, 0, packet_len.Length);
        Array.Clear(real_packet, 0, real_packet.Length);
        */
    }

    void OnApplicationQuit()
    {
        m_Socket.Close();
        m_Socket = null;
    }

    void Update()
    {

        Player_Position.x = Player.transform.position.x;
        Player_Position.y = Player.transform.position.y;
        Player_Position.z = Player.transform.position.z;
        Player_Rotation.x = Player.transform.eulerAngles.x;
        Player_Rotation.y = Player.transform.eulerAngles.y;
        Player_Rotation.z = Player.transform.eulerAngles.z;
        //Debug.Log(Player_Position.x + ", " + Player_Position.y + ", " + Player_Position.z);
        //Debug.Log(Player_Rotation.x + ", " + Player_Rotation.y + ", " + Player_Rotation.z);
        Send_POS(Player_Position, Player_Rotation);

        if (Connect == false)
        {
            Connect = true;
            //StartCoroutine(startServer(m_Socket));
        }
    }
}
