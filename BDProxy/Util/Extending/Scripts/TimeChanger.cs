using System;
using System.Linq;
using BDProxy.Processors;
using BDShared.Network.Model;

namespace BDProxy.Util.Extending
{

    public class TimeChanger : Script
    {
       

        public override string Name
        {
            get
            {
                return "TimeChanger";
            }
        }

        public override void Load()
        {
            base.Load();
        }

        public override BDPacket Game_CMSG(BDPacket packet)
        {
            if(PacketProcessor.ClientMessages.Packets.Any(t => t.Id == packet.PacketId))
                Console.WriteLine("{0}", PacketProcessor.ClientMessages.Packets.Where(t => t.Id == packet.PacketId).First().Name);

            return base.Game_CMSG(packet);
        }

        public override BDPacket Game_SMSG(BDPacket packet)
        {

            if(PacketProcessor.ServerMessages.Packets.Any(t => t.Id == packet.PacketId))
                Console.WriteLine("{0}", PacketProcessor.ServerMessages.Packets.Where(t => t.Id == packet.PacketId).First().Name);

            return base.Game_SMSG(packet);
        }

        public override void Unload()
        {
            base.Unload();
        }
        
    }
    
}