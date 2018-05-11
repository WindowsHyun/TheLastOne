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
using TheLastOne.GameClass;
using TheLastOne.NetworkClass;
using TheLastOne.SendFunction;

namespace dummy_server
{
    public partial class dummyServer : Form
    {
        public string iPAdress = "127.0.0.1";
        public const int kPort = 9000;

        private byte[] Sendbyte = new byte[7000];
        private int MaxClient = 35;

        public static Dictionary<int, Game_ClientClass> client_data = new Dictionary<int, Game_ClientClass>();
        // 클라이언트 데이터 저장할 컨테이너

        Random r = new Random();

        // 서버가 클라이언트에게 보내는 이벤트 타입
        SendFunction sF = new SendFunction();
        Game_ProtocolClass recv_protocol = new Game_ProtocolClass();

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

                m_Network t_network = new m_Network();
                // 0부터 최대 연결까지 돌린다.
                t_network.workSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                t_network.workSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
                t_network.workSocket.NoDelay = true;

                iPAdress = iPAdressText.Text.ToString();

                t_network.workSocket.BeginConnect(iPAdress, kPort, new AsyncCallback(ConnectCallback), t_network);

                this.DebugBox.Text += "클라이언트 생성 : " + i + "\n";
                Thread.Sleep(50);
            }

            foreach (var key in client_data.Keys.ToList())
            {
                // 레디 준비 완료.
                Send_Packet(client_data[key].m_Net ,sF.makePlayer_Status(2));
                Thread.Sleep(100);
            }
            connect_Server.Enabled = false;
        }

        void Update(int ci)
        {
            int h = 0;
            int v = 0;
            while (true)
            {
                switch (r.Next(0, 5))
                {
                    case 0:
                        // 위로
                        h = 0;
                        v = 1;
                        
                        client_data[ci].position.x += Int32.Parse(MovePos.Text);
                        client_data[ci].position.z += Int32.Parse(MovePos.Text);
                        break;
                    case 1:
                        // 아래로
                        h = 0;
                        v = -1;
                        client_data[ci].position.x -= Int32.Parse(MovePos.Text);
                        client_data[ci].position.z -= Int32.Parse(MovePos.Text);
                        break;
                    case 2:
                        // 왼쪽
                        h = -1;
                        v = 0;
                        client_data[ci].position.x -= Int32.Parse(MovePos.Text);
                        break;
                    case 3:
                        // 오른쪽
                        h = 1;
                        v = 0;
                        client_data[ci].position.x += Int32.Parse(MovePos.Text);
                        break;
                    case 4:
                        // 총알 발사!
                        // 더미 서버의 클라이언트와,  
                        //SetText(ci + "가 총을 발사 합니다.");
                        client_data[ci].m_Net.Sendbyte = sF.makeShot_PacketInfo(ci);
                        Send_Packet(client_data[ci].m_Net, client_data[ci].m_Net.Sendbyte);
                        break;
                }

                client_data[ci].m_Net.Sendbyte = sF.makeClient_PacketInfo(client_data[ci].position, 0, h, v, r.Next(1,4));
                Send_Packet(client_data[ci].m_Net, client_data[ci].m_Net.Sendbyte);
                Thread.Sleep(1024);
            }
        }


        public void Send_Packet(m_Network m_Net, byte[] packet)
        {
            if (m_Net.workSocket.Connected == true)
            {
                try
                {
                    m_Net.workSocket.Send(packet, packet.Length, 0);
                }
                catch (SocketException err)
                {
                    SetText("Socket send or receive error! : " + err.ToString());
                }
            }
        }

        private void dummyServer_Load(object sender, EventArgs e)
        {
            connectCount.Text = MaxClient.ToString();
        }

        public void ProcessPacket(int size, int type, byte[] recvPacket, m_Network m_Net)
        {

            if (type == recv_protocol.SC_ID)
            {
                // 클라이언트 아이디를 가져온다.
                byte[] t_buf = new byte[size + 1];
                System.Buffer.BlockCopy(recvPacket, 8, t_buf, 0, size); // 사이즈를 제외한 실제 패킷값을 복사한다.
                ByteBuffer revc_buf = new ByteBuffer(t_buf); // ByteBuffer로 byte[]로 복사한다.
                var Get_ServerData = Client_id.GetRootAsClient_id(revc_buf);
                //client_data[state.client_id].id = Int32.Parse(Get_ServerData.Id.ToString());

                client_data.Add(Get_ServerData.Id, new Game_ClientClass(Get_ServerData.Id, m_Net, new Vector3(r.Next(880, 1400), 30, r.Next(750, 1600))));


                System.Diagnostics.Debug.WriteLine("클라이언트 아이디 : " + Get_ServerData.Id.ToString());
                SetText("클라이언트 아이디 : " + Get_ServerData.Id.ToString());

                // 클라이언트 아이디를 여기서 제공해준다.
            }
            else if (type == recv_protocol.SC_Client_Data)
            {
                // 클라이언트 모든 데이터가 들어온다.
                byte[] t_buf = new byte[size + 1];
                System.Buffer.BlockCopy(recvPacket, 8, t_buf, 0, size); // 사이즈를 제외한 실제 패킷값을 복사한다.
                ByteBuffer revc_buf = new ByteBuffer(t_buf); // ByteBuffer로 byte[]로 복사한다.
                var Get_ServerData = Client_Collection.GetRootAsClient_Collection(revc_buf);

                // 서버에서 받은 데이터 묶음을 확인하여 묶음 수 만큼 추가해준다.
                for (int i = 0; i < Get_ServerData.DataLength; i++)
                {
                    if (client_data.ContainsKey(Get_ServerData.Data(i).Value.Id))
                    {
                        // 이미 값이 들어가 있는 상태라면
                        Game_ClientClass iter = client_data[Get_ServerData.Data(i).Value.Id];
                        iter.set_hp(Get_ServerData.Data(i).Value.Hp);
                    }
                }
            }

        }

        private void disconnect_Server_Click(object sender, EventArgs e)
        {
            foreach (var key in client_data.Keys.ToList())
            {
                client_data[key].m_Net.workSocket.Close();
                client_data[key].t1.Join();
                Thread.Sleep(50);
            }
            allReady.Enabled = true;
            connect_Server.Enabled = true;
            DebugBox.Text = "";
        }

        private void connectCountTimer_Tick(object sender, EventArgs e)
        {
            MaxClient = Int32.Parse(connectCount.Text.ToString());
        }

        public void ConnectCallback(IAsyncResult ar)
        {
            m_Network client = (m_Network)ar.AsyncState;
            // 서버가 정상적으로 연결이 되었을 경우.
            if (client.workSocket.Connected == true)
            {
                //Debug.Log("서버와 정상적으로 연결 되었습니다.");
                RecieveHeader(client);//start recieve header
            }
        }

        public void RecieveHeader(m_Network client)
        {

            client.workSocket.BeginReceive(client.msg.Receivebyte, 0, client.msg.LimitReceivebyte, SocketFlags.None, new AsyncCallback(RecieveHeaderCallback), client);
        }

        public PacketData Get_packet_size(byte[] Receivebyte)
        {
            //-------------------------------------------------------------------------------------
            /*
             C++ itoa를 통한 char로 넣은것을 for문을 통하여 컨버팅 하여 가져온다.
             124는 C++에서 '|'값 이다.
             str_size로 실제 패킷 값을 계산해서 넣는다.
             */
            string str_size = "";
            string tmp_int = "";
            byte[] temp = new byte[8];
            int type_Pos = 0;

            for (type_Pos = 0; type_Pos < 8; ++type_Pos)
            {
                if (Receivebyte[type_Pos] == 124)
                    break;
                temp[0] = Receivebyte[type_Pos];
                tmp_int = Encoding.Default.GetString(temp);
                str_size += Int32.Parse(tmp_int);
            }
            //-------------------------------------------------------------------------------------

            return new PacketData(Int32.Parse(str_size), type_Pos);
        }

        public void RecieveHeaderCallback(IAsyncResult ar)
        {
            try
            {
                m_Network client = (m_Network)ar.AsyncState;     // Recieve된 Packet을 받아온다.
                int bytesRead = client.workSocket.EndReceive(ar);        // 소켓에서 받아온 사이즈를 확인한다.

                PacketData size_data = Get_packet_size(client.msg.Receivebyte);

                int psize = size_data.p_size;
                int ptype = client.msg.Receivebyte[size_data.type_Pos + 1]; // 패킷 타입

                if (psize == bytesRead)
                {
                    // 소켓에서 받은 데이터와 실제 패킷 사이즈가 같을 경우
                    ProcessPacket(psize, ptype, client.msg.Receivebyte, client);

                    // 패킷 처리가 완료 되었으니 다시 리시브 상태로 돌아간다.
                    //NetworkMessage new_msg = new NetworkMessage();
                    client.workSocket.BeginReceive(client.new_msg.Receivebyte, 0, client.new_msg.LimitReceivebyte, SocketFlags.None, new AsyncCallback(RecieveHeaderCallback), client);
                }
                else
                {
                    // 소켓에서 받은 데이터와 실제 패킷 사이즈가 다를 경우
                    client.msg.sb.Append(Encoding.ASCII.GetString(client.msg.Receivebyte, 0, bytesRead));
                    client.msg.set_prev(bytesRead);
                    // 소켓에서 받은 데이터가 안맞는 경우 패킷이 뒤에 붙어서 오는거 같은 느낌이 든다...
                    size_data = Get_packet_size(client.msg.Receivebyte);
                    byte[] recv_byte = new byte[size_data.p_size + 9];

                    for (int i = 0; i < size_data.p_size; ++i)
                    {
                        recv_byte[i] = client.msg.Receivebyte[i];
                    }

                    ProcessPacket(psize, ptype, recv_byte, client);

                    client.workSocket.BeginReceive(client.msg.Receivebyte, 0, client.msg.LimitReceivebyte, SocketFlags.None, new AsyncCallback(RecieveHeaderCallback), client);
                }
            }
            catch
            {
                m_Network client = (m_Network)ar.AsyncState;
                if (client.workSocket.Connected != false)
                    client.workSocket.BeginReceive(client.new_msg.Receivebyte, 0, client.new_msg.LimitReceivebyte, SocketFlags.None, new AsyncCallback(RecieveHeaderCallback), client);
            }
        }

        private void allReady_Click(object sender, EventArgs e)
        {
            foreach (var key in client_data.Keys.ToList())
            {
                // 인게임 화면 완료.
                Send_Packet(client_data[key].m_Net, sF.makePlayer_Status(4));
                Thread.Sleep(100);

                client_data[key].t1 = new Thread(() => Update(key));
                client_data[key].t1.Start();
            }
           // allReady.Enabled = false;

        }
    }
}


