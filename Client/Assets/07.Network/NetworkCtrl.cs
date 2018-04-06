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
using TheLastOne.SendFunction;
using UnityEngine.UI;   // DebugText를 쓰기 위하여
//---------------------------------------------------------------

public struct Client_Data
{
    public int id;
    public int animator;    // 클라이언트 애니메이션
    public Vector3 position;  // 클라이언트 위치
    public Vector3 rotation;    // 클라이언트 보는 방향
    public bool prefab;    // 클라이언트 프리팹이 만들어졌는지 확인
    public bool connect;    // 클라이언트 접속
    public string name;     // 클라이언트 닉네임
    public GameObject Player;   // 프리팹을 위한 게임 오브젝트
    public OtherPlayerCtrl script;  // 프리팹 오브젝트 안의 함수를 호출하기 위한 스크립트
    public bool removeClient;   // 클라이언트 지울경우 true

    public Client_Data(Vector3 position, Vector3 rotation)
    {
        this.id = -1;
        this.animator = 0;
        this.position = position;
        this.rotation = rotation;
        this.connect = false;
        this.prefab = false;
        this.Player = null;
        this.name = "";
        this.script = null;
        this.removeClient = false;
    }
}

public struct item_Collection
{
    public int id;
    public string name;
    public Vector3 pos;
    public bool eat;
    public bool draw;
    public GameObject item;
    public item_Collection(int id, string name, Vector3 pos, bool eat)
    {
        this.id = id;
        this.name = name;
        this.pos = pos;
        this.eat = eat;
        this.draw = false;
        this.item = null;
    }
};

namespace TheLastOne.Game.Network
{
    public class NetworkCtrl : MonoBehaviour
    {
        public static Socket m_Socket;
        public GameObject Player;
        public GameObject PrefabPlayer;
        //------------------------------------------
        // 게임 아이템 Object
        public GameObject item_AK47;
        public GameObject item_M16;
        public GameObject item_556;
        public GameObject item_762;
        public GameObject item_AidKit;
        //------------------------------------------
        public Text DebugText;

        Vector3 Player_Position;
        Vector3 Player_Rotation;

        public string iPAdress = "127.0.0.1";
        public const int kPort = 9000;

        private const int MaxClient = 50;    // 최대 동접자수
        private const int MaxViewItem = 50;    // 최대 볼 수 있는 아이템
        public static Client_Data[] client_data = new Client_Data[MaxClient];      // 클라이언트 데이터 저장할 구조체
        public static item_Collection[] item_Collection = new item_Collection[MaxViewItem];
        /*
         Dictionary, Hashtable을 사용하려 하였지만, 
         Dic의 경우 Value 값을 수정하려면 임시 변수에 복사를 한후 수정하고 다시 복사를 해줘야 한다.
         결국 복사하는 시간이 발생하여 오히려 오버헤드가 일어 날 수 있다.
         Hashtable의 경우 Value값을 미리 지정을 하지 않고 자유롭게 넣을 수 있는 장점이 있으나,
         실제 Value 값을 수정을 하기 위해서는 foreach 혹은 Client_Data를 변환 하여서 가져오고 다시 대입을 해줘야 한다.
         미리 지정된 방식이 없으므로, Client_Data 구조체 내부를 가져올 수가 없다.
         결국 서버에서 클라이언트 고유번호를 처리 하는 방식으로 생각을 해야 겠다.
        */
        private static int Client_imei = -1;         // 자신의 클라이언트 아이디

        Socket_SendFunction sF = new Socket_SendFunction();

        private int LimitReceivebyte = 4000;                     // Receive Data Length. (byte)
        private byte[] Receivebyte = new byte[4000];    // Receive data by this array to save.
        private byte[] Sendbyte = new byte[4000];
        private string debugString = "";        // Debug 출력을 위한 string

        // 서버 연결을 했는지 체크
        private static bool serverConnect = false;

        // 서버가 클라이언트에게 보내는 이벤트 타입
        private int SC_ID = 1;                          // 클라이언트 아이디를 보낸다.
        private int SC_PUT_PLAYER = 2;            // 클라이언트 추가
        private int SC_REMOVE_PLAYER = 3;     // 클라이언트 삭제
        private int SC_Client_Data = 4;             // 클라이언트 모든 데이터
        private int SC_Server_Time = 5;             // 서버 타이머
        private int SC_Server_Item = 6;             // 서버 아이템

        IEnumerator startPrefab()
        {
            do
            {
                if (Client_imei != -1)
                {
                    for (int i = 0; i < MaxClient; ++i)
                    {
                        if (Client_imei == i)
                            continue;
                        if (client_data[i].connect == true && client_data[i].prefab == false)
                        {
                            client_data[i].Player = Instantiate(PrefabPlayer, client_data[i].position, Quaternion.identity);
                            //client_data[i].script = GameObject.Find("OtherPlayerCtrl").GetComponent<OtherPlayerCtrl>();
                            client_data[i].script = client_data[i].Player.GetComponent<OtherPlayerCtrl>();
                            client_data[i].prefab = true;

                            // 처음 위치를 넣어 줘야 한다. 그러지 않을경우 다른 클라이언트 에서는 0,0 에서부터 천천히 올라오게 보인다.
                            client_data[i].Player.transform.position = client_data[i].position;
                        }
                        else if (client_data[i].prefab == true)
                        {
                            // 실제로 캐릭터를 움직이는 것은 코루틴 여기서 움직임을 진행 한다.
                            client_data[i].script.MovePos(client_data[i].position);

                            var rotationX = client_data[i].rotation.x;
                            var rotationY = client_data[i].rotation.y;
                            var rotationZ = client_data[i].rotation.z;
                            //Debug.Log("  :  "+ rotationX + ", " + rotationY + ", " + rotationZ);
                            client_data[i].Player.transform.rotation = Quaternion.Euler(rotationX, rotationY, rotationZ);
                        }
                        if (client_data[i].removeClient == true)
                        {
                            // 연결은 해지 되었는데 프리팹이 살아 있는경우는 나간 경우 이므로
                            // 코루틴을 통하여 지워주자!
                            StartCoroutine(RemovePlayerCoroutine(i));
                        }
                    }
                }
                yield return null;
            } while (true);
            //yield return null;
        }

        IEnumerator drawItems()
        {
            do
            {
                if (Client_imei != -1)
                {
                    for (int i = 0; i < MaxViewItem; ++i)
                    {
                        if (item_Collection[i].draw == false)
                        {
                            // draw가 안되어 있을 경우
                            if (item_Collection[i].name == "AK47")
                            {
                                item_Collection[i].item = Instantiate(item_AK47, item_Collection[i].pos, Quaternion.identity);
                                item_Collection[i].draw = true;
                            }
                            if (item_Collection[i].name == "M16")
                            {
                                item_Collection[i].item = Instantiate(item_M16, item_Collection[i].pos, Quaternion.identity);
                                item_Collection[i].draw = true;
                            }
                            if (item_Collection[i].name == "556")
                            {
                                item_Collection[i].item = Instantiate(item_556, item_Collection[i].pos, Quaternion.identity);
                                item_Collection[i].item.transform.rotation = Quaternion.Euler(-90, 0, 0);
                                item_Collection[i].draw = true;
                            }
                            if (item_Collection[i].name == "762")
                            {
                                item_Collection[i].item = Instantiate(item_762, item_Collection[i].pos, Quaternion.identity);
                                item_Collection[i].item.transform.rotation = Quaternion.Euler(-90, 0, 0);
                                item_Collection[i].draw = true;
                            }

                        }

                        //if (item_Collection[i].draw == true && item_Collection[i].item.activeInHierarchy == false)
                        //{
                        //    // 이미 그려진 상태에서 아이템이 먹어졌을 경우, 서버로 아이템을 먹었다고 보내야 한다.
                        //    //Sendbyte = sF.makeEatItem_PacketInfo(item_Collection[i].id);
                        //    //Send_Packet(Sendbyte);
                        //}

                    }
                }
                yield return new WaitForSeconds(0.1f);
            } while (true);
            //yield return null;
        }

        IEnumerator RecvCoroutine()
        {
            do
            {
                m_Socket.BeginReceive(Receivebyte, 0, LimitReceivebyte, 0, DataReceived, m_Socket);
                yield return null;
            } while (true);
            //yield return null;
        }

        IEnumerator SendCoroutine()
        {
            do
            {
                if (Client_imei != -1)
                {
                    Player_Position.x = Player.transform.position.x;
                    Player_Position.y = Player.transform.position.y;
                    Player_Position.z = Player.transform.position.z;
                    Player_Rotation.x = Player.transform.eulerAngles.x;
                    Player_Rotation.y = Player.transform.eulerAngles.y;
                    Player_Rotation.z = Player.transform.eulerAngles.z;
                    Enum get_int_enum = Player.GetComponent<PlayerCtrl>().playerState;
                    Sendbyte = sF.makeClient_PacketInfo(Player_Position, Convert.ToInt32(get_int_enum), Player_Rotation);
                    Send_Packet(Sendbyte);
                    yield return new WaitForSeconds(0.05f);
                    // 초당 20번 패킷 전송으로 제한을 한다.
                }
            } while (true);
            //yield return null;
        }

        IEnumerator RemovePlayerCoroutine(int client_id)
        {
            // 클라이언트 데이터 및 프리팹을 삭제 하여 다른 클라이언트가 들어올 수 있게 만든다.
            //Debug.Log(client_id + "삭제 코루틴 접근");
            client_data[client_id].connect = false;
            client_data[client_id].removeClient = false;
            client_data[client_id].prefab = false;
            client_data[client_id].position = new Vector3(0, 0, 0);
            client_data[client_id].rotation = new Vector3(0, 0, 0);
            Destroy(client_data[client_id].Player);
            client_data[client_id].Player = null;
            client_data[client_id].script = null;
            client_data[client_id].id = -1;
            yield return null;
        }

        IEnumerator DrawDebugText()
        {
            do
            {

                DebugText.text = debugString.ToString();
                yield return null;
            } while (true);
            //yield return null;
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
                debugString = "Client ID :" + Client_imei + "/";
                //----------------------------------------------------------------
                // 클라이언트 아이디가 정상적으로 받은건지 확인을 한다.
                // 버그로 인하여 일단 임시로 나둔다.
                //Sendbyte = sF.check_ClientIMEI(Int32.Parse(Get_ServerData.Id.ToString()));
                //Send_Packet(Sendbyte);
                //----------------------------------------------------------------
                Debug.Log("클라이언트 아이디 : " + Client_imei);
            }
            else if (type == SC_PUT_PLAYER)
            {
                // 클라이언트 하나에 대한 데이터가 들어온다.
                byte[] t_buf = new byte[size + 1];
                System.Buffer.BlockCopy(recvPacket, 8, t_buf, 0, size); // 사이즈를 제외한 실제 패킷값을 복사한다.
                ByteBuffer revc_buf = new ByteBuffer(t_buf); // ByteBuffer로 byte[]로 복사한다.

                var data = new Offset<Client_info>[MaxClient];
                var Get_ServerData = Client_info.GetRootAsClient_info(revc_buf);

                // 클라이언트 데이터에 서버에서 받은 데이터를 넣어준다.
                client_data[Get_ServerData.Id].position = new Vector3(Get_ServerData.Position.Value.X, Get_ServerData.Position.Value.Y, Get_ServerData.Position.Value.Z);
                client_data[Get_ServerData.Id].rotation = new Vector3(Get_ServerData.Rotation.Value.X, Get_ServerData.Rotation.Value.Y, Get_ServerData.Rotation.Value.Z);
                client_data[Get_ServerData.Id].name = Get_ServerData.Name;

                if (client_data[Get_ServerData.Id].connect != true && Client_imei != Get_ServerData.Id)
                {
                    // 클라이언트가 처음 들어와서 프리팹이 없을경우 
                    client_data[Get_ServerData.Id].connect = true;
                }
            }
            else if (type == SC_Client_Data)
            {
                // 클라이언트 모든 데이터가 들어온다.
                byte[] t_buf = new byte[size + 1];
                System.Buffer.BlockCopy(recvPacket, 8, t_buf, 0, size); // 사이즈를 제외한 실제 패킷값을 복사한다.
                ByteBuffer revc_buf = new ByteBuffer(t_buf); // ByteBuffer로 byte[]로 복사한다.
                var Get_ServerData = All_information.GetRootAsAll_information(revc_buf);

                // 서버에서 받은 데이터 묶음을 확인하여 묶음 수 만큼 추가해준다.
                for (int i = 0; i < Get_ServerData.DataLength; i++)
                {
                    // 클라이언트 데이터에 서버에서 받은 데이터를 넣어준다.
                    var client_id = Get_ServerData.Data(i).Value.Id;
                    var position_x = Get_ServerData.Data(i).Value.Position.Value.X;
                    var position_y = Get_ServerData.Data(i).Value.Position.Value.Y;
                    var position_z = Get_ServerData.Data(i).Value.Position.Value.Z;
                    client_data[client_id].position = new Vector3(position_x, position_y, position_z);
                    // 캐릭터 이동 속도 변수

                    var rotation_x = Get_ServerData.Data(i).Value.Rotation.Value.X;
                    var rotation_y = Get_ServerData.Data(i).Value.Rotation.Value.Y;
                    var rotation_z = Get_ServerData.Data(i).Value.Rotation.Value.Z;
                    client_data[client_id].rotation = new Vector3(rotation_x, rotation_y, rotation_z);

                    client_data[client_id].name = Get_ServerData.Data(i).Value.Name;


                    if (client_data[client_id].prefab == true)
                    {
                        // 프리팹이 만들어진 이후 부터 script를 사용할 수 있기 때문에 그 이후 애니메이션 동기화를 시작한다.
                        client_data[client_id].script.get_Animator(Get_ServerData.Data(i).Value.Animator);
                    }

                    if (Get_ServerData.Data(i).Value.Shotting == true && i != Client_imei)
                    {
                        // 자신이 아닌 다른 클라이언트가 총을 쏘면 해당 클라이언트의 script에 Fire을 호출한다.
                        Debug.Log(client_id + "가 총을 쏘다.");
                        // 자신의 클라이언트 위치를 넘겨 준다.
                        client_data[client_id].script.Fire(client_data[Client_imei].position);
                    }

                    if (client_data[client_id].connect != true && Client_imei != client_id)
                    {
                        // 클라이언트가 처음 들어와서 프리팹이 없을경우 
                        client_data[client_id].connect = true;
                    }

                }
            }
            else if (type == SC_REMOVE_PLAYER)
            {
                // 서버에서 내보낸 클라이언트를 가져 온다.
                byte[] t_buf = new byte[size + 1];
                System.Buffer.BlockCopy(recvPacket, 8, t_buf, 0, size); // 사이즈를 제외한 실제 패킷값을 복사한다.
                ByteBuffer revc_buf = new ByteBuffer(t_buf); // ByteBuffer로 byte[]로 복사한다.
                var Get_ServerData = Client_id.GetRootAsClient_id(revc_buf);

                client_data[Get_ServerData.Id].removeClient = true;
                //Debug.Log(Get_ServerData.Id + "번 클라이언트 삭제..!");

            }
            else if (type == SC_Server_Time)
            {
                byte[] t_buf = new byte[size + 1];
                System.Buffer.BlockCopy(recvPacket, 8, t_buf, 0, size); // 사이즈를 제외한 실제 패킷값을 복사한다.
                ByteBuffer revc_buf = new ByteBuffer(t_buf); // ByteBuffer로 byte[]로 복사한다.
                var Get_ServerData = Game_Timer.GetRootAsGame_Timer(revc_buf);
                //Debug.Log("Time : " + Get_ServerData.Time);
                debugString = "Time : " + Get_ServerData.Time;
            }
            else if (type == SC_Server_Item)
            {
                // 서버 아이템 관련...
                byte[] t_buf = new byte[size + 1];
                System.Buffer.BlockCopy(recvPacket, 8, t_buf, 0, size); // 사이즈를 제외한 실제 패킷값을 복사한다.
                ByteBuffer revc_buf = new ByteBuffer(t_buf); // ByteBuffer로 byte[]로 복사한다.

                var Get_ServerData = Game_Items.GetRootAsGame_Items(revc_buf);

                for (int i = 0; i < Get_ServerData.DataLength; i++)
                {
                    Vector3 pos;
                    pos.x = Get_ServerData.Data(i).Value.X;
                    pos.y = 29.99451f;
                    pos.z = Get_ServerData.Data(i).Value.Z;

                    item_Collection[i].id = Get_ServerData.Data(i).Value.Id;
                    item_Collection[i].name = Get_ServerData.Data(i).Value.Name.ToString();
                    item_Collection[i].eat = Get_ServerData.Data(i).Value.Eat;
                    item_Collection[i].pos = pos;
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
            //DebugText.transform.position = new Vector3(Screen.width / 2, Screen.height - 10, 0);

            //=======================================================
            // Socket connect.
            try
            {
                IPAddress ipAddr = System.Net.IPAddress.Parse(iPAdress);
                IPEndPoint ipEndPoint = new System.Net.IPEndPoint(ipAddr, kPort);
                serverConnect = true;
                m_Socket.Connect(ipEndPoint);
                m_Socket.BeginReceive(Receivebyte, 0, LimitReceivebyte, 0, DataReceived, m_Socket);
                StartCoroutine(startPrefab());
                StartCoroutine(RecvCoroutine());
                StartCoroutine(SendCoroutine());
                StartCoroutine(DrawDebugText());
                StartCoroutine(drawItems());
            }
            catch (SocketException SCE)
            {
                //Debug.Log();
                debugString = "Socket connect error! : " + SCE.ToString();
                return;
            }

            //=======================================================
        }

        void DataReceived(IAsyncResult ar)
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

            int psize = Int32.Parse(str_size);
            int ptype = Receivebyte[type_Pos + 1]; // 패킷 타입
            //Debug.Log("총 사이즈 : " + psize + ", 패킷 타입 : " + ptype);

            try
            {
                if (psize == m_Socket.EndReceive(ar))
                {
                    ProcessPacket(psize, ptype, Receivebyte);
                }
                else
                {
                    Debug.Log(m_Socket.EndReceive(ar) + "패킷 Error | " + psize);
                }
            }
            catch
            {
                //debugString = "패킷 오류..!";
            }

        }

        public void Send_Packet(byte[] packet)
        {
            if (serverConnect == true)
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
        }

        public void Player_Shot()
        {
            Sendbyte = sF.makeShot_PacketInfo(Client_imei);
            Send_Packet(Sendbyte);
        }

        void OnApplicationQuit()
        {
            m_Socket.Close();
            m_Socket = null;
        }

        public float DistanceToPoint(Vector3 a, Vector3 b)
        {
            // 캐릭터 간의 거리 구하기.
            return (float)Math.Sqrt(Math.Pow(a.x - b.x, 2) + Math.Pow(a.z - b.z, 2));
        }


    }
}