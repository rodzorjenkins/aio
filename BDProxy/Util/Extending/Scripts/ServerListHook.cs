using System.Text;
using BDShared.Network.Model;
using BDShared.Util;

namespace BDProxy.Util.Extending
{

    public class ServerListHook : Script
    {

        public override string Name
        {
            get
            {
                return "ServerListHook";
            }
        }

        public override void Load()
        {
            base.Load();
        }

        public override BDPacket Login_SMSG(BDPacket packet)
        {
            if(packet.PacketId == 0x0C82)
            {
                var p = new BDPacket();
                p.AddUShort(0); // changeto length later
                p.AddBool(true); // is encrypted
                p.AddUShort(packet.SequenceId);
                p.AddUShort(packet.PacketId);

                p.AddInt(0); // unk
                p.AddLong(1457907576); // time?

                p.AddShort(1); // server count


                p.AddShort(1); // channel id
                p.AddShort(1); // server id
                p.AddShort(16384); // ranodm value,

                p.AddString("BDProxy - Channel", Encoding.Unicode, 62); // channel name
                p.AddString("BDProxy - Server", Encoding.Unicode, 62); // server name

                p.AddByte(0);
                p.AddString("127.0.0.1", Encoding.ASCII, 16);
                p.AddByte(0);

                p.AddBytes(new byte[84]);

                p.AddShort(8889); // server port

                p.AddByte(1); // population status
                p.AddByte(1); // can joined by public?
                p.AddByte(1); // unk

                p.AddByte(0); // characters
                p.AddByte(0); // characters to be deleted lol

                p.AddShort(0); // dunno

                p.AddLong(0);
                p.AddLong(0);

                p.AddByte(0); // exp drop bonus?

                p.AddBytes(new byte[13]);

                p.AddByte(0);

                p.SetUShort(p.Length, 0);
                
                Logger.Log("ServerListHook", "modified server list packet!");

                return p;
            }

            return packet;
        }

        public override void Unload()
        {
            base.Unload();
        }
        
    }
    
}