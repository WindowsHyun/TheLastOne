using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using FlatBuffers;
using Game.TheLastOne; // Client, Vec3 을 불러오기 위해
using TheLastOne.GameClass;
//using TheLastOne.Game.Network;



namespace TheLastOne.SendFunction
{
    class Socket_SendFunction : Game_ProtocolClass
    {


        public Byte[] makeClient_PacketInfo(Vector3 Player, int Player_Animator, float horizontal, float vertical, Vector3 PlayerRotation, int Player_Weapone, int inCar, bool dangerLineIn, Vector3 CarRotation)
        {
            //var offset = fbb.CreateString("WindowsHyun"); // String 문자열이 있을경우 미리 생성해라.
            FlatBufferBuilder fbb = new FlatBufferBuilder(1);
            fbb.Clear(); // 클리어를 안해주고 시작하면 계속 누적해서 데이터가 들어간다.
            Client_info.StartClient_info(fbb);
            //Client.AddName(fbb, offset); // string 사용
            Client_info.AddAnimator(fbb, Player_Animator);
            Client_info.AddHorizontal(fbb, horizontal);
            Client_info.AddVertical(fbb, vertical);
            Client_info.AddInCar(fbb, inCar);
            Client_info.AddCarrotation(fbb, Vec3.CreateVec3(fbb, CarRotation.x, CarRotation.y, CarRotation.z));
            Client_info.AddPosition(fbb, Vec3.CreateVec3(fbb, Player.x, Player.y, Player.z));
            Client_info.AddRotation(fbb, Vec3.CreateVec3(fbb, PlayerRotation.x, PlayerRotation.y, PlayerRotation.z));
            Client_info.AddDangerLineIn(fbb, dangerLineIn);
            Client_info.AddNowWeapon(fbb, Player_Weapone);
            var endOffset = Client_info.EndClient_info(fbb);
            fbb.Finish(endOffset.Value);


            byte[] packet = fbb.SizedByteArray();   // flatbuffers 실제 패킷 데이터
            byte[] magic_packet = makePacketinfo(packet.Length, CS_Info);
            byte[] real_packet = new byte[packet.Length + 8];
            System.Buffer.BlockCopy(magic_packet, 0, real_packet, 0, magic_packet.Length);
            System.Buffer.BlockCopy(packet, 0, real_packet, 8, packet.Length);

            return real_packet;
        }

        public Byte[] makeZombie_PacketInfo(Dictionary<int, Game_ZombieClass> zombie_data, int client_imei)
        {
            FlatBufferBuilder fbb = new FlatBufferBuilder(1);
            fbb.Clear();
            //var target_zombie = new Offset<Zombie_info>[10];
            List<Offset<Zombie_info>> target_zombie = new List<Offset<Zombie_info>>();

            int num = 0;
            foreach (var key in zombie_data.Keys.ToList())
            {
                if (zombie_data[key].get_target() == client_imei && zombie_data[key].get_isDie() == false)
                {
                    // 좀비 Target과 Client_Imei가 같은경우에만 Vector에 넣는다.
                    Zombie_info.StartZombie_info(fbb);
                    Zombie_info.AddId(fbb, zombie_data[key].get_id());
                    Zombie_info.AddAnimator(fbb, zombie_data[key].get_animator());
                    Zombie_info.AddTargetPlayer(fbb, zombie_data[key].get_target());
                    Zombie_info.AddPosition(fbb, Vec3.CreateVec3(fbb, zombie_data[key].get_pos().x, zombie_data[key].get_pos().y, zombie_data[key].get_pos().z));
                    Zombie_info.AddRotation(fbb, Vec3.CreateVec3(fbb, zombie_data[key].get_rot().x, zombie_data[key].get_rot().y, zombie_data[key].get_rot().z));
                    target_zombie.Add(Zombie_info.EndZombie_info(fbb));
                    //target_zombie[num] = Zombie_info.EndZombie_info(fbb)
                    ++num;
                }
            }

            var send_zombie = new Offset<Zombie_info>[target_zombie.Count()];
            num = 0;
            foreach (var data in target_zombie)
            {
                send_zombie[num] = data;
                ++num;
            }

            var zombie_vector = Zombie_Collection.CreateDataVector(fbb, send_zombie);
            Zombie_Collection.StartZombie_Collection(fbb);
            Zombie_Collection.AddData(fbb, zombie_vector);
            var endOffset = Zombie_Collection.EndZombie_Collection(fbb);
            fbb.Finish(endOffset.Value);

            byte[] packet = fbb.SizedByteArray();   // flatbuffers 실제 패킷 데이터
            byte[] magic_packet = makePacketinfo(packet.Length, CS_Zombie_info);
            byte[] real_packet = new byte[packet.Length + 8];
            System.Buffer.BlockCopy(magic_packet, 0, real_packet, 0, magic_packet.Length);
            System.Buffer.BlockCopy(packet, 0, real_packet, 8, packet.Length);

            //Debug.Log(real_packet.Length);

            if (num != 0)
                return real_packet;
            else
                return null;

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

        public Byte[] check_ClientIMEI(int client)
        {
            FlatBufferBuilder fbb = new FlatBufferBuilder(1);
            //var offset = fbb.CreateString("WindowsHyun"); // String 문자열이 있을경우 미리 생성해라.
            fbb.Clear(); // 클리어를 안해주고 시작하면 계속 누적해서 데이터가 들어간다.
            Client_Packet.StartClient_Packet(fbb);
            Client_Packet.AddId(fbb, client);
            var endOffset = Client_Packet.EndClient_Packet(fbb);
            fbb.Finish(endOffset.Value);

            byte[] packet = fbb.SizedByteArray();   // flatbuffers 실제 패킷 데이터
            byte[] magic_packet = makePacketinfo(packet.Length, CS_Check_info);
            byte[] real_packet = new byte[packet.Length + 8];
            System.Buffer.BlockCopy(magic_packet, 0, real_packet, 0, magic_packet.Length);
            System.Buffer.BlockCopy(packet, 0, real_packet, 8, packet.Length);
            return real_packet;
        }

        public Byte[] makeEatItem_PacketInfo(int item_num)
        {
            FlatBufferBuilder fbb = new FlatBufferBuilder(1);
            //var offset = fbb.CreateString("WindowsHyun"); // String 문자열이 있을경우 미리 생성해라.
            fbb.Clear(); // 클리어를 안해주고 시작하면 계속 누적해서 데이터가 들어간다.
            Client_Packet.StartClient_Packet(fbb);
            // 아이템 뿌리기 테스트를 위하여 먹어도 안먹었다고 보내기로 한다./
            Client_Packet.AddId(fbb, item_num);
            var endOffset = Client_Packet.EndClient_Packet(fbb);
            fbb.Finish(endOffset.Value);

            byte[] packet = fbb.SizedByteArray();   // flatbuffers 실제 패킷 데이터
            byte[] magic_packet = makePacketinfo(packet.Length, CS_Eat_Item);
            byte[] real_packet = new byte[packet.Length + 8];
            System.Buffer.BlockCopy(magic_packet, 0, real_packet, 0, magic_packet.Length);
            System.Buffer.BlockCopy(packet, 0, real_packet, 8, packet.Length);
            return real_packet;
        }

        public Byte[] makeHP_PacketInfo(int id, int hp, int armour, int kind)
        {
            FlatBufferBuilder fbb = new FlatBufferBuilder(1);
            fbb.Clear(); // 클리어를 안해주고 시작하면 계속 누적해서 데이터가 들어간다.
            Game_HP_Set.StartGame_HP_Set(fbb);
            Game_HP_Set.AddId(fbb, id);
            Game_HP_Set.AddHp(fbb, hp);
            Game_HP_Set.AddArmour(fbb, armour);
            Game_HP_Set.AddKind(fbb, kind);
            var endOffset = Game_HP_Set.EndGame_HP_Set(fbb);
            fbb.Finish(endOffset.Value);

            byte[] packet = fbb.SizedByteArray();   // flatbuffers 실제 패킷 데이터
            byte[] magic_packet = makePacketinfo(packet.Length, CS_Object_HP);
            byte[] real_packet = new byte[packet.Length + 8];
            System.Buffer.BlockCopy(magic_packet, 0, real_packet, 0, magic_packet.Length);
            System.Buffer.BlockCopy(packet, 0, real_packet, 8, packet.Length);
            return real_packet;
        }

        public Byte[] makeCar_Status(int id, bool value)
        {
            FlatBufferBuilder fbb = new FlatBufferBuilder(1);
            fbb.Clear(); // 클리어를 안해주고 시작하면 계속 누적해서 데이터가 들어간다.
            Client_Packet.StartClient_Packet(fbb);
            Client_Packet.AddId(fbb, id);
            var endOffset = Client_Packet.EndClient_Packet(fbb);
            fbb.Finish(endOffset.Value);

            byte[] packet = fbb.SizedByteArray();   // flatbuffers 실제 패킷 데이터
            int sc_type;
            if (value)
                sc_type = CS_Car_Riding;    // 차량 탑습
            else
                sc_type = CS_Car_Rode;      // 차량 하차
            byte[] magic_packet = makePacketinfo(packet.Length, sc_type);
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
            intBytes[p_size.ToString().Length + 1] = cpy_type[0];
            intBytes[p_size.ToString().Length + 1] -= 48;
            // 48을 마이너스 해준 이유는 Server에서 packet[i]로 형 변환 없이 바로 값을 확인하기 위하여

            return intBytes;
        }

        public Byte[] makePlayer_Status(int status)
        {
            FlatBufferBuilder fbb = new FlatBufferBuilder(1);
            fbb.Clear(); // 클리어를 안해주고 시작하면 계속 누적해서 데이터가 들어간다.
            Client_Packet.StartClient_Packet(fbb);
            Client_Packet.AddId(fbb, status);
            var endOffset = Client_Packet.EndClient_Packet(fbb);
            fbb.Finish(endOffset.Value);

            byte[] packet = fbb.SizedByteArray();   // flatbuffers 실제 패킷 데이터
            byte[] magic_packet = makePacketinfo(packet.Length, CS_Player_Status);
            byte[] real_packet = new byte[packet.Length + 8];
            System.Buffer.BlockCopy(magic_packet, 0, real_packet, 0, magic_packet.Length);
            System.Buffer.BlockCopy(packet, 0, real_packet, 8, packet.Length);
            return real_packet;
        }

    }
}
