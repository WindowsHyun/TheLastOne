using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using dummy_server;

namespace TheLastOne.NetworkClass
{
    public class m_Network
    {
        // Client  socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 7000;
        // Receive buffer.
        public byte[] Receivebyte = new byte[BufferSize];
        public byte[] Sendbyte = new byte[BufferSize];
        public int client_id = -1;
        public NetworkMessage msg = new NetworkMessage();
        public NetworkMessage new_msg = new NetworkMessage();
    }

    public class NetworkMessage
    {
        public int LimitReceivebyte = 7000;                     // Receive Data Length. (byte)
        public byte[] Receivebyte = new byte[7000];    // Receive data by this array to save.
        public byte[] Sendbyte = new byte[7000];
        public int now_packet_size = 0;
        public int prev_packet_size = 0;
        public StringBuilder sb = new StringBuilder();

        public void set_prev(int value)
        {
            this.prev_packet_size = value;
        }
    };

    public class PacketData
    {
        public int p_size = 0;
        public int type_Pos = 0;
        public PacketData(int size, int pos)
        {
            this.p_size = size;
            this.type_Pos = pos;
        }
    };
}
