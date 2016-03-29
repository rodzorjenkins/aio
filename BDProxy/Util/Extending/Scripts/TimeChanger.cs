using BDProxy.Network;
using BDProxy.Processors.Model;
using BDShared.Network.Model;
using BDShared.Util;

namespace BDProxy.Util.Extending
{

    public class TimeChanger : Script
    {

        private TcpProxy gameProxy;

        public override void Load(TcpProxy[] proxys)
        {
            gameProxy = proxys[1];
            RegisterCommand("time", ProcessTimeCommand);

            Logger.Log("TimeChanger", "has been loaded.");
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
                packet.AddBytes("000000000000000074BC2E200000000000A0AE5600000000000000000000000000");
                gameProxy.SendToGame(packet);
            }
        }

        public override void Unload()
        {
            Logger.Log("TimeChanger", "has been unloaded.");
        }
        
    }
    
}