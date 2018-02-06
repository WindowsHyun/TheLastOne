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
using Game.Hyun; // Client, Vec3 을 불러오기 위해
//---------------------------------------------------------------

public class Network : MonoBehaviour
{
    private Socket m_Socket;
    public GameObject Player;
    public GameObject OtherPlayer;
    public GameObject Camera;

    /*
     // 오브젝트를 생성한다.
    public GameObject[] cube = new GameObject[6];
    //cube[1] = (GameObject)Instantiate(OtherPlayer, OtherPlayer.transform.position, OtherPlayer.transform.rotation);
    //cube[1].transform.position = new Vector3(cube[1].transform.position.x + 0.001f, cube[1].transform.position.y, cube[1].transform.position.z);
    */

    Vector3 Player_Pos;
    Vector3 Camera_Pos;

    public string iPAdress = "127.0.0.1";
    public const int kPort = 9000;

    //private int SenddataLength;                     // Send Data Length. (byte)
    private int ReceivedataLength;                     // Receive Data Length. (byte)

    //private byte[] Sendbyte;                        // Data encoding to send. ( to Change bytes)
    private byte[] Receivebyte = new byte[2000];    // Receive data by this array to save.
    private string ReceiveString;                     // Receive bytes to Change string. 

    FlatBufferBuilder fbb = new FlatBufferBuilder(1);

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
        }
        catch (SocketException SCE)
        {
            Debug.Log("Socket connect error! : " + SCE.ToString());
            return;
        }

        //=======================================================
        // Send data write.




        //client.BeginSend(packet, 0, packet.Length, 0, new AsyncCallback(SendCallback), client);
        //
        //StringBuilder sb = new StringBuilder(); // String Builder Create
        //sb.Append("Test 1 - By Mac!!");
        //sb.Append("Test 2 - By Mac!!");
        /*
        try
        {
            //=======================================================
            // Send.
            //SenddataLength = Encoding.Default.GetByteCount(sb.ToString());
            //Sendbyte = Encoding.Default.GetBytes(sb.ToString());


            //=======================================================       
            // Receive.
            
            m_Socket.Receive(Receivebyte);
            ReceiveString = Encoding.Default.GetString(Receivebyte);
            ReceivedataLength = Encoding.Default.GetByteCount(ReceiveString.ToString());
            Debug.Log("Receive Data : " + ReceiveString + "(" + ReceivedataLength + ")");
           
    }
        catch (SocketException err)
        {
            Debug.Log("Socket send or receive error! : " + err.ToString());
        }
         */
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

    void Send_POS(Vector3 Player, Vector3 Camera)
    {
        //var offset = fbb.CreateString("WindowsHyun"); // String 문자열이 있을경우 미리 생성해라.
        fbb.Clear(); // 클리어를 안해주고 시작하면 계속 누적해서 데이터가 들어간다.
        Client.StartClient(fbb);
        //Client.AddName(fbb, offset); // string 사용
        Client.AddPos(fbb, Vec3.CreateVec3(fbb, Player.x, Player.y, Player.z));
        Client.AddView(fbb, Vec3.CreateVec3(fbb, Camera.x, Camera.y, Camera.z));
        var endOffset = Client.EndClient(fbb);
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

    // Update is called once per frame
    void Update()
    {

        Player_Pos.x = Player.transform.position.x;
        Player_Pos.y = Player.transform.position.y;
        Player_Pos.z = Player.transform.position.z;
        Camera_Pos.x = Camera.transform.position.x;
        Camera_Pos.y = Camera.transform.position.y;
        Camera_Pos.z = Camera.transform.position.z;
        Send_POS(Player_Pos, Camera_Pos);



        m_Socket.Receive(Receivebyte);
        int psize = Receivebyte[0]; // 패킷 사이즈
        byte[] t_buf = new byte[psize + 1];
        System.Buffer.BlockCopy(Receivebyte, 4, t_buf, 0, psize); // 사이즈를 제외한 실제 패킷값을 복사한다.

        ByteBuffer revc_buf = new ByteBuffer(t_buf); // ByteBuffer로 byte[]로 복사한다.
        var Get_ServerData = Client.GetRootAsClient(revc_buf);

        OtherPlayer.transform.position = new Vector3(Get_ServerData.Pos.Value.X, Get_ServerData.Pos.Value.Y, Get_ServerData.Pos.Value.Z);


    }
}
