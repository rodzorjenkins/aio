using System.Text;
using BDProxy.Core;
using BDShared.Network.Model;

namespace BDProxy.Processors
{
    public class WorldProcessor
    {

        public static void CreateWorldNotice(string message)
        {
            using(var packet = new BDPacket())
            {
                packet.AddUShort(0);
                packet.AddBool(false);
                packet.AddUShort(0);
                packet.AddUShort(0xEAF);

                packet.AddByte(1);
                packet.AddByte(1);
                packet.AddInt(0); // session id?
                packet.AddString(string.Empty, Encoding.Unicode, 62); // character name
                packet.AddByte(1);
                packet.AddByte(1);
                packet.AddByte(1);

                byte[] bMessage = Encoding.Unicode.GetBytes(message);
                var len = bMessage.Length + 2;
                packet.AddUShort((ushort)len);
                packet.AddBytes(bMessage);
                packet.AddBytes("0000");

                packet.SetUShort(packet.Length, 0);

                MainContext.gameProxy.SendToGame(packet);
            }
        }

    }
}