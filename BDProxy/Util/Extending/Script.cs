using BDProxy.Network;
using BDProxy.Processors;
using BDProxy.Processors.Model;
using BDShared.Network.Model;

namespace BDProxy.Util.Extending
{
    public abstract class Script
    {
        
        public virtual void Load(TcpProxy[] proxies) { }

        public virtual void Tick() { }

        public bool RegisterCommand(string name, Command.CommandExecutedCallback callback)
        {
            return CommandProcessor.Register(name, callback);
        }

        public virtual BDPacket Login_SMSG(BDPacket packet)
        {
            return packet;
        }

        public virtual BDPacket Login_CMSG(BDPacket packet)
        {
            return packet;
        }

        public virtual BDPacket Game_SMSG(BDPacket packet)
        {
            return packet;
        }

        public virtual BDPacket Game_CMSG(BDPacket packet)
        {
            return packet;
        }

        public virtual void Unload() { }

    }
}