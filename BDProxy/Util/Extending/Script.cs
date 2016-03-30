using BDProxy.Core;
using BDProxy.Processors;
using BDProxy.Processors.Model;
using BDShared.Network.Model;

namespace BDProxy.Util.Extending
{
    public abstract class Script
    {
        
        public virtual void Load() { }

        public virtual void Tick() { }

        public bool RegisterCommand(string name, Command.CommandExecutedCallback callback)
        {
            return CommandProcessor.Register(name, callback);
        }

        public bool DeregisterCommand(string name)
        {
            return CommandProcessor.Deregister(name);
        }

        public void SendLoginPacketToGame(BDPacket packet)
        {
            MainContext.loginProxy.SendToGame(packet);
        }

        public void SendLoginPacketToServer(BDPacket packet)
        {
            MainContext.loginProxy.SendToServer(packet);
        }

        public void SendGamePacketToGame(BDPacket packet)
        {
            MainContext.gameProxy.SendToGame(packet);
        }

        public void SendGamePacketToServer(BDPacket packet)
        {
            MainContext.gameProxy.SendToServer(packet);
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