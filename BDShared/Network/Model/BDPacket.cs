using System;
using System.IO;
using System.Text;
using BDShared.Network.Cryptography;
using BDShared.Util;
using BDShared.Util.Attributes;

namespace BDShared.Network.Model
{
    [Developer("Johannes Jacobs")]
    public class BDPacket : IDisposable
    {

        public enum DumpLocation
        {
            Desktop,
            Custom
        }

        private MemoryStream memoryStream;
        private BinaryReader binaryReader;
        private BinaryWriter binaryWriter;

        public static BDPacket Empty { get { return null; } }

        public BDPacket()
        {
            memoryStream = new MemoryStream();
            binaryWriter = new BinaryWriter(memoryStream);
            binaryReader = new BinaryReader(memoryStream);
        }

        public BDPacket(byte[] buffer) : this()
        {
            binaryWriter.Write(buffer);
            memoryStream.Position = 0;
        }

        public BDPacket(string hex) : this()
        {
            binaryWriter.Write(hex.ToByteArray());
        }

        public string GetString(Encoding encoding, int len, int pos)
        {
            memoryStream.Position = pos;
            return encoding.GetString(binaryReader.ReadBytes(len));
        }

        public ushort GetUShort(int pos)
        {
            memoryStream.Position = pos;
            return binaryReader.ReadUInt16();
        }

        

        public ushort Length { get { return (ushort)memoryStream.Length; } }
        public byte[] ToArray() { return memoryStream.ToArray(); }
        public ushort PacketId
        {
            get { return BitConverter.ToUInt16(ToArray(), 5); }
            set { SetUShort(value, 5); }
        }
        public ushort SequenceId
        {
            get { return BitConverter.ToUInt16(ToArray(), 3); }
            set { SetUShort(value, 3); }
        }
        public bool IsEncrypted
        {
            get { return BitConverter.ToBoolean(ToArray(), 2); }
            set { SetBool(value, 2); }
        }

        public void SetUShort(ushort data, int pos)
        {
            if(Length < pos)
                throw new Exception("BDPacket object is not long enough.");

            memoryStream.Position = pos;
            AddUShort(data);
        }

        public void SetInt(int data, int pos)
        {
            if(Length < pos)
                throw new Exception("BDPacket object is not long enough.");

            memoryStream.Position = pos;
            AddInt(data);
        }

        public void SetString(string data, Encoding encoding, int pos)
        {
            if(Length < pos)
                throw new Exception("BDPacket object is not long enough.");

            memoryStream.Position = pos;
            AddString(data, encoding);
        }

        public void SetString(string data, Encoding encoding, int fixedLength, int pos)
        {
            if(Length < pos)
                throw new Exception("BDPacket object is not long enough.");

            memoryStream.Position = pos;
            AddString(data, encoding, fixedLength);
        }

        public void SetLong(long data, int pos)
        {
            if(Length < pos)
                throw new Exception("BDPacket object is not long enough.");

            memoryStream.Position = pos;
            AddLong(data);
        }

        public void SetShort(short data, int pos)
        {
            if(Length < pos)
                throw new Exception("BDPacket object is not long enough.");

            memoryStream.Position = pos;
            AddShort(data);
        }

        public void SetFloat(float data, int pos)
        {
            if(Length < pos)
                throw new Exception("BDPacket object is not long enough.");

            memoryStream.Position = pos;
            AddFloat(data);
        }

        public void SetBool(bool data, int pos)
        {
            if(Length < pos)
                throw new Exception("BDPacket object is not long enough.");

            memoryStream.Position = pos;
            AddBool(data);
        }

        public void SetByte(byte data, int pos)
        {
            if(Length < pos)
                throw new Exception("BDPacket object is not long enough.");

            memoryStream.Position = pos;
            AddByte(data);
        }

        public void SetBytes(byte[] data, int pos)
        {
            if(Length < pos)
                throw new Exception("BDPacket object is not long enough.");

            memoryStream.Position = pos;
            AddBytes(data);
        }

        public void AddFloat(float data)
            => binaryWriter.Write(data);

        public void AddUShort(ushort data)
            => binaryWriter.Write(data);

        public void AddInt(int data)
            => binaryWriter.Write(data);

        public void AddString(string data, Encoding encoding)
            => binaryWriter.Write(encoding.GetBytes(data));

        public void AddString(string data, Encoding encoding, int fixedLength)
        {
            byte[] fixedArray = new byte[fixedLength];
            byte[] dataArray = encoding.GetBytes(data);
            Buffer.BlockCopy(dataArray, 0, fixedArray, 0, dataArray.Length);
            binaryWriter.Write(fixedArray);
        }

        public void AddLong(long data)
            => binaryWriter.Write(data);

        public void AddShort(short data)
            => binaryWriter.Write(data);

        public void AddBool(bool data)
            => binaryWriter.Write(data);

        public void AddByte(byte data)
            => binaryWriter.Write(data);

        public void AddBytes(byte[] data)
            => binaryWriter.Write(data);

        public void AddBytes(string hex)
            => binaryWriter.Write(hex.ToByteArray());

        public void SetBytes(string hex, int pos)
        {
            if(Length < pos)
                throw new Exception("BDPacket object is not long enough.");

            memoryStream.Position = pos;
            AddBytes(hex);
        }

        public byte GetByte(int pos)
        {
            memoryStream.Position = pos;
            return binaryReader.ReadByte();
        }

        public int GetInt(int pos)
        {
            memoryStream.Position = pos;
            return binaryReader.ReadInt32();
        }

        public long GetLong(int pos)
        {
            memoryStream.Position = pos;
            return binaryReader.ReadInt64();
        }

        public float GetFloat(int pos)
        {
            memoryStream.Position = pos;
            return binaryReader.ReadSingle();
        }

        public void Transform(ref BDTransformer transformer, bool encrypt)
        {
            if(!IsEncrypted)
                return;

            if(transformer == null)
                throw new ArgumentNullException("transformer");

            byte[] bufferToTransform = ToArray().Extract(2);

            transformer.Transform(ref bufferToTransform, 0, !encrypt);

            memoryStream.Position = 2;
            binaryWriter.Write(bufferToTransform);
        }

        public void CreateFileDump(DumpLocation dumpLocation = DumpLocation.Desktop, string location = null)
        {
            if(dumpLocation == DumpLocation.Desktop)
            {
                if(location != null)
                    File.WriteAllBytes(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), location, string.Format("0x{0:X4}.bin", PacketId)), ToArray());
                else
                    File.WriteAllBytes(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), string.Format("0x{0:X4}.bin", PacketId)), ToArray());
            }
            else
                File.WriteAllBytes(Path.Combine(location, string.Format("0x{0:X4}.bin", PacketId)), ToArray());
        }

        public void CreateConsoleDump()
        {
            Console.WriteLine(ToArray().FormatHex());
        }

        public void Dispose()
        {
            binaryWriter.Dispose();
            binaryReader.Dispose();
            memoryStream.Dispose();
        }
    }
}