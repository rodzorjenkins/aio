using System.Threading;
using BDProxy.Processors.Model;
using BDShared.Network.Model;

namespace BDProxy.Util.Extending
{

    public class TimeChanger : Script
    {

        private System.Diagnostics.Stopwatch stopWatch;

        public override string Name
        {
            get
            {
                return "TimeChanger";
            }
        }

        public override void Load()
        {
            stopWatch = new System.Diagnostics.Stopwatch();

            RegisterCommand("time", ProcessTimeCommand);
            RegisterCommand("spawn", SpawnCommand);
            RegisterCommand("respawn", RespawnCommand);
            RegisterCommand("test", TestCommand);

            new Thread(Tick).Start();
            
            base.Load();
        }

        private void Tick()
        {
            while(IsScriptLoaded)
            {
                if(stopWatch.ElapsedMilliseconds >= 1)
                    gameTime++;
            }
        }

        private void TestCommand(Command command)
        {
            using(var packet = new BDPacket())
            {
                packet.AddBytes("7D00000000F10B00010364F7000000000000000000D0F205A301000000010000003400000000000000FFFFFFFFFFFFFFFF0000000000000000000100FF7FFF7F2E52FDE5E40D4700FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF0000000000000000000000000000000000000000000000000000000000");

                packet.SetInt(93, 33);

                SendGamePacketToGame(packet);
            }
        }

        private void RespawnCommand(Command command)
        {
            using(var packet = new BDPacket())
            {
                packet.AddBytes("0C00000000E80C0111000000");

                SendGamePacketToServer(packet);
            }
        }

        private void SpawnCommand(Command command)
        {
            byte monsterCount = 1;

            using(var packet = new BDPacket())
            {
                packet.AddUShort(0);
                packet.AddBool(false);
                packet.AddUShort(0);
                packet.AddUShort(0xBBA);

                packet.AddBytes("0100");
                packet.AddByte(monsterCount); // monster count

                for(int i = 0; i < monsterCount; i++)
                {
                    packet.AddInt(184835072); // session id
                    packet.AddFloat(x); // x
                    packet.AddFloat(z); // z
                    packet.AddFloat(y); // y
                    packet.AddFloat(0); // cosinus
                    packet.AddInt(0); // unk
                    packet.AddFloat(0); // sinus
                    packet.AddUShort(ushort.Parse(command.Parameters[0])); // npcid
                    packet.AddUShort(0); // npc group id
                    packet.AddFloat(100); // current hp
                    packet.AddFloat(100); // max hp
                    packet.AddLong(-88142); // cache id
                    packet.AddLong(0); // unk
                    packet.AddLong(-2097153); // cache id
                    packet.AddByte(1); // function id
                    packet.AddInt(633942026); // magickey

                    packet.AddBytes("0C000000000000000000000000000000030000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000003000000000000000000000000000000000000000000000000000000000000000010000000000000000000000000000000000000000000000000000000000000009000000000000000000000000000000000000000000000000000000000000006800000000000000000000000000000000000000000000000000000000000000D0000000000000000000000000000000000000000000000000000000007F000006000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000007F000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000025000000000000000000000000000000000000000000000000000000007F00000000000000000000000000000000000000000000000000000000000000000000CF000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000005000000000000000000000000000000000000000000000000000000007F00009200000000000000000000000000000000000000000000000000000000AC38C543000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000A000000000000000000000000000000000000000000000000000000000AC38C54300000000000000000000000000000000000000000000000000000000000000D0000000000000000000000000000000000000000000000000000000007F000001000000000000000000269026468F36C4C587259C47269026468F36C4C587259C470000000000000000000000000000000000002041000000000000000000000000000000000000000018B4FC0B000000003673141500000000000000000000557300FCFFFFB373141500000000B373141500000000D2731415000000000100E0FFFFFFFFFF0000000000000000000000");
                }

                packet.SetUShort(packet.Length, 0);
                SendGamePacketToGame(packet);
            }

            CreateWorldNotice("Spawned a monster");
        }

        float x,y,z;

        public override BDPacket Game_CMSG(BDPacket packet)
        {

            if(packet.PacketId == 0xbd6)
                packet.CreateFileDump();
            

            return packet;
        }

        int gameTime = 0;

        public override BDPacket Game_SMSG(BDPacket packet)
        {

            if(packet.PacketId == 0xd55)
            {
                gameTime = packet.GetInt(7);
                if(stopWatch.IsRunning)
                    stopWatch.Reset();
                stopWatch.Start();
            }

            if(packet.PacketId == 0x1072)
                packet.SetInt(0, 7);

            return packet;
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
            DeregisterCommand("spawn");
            DeregisterCommand("respawn");
            DeregisterCommand("test");
            base.Unload();
        }
        
    }
    
}