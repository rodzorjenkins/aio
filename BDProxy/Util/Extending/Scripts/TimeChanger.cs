using BDProxy.Processors.Model;
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
            RegisterCommand("time", ProcessTimeCommand);
            base.Load();
        }

        private void ProcessTimeCommand(Command command)
        {
            if(command.Parameters.Count < 1)
                return;

            using(var packet = new BDPacket("2C00000000550D"))
            {
                string time = command.Parameters[0];
                if(time.Equals("day"))
                    packet.AddInt(20000);
                else if(time.Equals("night"))
                    packet.AddInt(200900);
                else
                    return;
                packet.AddBytes("000000000000000074BC2E200000000000A0AE5600000000000000000000000000");
                SendGamePacketToGame(packet);
            }
        }

        public override void Unload()
        {
            DeregisterCommand("time");
            base.Unload();
        }
        
    }
    
}