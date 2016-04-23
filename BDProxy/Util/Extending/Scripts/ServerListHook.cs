using System;
using System.IO;
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

                p.AddString(Config.GetValue<string>("ChannelName"), Encoding.Unicode, 62); // channel name
                p.AddString(Config.GetValue<string>("ServerName"), Encoding.Unicode, 62); // server name

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


            //if(packet.PacketId == 0xc82)
            //{

            //    ushort realmCount = packet.GetUShort(19);

            //    Console.WriteLine(realmCount);
            //    int index = 21;

            //    string edan="";
            //    string orwen ="";
            //    string uno="";
            //    string croxus="";
            //    string jordine ="";
            //    string alustin="";
            //    for(int i = 0; i < realmCount; i++)
            //    {
            //        string channelName = packet.GetString(Encoding.Unicode, 62, index + 6);
            //        channelName = channelName.Split('\0')[0];

            //        string servername = packet.GetString(Encoding.Unicode, 62, index + 6 + 62);
            //        servername = servername.Split('\0')[0];

            //        string ip = packet.GetString(Encoding.ASCII, 16, index + 6 + 62 + 62 + 1);
            //        ip = ip.Split('\0')[0];

            //        Console.WriteLine(channelName + " - " + servername + " - " + ip);

            //        if(servername=="Edan")
            //        {
            //            edan += string.Format("Channels.Add(new KeyValuePair<string, string>(\"{0}\", \"{1}\"));{2}", channelName, ip, Environment.NewLine);
            //        }
            //        if(servername == "Orwen")
            //        {
            //            orwen += string.Format("Channels.Add(new KeyValuePair<string, string>(\"{0}\", \"{1}\"));{2}", channelName, ip, Environment.NewLine);
            //        }
            //        if(servername == "Uno")
            //        {
            //            uno += string.Format("Channels.Add(new KeyValuePair<string, string>(\"{0}\", \"{1}\"));{2}", channelName, ip, Environment.NewLine);
            //        }

            //        if(servername == "Croxus")
            //        {
            //            croxus += string.Format("Channels.Add(new KeyValuePair<string, string>(\"{0}\", \"{1}\"));{2}", channelName, ip, Environment.NewLine);
            //        }
            //        if(servername == "Jordine")
            //        {
            //            jordine += string.Format("Channels.Add(new KeyValuePair<string, string>(\"{0}\", \"{1}\"));{2}", channelName, ip, Environment.NewLine);
            //        }
            //        if(servername == "Alustin")
            //        {
            //            alustin += string.Format("Channels.Add(new KeyValuePair<string, string>(\"{0}\", \"{1}\"));{2}", channelName, ip, Environment.NewLine);
            //        }

            //        index += 272;
            //    }

            //    File.WriteAllText("C:\\Users\\Johannes\\Desktop\\Edan.txt", edan);
            //    File.WriteAllText("C:\\Users\\Johannes\\Desktop\\Orwen.txt", orwen);
            //    File.WriteAllText("C:\\Users\\Johannes\\Desktop\\Uno.txt", uno);
            //    File.WriteAllText("C:\\Users\\Johannes\\Desktop\\Croxus.txt", croxus);
            //    File.WriteAllText("C:\\Users\\Johannes\\Desktop\\Jordine.txt", jordine);
            //    File.WriteAllText("C:\\Users\\Johannes\\Desktop\\Alustin.txt", alustin);
            //}


            return packet;
        }

        public override void Unload()
        {
            base.Unload();
        }
        
    }
    
}