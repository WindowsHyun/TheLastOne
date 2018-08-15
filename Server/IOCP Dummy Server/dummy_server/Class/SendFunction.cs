using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlatBuffers;
using Game.TheLastOne;
using TheLastOne.GameClass;

namespace TheLastOne.SendFunction
{
    public class SendFunction : Game_ProtocolClass
    {
        Random r = new Random();
        public Byte[] makeClient_PacketInfo(Vector3 Player, int Player_Animator, float horizontal, float vertical, int Player_Weapone)
        {
            //var offset = fbb.CreateString("WindowsHyun"); // String 문자열이 있을경우 미리 생성해라.
            FlatBufferBuilder fbb = new FlatBufferBuilder(1);
            fbb.Clear(); // 클리어를 안해주고 시작하면 계속 누적해서 데이터가 들어간다.
            var fbbNickName = fbb.CreateString("AI_Bot" + r.Next(1, 1000).ToString()); // String 문자열이 있을경우 미리 생성해라.
            Client_info.StartClient_info(fbb);
            Client_info.AddAnimator(fbb, Player_Animator);  // 0 살아 있음, 1 죽음
            Client_info.AddHorizontal(fbb, horizontal);
            Client_info.AddVertical(fbb, vertical);
            Client_info.AddInCar(fbb, -1);
            Client_info.AddName(fbb, fbbNickName);
            Client_info.AddCarrotation(fbb, Vec3.CreateVec3(fbb, 0, 0, 0));
            Client_info.AddCarkmh(fbb, 0);
            Client_info.AddPosition(fbb, Vec3.CreateVec3(fbb, Player.x, Player.y, Player.z));
            Client_info.AddRotation(fbb, Vec3.CreateVec3(fbb, 0, 0, 0));
            Client_info.AddDangerLineIn(fbb, true);
            Client_info.AddNowWeapon(fbb, Player_Weapone);
            Client_info.AddPlayerDie(fbb, false);
            Client_info.AddCostumNum(fbb, r.Next(0, 14));
            var endOffset = Client_info.EndClient_info(fbb);
            fbb.Finish(endOffset.Value);


            byte[] packet = fbb.SizedByteArray();   // flatbuffers 실제 패킷 데이터
            byte[] magic_packet = makePacketinfo(packet.Length, CS_Info);
            byte[] real_packet = new byte[packet.Length + 8];
            System.Buffer.BlockCopy(magic_packet, 0, real_packet, 0, magic_packet.Length);
            System.Buffer.BlockCopy(packet, 0, real_packet, 8, packet.Length);

            return real_packet;
        }

        public Byte[] makeShot_PacketInfo(int client)
        {
            FlatBufferBuilder fbb = new FlatBufferBuilder(1);
            //var offset = fbb.CreateString("WindowsHyun"); // String 문자열이 있을경우 미리 생성해라.
            fbb.Clear(); // 클리어를 안해주고 시작하면 계속 누적해서 데이터가 들어간다.
            Client_Packet.StartClient_Packet(fbb);
            Client_Packet.AddId(fbb, client);
            var endOffset = Client_Packet.EndClient_Packet(fbb);
            fbb.Finish(endOffset.Value);

            byte[] packet = fbb.SizedByteArray();   // flatbuffers 실제 패킷 데이터
            byte[] magic_packet = makePacketinfo(packet.Length, CS_Shot_info);
            byte[] real_packet = new byte[packet.Length + 8];
            System.Buffer.BlockCopy(magic_packet, 0, real_packet, 0, magic_packet.Length);
            System.Buffer.BlockCopy(packet, 0, real_packet, 8, packet.Length);
            return real_packet;
        }

        public Byte[] makePlayer_Status(int status, int mapType)
        {
            FlatBufferBuilder fbb = new FlatBufferBuilder(1);
            fbb.Clear(); // 클리어를 안해주고 시작하면 계속 누적해서 데이터가 들어간다.
            Client_Status.StartClient_Status(fbb);
            Client_Status.AddStatus(fbb, status);
            Client_Status.AddMapType(fbb, mapType);
            var endOffset = Client_Status.EndClient_Status(fbb);
            fbb.Finish(endOffset.Value);

            byte[] packet = fbb.SizedByteArray();   // flatbuffers 실제 패킷 데이터
            byte[] magic_packet = makePacketinfo(packet.Length, CS_Player_Status);
            byte[] real_packet = new byte[packet.Length + 8];
            System.Buffer.BlockCopy(magic_packet, 0, real_packet, 0, magic_packet.Length);
            System.Buffer.BlockCopy(packet, 0, real_packet, 8, packet.Length);
            return real_packet;
        }

        public Byte[] makePacketinfo(int p_size, int type)
        {
            byte[] intBytes = new byte[p_size.ToString().Length + 2];   // 숫자 길이 + | + type
            byte[] cpy_size = Encoding.UTF8.GetBytes(p_size.ToString());
            byte[] cpy_type = Encoding.UTF8.GetBytes(type.ToString());

            for (int i = 0; i < p_size.ToString().Length; ++i)
            {
                intBytes[i] = cpy_size[i];
                //intBytes[i] -= 48;
            }
            intBytes[p_size.ToString().Length] = 124;
            if (type >= 10)
            {
                // 10보다 큰 경우 앞 숫자가 아닌 뒷 숫자로 판별한다.
                intBytes[p_size.ToString().Length + 1] = cpy_type[1];
                intBytes[p_size.ToString().Length + 1] -= 38;
            }
            else
            {
                intBytes[p_size.ToString().Length + 1] = cpy_type[0];
                intBytes[p_size.ToString().Length + 1] -= 48;
            }
            // 48을 마이너스 해준 이유는 Server에서 packet[i]로 형 변환 없이 바로 값을 확인하기 위하여
            return intBytes;
        }

    }
}
