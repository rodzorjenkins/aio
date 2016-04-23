using System;
using System.IO;
using BDProxy.Processors.Model;
using BDShared.Util;
using Newtonsoft.Json;

namespace BDProxy.Processors
{
    internal class PacketProcessor
    {
        
        public static PacketList ClientMessages, ServerMessages;
        
        public static void LoadPackets()
        {
            LoadClientPackets();
            LoadServerPackets();
        }

        private static void LoadClientPackets()
        {
            if(!File.Exists("packets\\client.json"))
                return;

            try
            {
                ClientMessages = JsonConvert.DeserializeObject<PacketList>(File.ReadAllText("packets\\client.json"));

                Logger.Log("PacketProcessor", "Successfully loaded {0} client packets.", Logger.LogLevel.Config, ClientMessages.Packets.Count);
            }
            catch(Exception)
            {
                Logger.Log("PacketProcessor", "Error while parsing ClientMessages.", Logger.LogLevel.Error);
            }
        }

        private static void LoadServerPackets()
        {
            if(!File.Exists("packets\\server.json"))
                return;

            try
            {
                ServerMessages = JsonConvert.DeserializeObject<PacketList>(File.ReadAllText("packets\\server.json"));

                Logger.Log("PacketProcessor", "Successfully loaded {0} server packets.", Logger.LogLevel.Config, ClientMessages.Packets.Count);
            }
            catch(Exception)
            {
                Logger.Log("PacketProcessor", "Error while parsing ServerMessages.", Logger.LogLevel.Error);
            }
        }

    }
}