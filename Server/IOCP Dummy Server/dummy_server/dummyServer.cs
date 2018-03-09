using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Net;
using System.Net.Sockets;
using System.Threading; // ManualResetEvent
using System.Runtime.InteropServices; // sizeof
using FlatBuffers;
using Game.TheLastOne; // FlatBuffers 필드 추가.


/*
  더미 캐릭터 예상 위치
  x : 622, y : 30, z : 1100
 */

namespace dummy_server
{
    public partial class dummyServer : Form
    {
        public string iPAdress = "127.0.0.1";
        public const int kPort = 9000;
        private static int Client_imei = 0;         // 자신의 클라이언트 아이디

        private const int MaxClient = 48;    // 최대 동접자수
        public static Client_Data[] client_data = new Client_Data[MaxClient];      // 클라이언트 데이터 저장할 구조체

        private int LimitReceivebyte = 2000;

        Random r = new Random();

        // 서버가 클라이언트에게 보내는 이벤트 타입
        private int SC_ID = 1;                          // 클라이언트 아이디를 보낸다.
        private int SC_PUT_PLAYER = 2;            // 클라이언트 추가
        private int SC_REMOVE_PLAYER = 3;     // 클라이언트 삭제
        private int SC_Client_Data = 4;		        // 클라이언트 모든 데이터

        private int CS_Info = 1;              // 클라이언트가 서버에게 자신의 위치정보를 보내준다.
        private int CS_Shot_info = 2;       // 클라이언트가 서버에게 Shot 정보를 보내준다.

        public dummyServer()
        {
            InitializeComponent();
        }

        delegate void SetTextCallback(string text);

        private void SetText(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.DebugBox.Text += text + "\n";
            }
        }

        private void connect_Server_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < MaxClient; ++i)
            {
                StateObject state = new StateObject();
                //=======================================================
                // Socket create.
                state.workSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                state.workSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 10000);
                state.workSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 10000);
                //=======================================================
                // Socket connect.
                try
                {
                    IPAddress ipAddr = System.Net.IPAddress.Parse(iPAdress);
                    IPEndPoint ipEndPoint = new System.Net.IPEndPoint(ipAddr, kPort);
                    state.workSocket.Connect(ipEndPoint);
                    state.client_id = i;
                    client_data[i].id = i;

                    client_data[i].position.x = r.Next(500, 1200);
                    client_data[i].position.y = 30;
                    client_data[i].position.z = r.Next(900, 1800);

                    client_data[i].rotation.x = 0;
                    client_data[i].rotation.y = 0;
                    client_data[i].rotation.z = 0;

                    client_data[i].stateObject = state;

                    state.workSocket.BeginReceive(state.Receivebyte, 0, LimitReceivebyte, 0, DataReceived, state);
                    Thread t1 = new Thread(() => Update(i - 1));
                    t1.Start();
                    //StartCoroutine(startPrefab());
                }
                catch (SocketException SCE)
                {
                    DebugBox.Text += "Socket connect error! : " + SCE.ToString() + "\n";
                    return;
                }
                //=======================================================


            }

        }

        void Update(int ci)
        {
            int animation = 0;
            while (true)
            {
                switch (r.Next(0, 5))
                {
                    case 0:
                        // 위로
                        animation = 3;
                        client_data[ci].position.x += Int32.Parse(MovePos.Text);
                        client_data[ci].position.z += Int32.Parse(MovePos.Text);
                        break;
                    case 1:
                        // 아래로
                        animation = 4;
                        client_data[ci].position.x -= Int32.Parse(MovePos.Text);
                        client_data[ci].position.z -= Int32.Parse(MovePos.Text);
                        break;
                    case 2:
                        // 왼쪽
                        animation = 5;
                        client_data[ci].position.x -= Int32.Parse(MovePos.Text);
                        break;
                    case 3:
                        // 오른쪽
                        animation = 6;
                        client_data[ci].position.x += Int32.Parse(MovePos.Text);
                        break;
                    case 4:
                        // 총알 발사!
                        client_data[ci].stateObject.Sendbyte = makeShot_PacketInfo(ci);
                        Send_Packet(client_data[ci].stateObject.workSocket, client_data[ci].stateObject.Sendbyte);
                        break;
                }

                client_data[ci].stateObject.Sendbyte = makeClient_PacketInfo(client_data[ci].position, animation, client_data[ci].rotation);
                Send_Packet(client_data[ci].stateObject.workSocket, client_data[ci].stateObject.Sendbyte);
                Thread.Sleep(1000);
            }
        }

        public void Send_Packet(Socket m_Socket, byte[] packet)
        {
            try
            {
                m_Socket.Send(packet, packet.Length, 0);
            }
            catch (SocketException err)
            {
                SetText("Socket send or receive error! : " + err.ToString());
            }
        }

        void DataReceived(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            //-------------------------------------------------------------------------------------
            /*
             C++ itoa를 통한 char로 넣은것을 for문을 통하여 컨버팅 하여 가져온다.
             124는 C++에서 '|'값 이다.
             str_size로 실제 패킷 값을 계산해서 넣는다.
             */
            try
            {
                string str_size = "";
                string tmp_int = "";
                byte[] temp = new byte[8];
                int type_Pos = 0;

                for (type_Pos = 0; type_Pos < 8; ++type_Pos)
                {
                    if (state.Receivebyte[type_Pos] == 124)
                        break;
                    temp[0] = state.Receivebyte[type_Pos];
                    tmp_int = Encoding.Default.GetString(temp);
                    str_size += Int32.Parse(tmp_int);
                }
                //-------------------------------------------------------------------------------------

                int psize = Int32.Parse(str_size);
                int ptype = state.Receivebyte[type_Pos + 1]; // 패킷 타입
                                                             //SetText("총 사이즈 : " + psize + ", 패킷 타입 : " + ptype);
                if (psize == state.workSocket.EndReceive(ar))
                {
                    ProcessPacket(psize, ptype, state.Receivebyte);
                }
            }
            catch
            {

            }
            state.workSocket.BeginReceive(state.Receivebyte, 0, LimitReceivebyte, 0, DataReceived, state);
        }

        void ProcessPacket(int size, int type, byte[] recvPacket)
        {
            if (type == SC_ID)
            {
                // 클라이언트 아이디를 가져온다.
                byte[] t_buf = new byte[size + 1];
                System.Buffer.BlockCopy(recvPacket, 8, t_buf, 0, size); // 사이즈를 제외한 실제 패킷값을 복사한다.
                ByteBuffer revc_buf = new ByteBuffer(t_buf); // ByteBuffer로 byte[]로 복사한다.
                var Get_ServerData = Client_id.GetRootAsClient_id(revc_buf);
                Client_imei = Int32.Parse(Get_ServerData.Id.ToString());
                SetText("클라이언트 아이디 : " + Client_imei);
            }

        }

        private void dummyServer_Load(object sender, EventArgs e)
        {
            connectCount.Text = MaxClient.ToString();
        }



        public Byte[] makeClient_PacketInfo(Vector3 Player, int Player_Animator, Vector3 PlayerRotation)
        {
            FlatBufferBuilder fbb = new FlatBufferBuilder(1);
            //var offset = fbb.CreateString("WindowsHyun"); // String 문자열이 있을경우 미리 생성해라.
            fbb.Clear(); // 클리어를 안해주고 시작하면 계속 누적해서 데이터가 들어간다.
            Client_info.StartClient_info(fbb);
            //Client.AddName(fbb, offset); // string 사용
            Client_info.AddAnimator(fbb, Player_Animator);
            Client_info.AddPosition(fbb, Vec3.CreateVec3(fbb, Player.x, Player.y, Player.z));
            Client_info.AddRotation(fbb, Vec3.CreateVec3(fbb, PlayerRotation.x, PlayerRotation.y, PlayerRotation.z));
            var endOffset = Client_info.EndClient_info(fbb);
            fbb.Finish(endOffset.Value);


            byte[] packet = fbb.SizedByteArray();   // flatbuffers 실제 패킷 데이터
            byte[] packet_len = BitConverter.GetBytes(packet.Length);   // flatbuffers의 패킷 크기
            byte[] packet_type = BitConverter.GetBytes(CS_Info);
            byte[] real_packet = new byte[packet_len.Length + packet.Length];

            System.Buffer.BlockCopy(packet_len, 0, real_packet, 0, packet_len.Length);
            System.Buffer.BlockCopy(packet_type, 0, real_packet, 1, packet_type.Length);
            System.Buffer.BlockCopy(packet, 0, real_packet, 4, packet.Length);
            return real_packet;
        }

        public Byte[] makeShot_PacketInfo(int client)
        {
            FlatBufferBuilder fbb = new FlatBufferBuilder(1);
            //var offset = fbb.CreateString("WindowsHyun"); // String 문자열이 있을경우 미리 생성해라.
            fbb.Clear(); // 클리어를 안해주고 시작하면 계속 누적해서 데이터가 들어간다.
            Client_Shot_info.StartClient_Shot_info(fbb);
            Client_Shot_info.AddId(fbb, client);
            var endOffset = Client_Shot_info.EndClient_Shot_info(fbb);
            fbb.Finish(endOffset.Value);


            byte[] packet = fbb.SizedByteArray();   // flatbuffers 실제 패킷 데이터
            byte[] packet_len = BitConverter.GetBytes(packet.Length);   // flatbuffers의 패킷 크기
            byte[] packet_type = BitConverter.GetBytes(CS_Shot_info);
            byte[] real_packet = new byte[packet_len.Length + packet.Length];

            System.Buffer.BlockCopy(packet_len, 0, real_packet, 0, packet_len.Length);
            System.Buffer.BlockCopy(packet_type, 0, real_packet, 1, packet_type.Length);
            System.Buffer.BlockCopy(packet, 0, real_packet, 4, packet.Length);
            return real_packet;
        }

        private void disconnect_Server_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < MaxClient; ++i)
            {
                client_data[i].stateObject.workSocket.Close();
                client_data[i].stateObject.workSocket = null;
            }
        }
    }
}

public struct Vector3
{
    public float x;
    public float y;
    public float z;
    public Vector3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
}

public class StateObject
{
    // Client  socket.
    public Socket workSocket = null;
    // Size of receive buffer.
    public const int BufferSize = 2000;
    // Receive buffer.
    public byte[] Receivebyte = new byte[BufferSize];
    public byte[] Sendbyte = new byte[BufferSize];

    public int client_id = -1;

}

public struct Client_Data
{
    public StateObject stateObject;
    public int id;
    public Vector3 position;  // 클라이언트 위치
    public Vector3 rotation;    // 클라이언트 보는 방향
    public bool prefab;    // 클라이언트 프리팹이 만들어졌는지 확인
    public bool connect;    // 클라이언트 접속
    public string name;     // 클라이언트 닉네임

    public Client_Data(Vector3 position, Vector3 rotation)
    {
        this.id = -1;
        this.position = position;
        this.rotation = rotation;
        this.connect = false;
        this.prefab = false;
        this.name = "";
        this.stateObject = null;
    }
}
