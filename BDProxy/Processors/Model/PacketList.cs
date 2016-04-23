using System.Collections.Generic;

namespace BDProxy.Processors.Model
{
    public class PacketList
    {

        public class Packet
        {
            public string Name { get; set; }
            public ushort Id { get; set; }
        }

        public List<Packet> Packets { get; set; }

    }
}