using BDProxy.Core;
using BDProxy.Processors;
using BDProxy.Processors.Model;
using BDShared.Network.Model;
using BDShared.Util;

namespace BDProxy.Util.Extending
{
    public abstract class Script
    {

        /// <summary>
        /// Gets a value indicating the name of this script.
        /// </summary>
        public abstract string Name { get; }
        
        /// <summary>
        /// Occurs whenever this script gets loaded.
        /// </summary>
        public virtual void Load()
        {
            Logger.Log(Name, "has been loaded.", Logger.LogLevel.Script);
        }

        public virtual void Tick() { }

        /// <summary>
        /// Registers a chat command to the CommandProcessor.
        /// </summary>
        /// <param name="name">The command to register.</param>
        /// <returns>Whether the registration of the command was successful or not.</returns>
        public bool RegisterCommand(string name, Command.CommandExecutedCallback callback)
        {
            return CommandProcessor.Register(name, callback);
        }

        /// <summary>
        /// Deregisters a chat command from the CommandProcessor.
        /// </summary>
        /// <param name="name">The command to deregister.</param>
        /// <returns>Whether the deregistration of the command was successful or not.</returns>
        public bool DeregisterCommand(string name)
        {
            return CommandProcessor.Deregister(name);
        }

        /// <summary>
        /// Gets a value indicating whether the player is ingame.
        /// </summary>
        public bool IsPlayerIngame { get { return MainContext.IsPlayerIngame; } }


        /// <summary>
        /// Emulates a loginserver packet to the game.
        /// </summary>
        /// <param name="packet"></param>
        public void SendLoginPacketToGame(BDPacket packet)
        {
            MainContext.loginProxy.SendToGame(packet);
        }

        /// <summary>
        /// Emulates a loginserver packet to official server.
        /// </summary>
        /// <param name="packet"></param>
        public void SendLoginPacketToServer(BDPacket packet)
        {
            MainContext.loginProxy.SendToServer(packet);
        }

        /// <summary>
        /// Emulates a gameserver packet to the game.
        /// </summary>
        /// <param name="packet"></param>
        public void SendGamePacketToGame(BDPacket packet)
        {
            MainContext.gameProxy.SendToGame(packet);
        }

        /// <summary>
        /// Emulates a gameserver packet to official server.
        /// </summary>
        /// <param name="packet"></param>
        public void SendGamePacketToServer(BDPacket packet)
        {
            MainContext.gameProxy.SendToServer(packet);
        }

        /// <summary>
        /// Occurs when receiving a login packet from official server.
        /// </summary>
        /// <param name="packet"></param>
        /// <returns>The modified packet and redirects it to the game.</returns>
        public virtual BDPacket Login_SMSG(BDPacket packet)
        {
            return packet;
        }

        /// <summary>
        /// Occurs when receiving a login packet from game.
        /// </summary>
        /// <param name="packet"></param>
        /// <returns>The modified packet and redirects it to official server.</returns>
        public virtual BDPacket Login_CMSG(BDPacket packet)
        {
            return packet;
        }

        /// <summary>
        /// Occurs when receiving a game packet from official server.
        /// </summary>
        /// <param name="packet"></param>
        /// <returns>The modified packet and redirects it to the game.</returns>
        public virtual BDPacket Game_SMSG(BDPacket packet)
        {
            return packet;
        }

        /// <summary>
        /// Occurs when receiving a game packet from game.
        /// </summary>
        /// <param name="packet"></param>
        /// <returns>The modified packet and redirects it to official server.</returns>
        public virtual BDPacket Game_CMSG(BDPacket packet)
        {
            return packet;
        }

        /// <summary>
        /// Occurs whenever this script gets unloaded.
        /// </summary>
        public virtual void Unload()
        {
            Logger.Log(Name, "has been unloaded.", Logger.LogLevel.Script);
        }

    }
}