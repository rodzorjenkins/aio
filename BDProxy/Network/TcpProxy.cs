using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using BDShared.Network.Cryptography;
using BDShared.Network.Model;
using BDShared.Util;
using BDShared.Util.Attributes;

namespace BDProxy.Network
{

    [Developer("Johannes Jacobs")]
    public class TcpProxy
    {

        public event EventHandler ServerListening;
        public event EventHandler ConnectionAccepted;
        public event EventHandler<BDPacket> GameToServerPacket;
        public event EventHandler<BDPacket> ServerToGamePacket;
        public event EventHandler ClientConnected;

        private Socket serverSocket;
        private Socket connectionSocket;
        private Socket clientSocket;

        private BDTransformer cryptoTransformer;

        private byte[] SMSG_Buffer, CMSG_Buffer;

        public bool IsGameServerConnectionValid { get; set; }

        public TcpProxy()
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) { NoDelay = true };
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) { NoDelay = true };
            
            SMSG_Buffer = new byte[2];
            CMSG_Buffer = new byte[2];
        }

        public void SendToServer(BDPacket packet)
        {
            if(packet == null)
                return;

            if(packet.IsEncrypted)
                packet.Transform(ref cryptoTransformer, true);

            clientSocket.BeginSend(packet.ToArray(), 0, packet.Length, SocketFlags.None, ar =>
            {
                clientSocket.EndSend(ar);
            }, null);
        }

        public void SendToGame(BDPacket packet)
        {
            if(packet == null)
                return;

            if(packet.IsEncrypted)
                packet.Transform(ref cryptoTransformer, true);

            connectionSocket.BeginSend(packet.ToArray(), 0, packet.Length, SocketFlags.None, ar =>
            {
                connectionSocket.EndSend(ar);
            }, null);
        }

        public void StartServer(IPEndPoint endPoint)
        {
            try
            {
                if(!serverSocket.IsBound)
                {
                    serverSocket.Bind(endPoint);
                    serverSocket.Listen(1);
                    serverSocket.Blocking = false;

                    if(ServerListening != null) ServerListening(this, EventArgs.Empty);

                    serverSocket.BeginAccept(ar =>
                    {
                        connectionSocket = serverSocket.EndAccept(ar);
                        if(ConnectionAccepted != null) ConnectionAccepted(this, EventArgs.Empty);
                        connectionSocket.BeginReceive(CMSG_Buffer, 0, 2, SocketFlags.None, new AsyncCallback(connectionReceiveCallback), null);
                    } , null);
                }
            }
            catch(Exception e)
            {
                Logger.Log("Server", "{0}", Logger.LogLevel.Error, e);
            }
        }

        public void ConnectClient(IPEndPoint endPoint)
        {
            try
            {
                if(!clientSocket.Connected)
                {
                    clientSocket.BeginConnect(endPoint, ar => {
                        clientSocket.EndConnect(ar);

                        if(clientSocket.Connected)
                        {
                            if(ClientConnected != null) ClientConnected(this, EventArgs.Empty);
                            clientSocket.BeginReceive(SMSG_Buffer, 0, 2, SocketFlags.None, new AsyncCallback(serverReceivedCallback), null);
                        }
                    }, null);
                }
            }
            catch(Exception e)
            {
                Logger.Log("Client", "{0}", Logger.LogLevel.Error, e);
            }
        }

        private void connectionReceiveCallback(IAsyncResult ar)
        {
            try
            {
                int len = connectionSocket.EndReceive(ar);
                if(len != 2)
                    return;

                var needed = BitConverter.ToUInt16(CMSG_Buffer, 0);
                if(needed > 0)
                    ReceivePacket(connectionSocket, needed, false);
            }
            catch (Exception e)
            {
                Logger.Log("Server", "{0}", Logger.LogLevel.Error, e);
            }
        }

        private void serverReceivedCallback(IAsyncResult ar)
        {
            try
            {
                int len = clientSocket.EndReceive(ar);
                if(len != 2)
                    return;

                var needed = BitConverter.ToUInt16(SMSG_Buffer, 0);
                if(needed > 0)
                    ReceivePacket(clientSocket, needed, true);
            }
            catch(Exception e)
            {
                Logger.Log("Server", "{0}", Logger.LogLevel.Error, e);
            }
        }

        private async void ReceivePacket(Socket socket, int needed, bool SMSG)
        {
            await StartReceive(socket, needed, SMSG);
        }

        private async Task StartReceive(Socket socket, int needed, bool SMSG)
        {
            try
            {
                var buffer = new byte[needed];
                Buffer.BlockCopy(BitConverter.GetBytes(needed), 0, buffer, 0, 2);

                var bytesRead = 2;
                while(bytesRead < needed)
                {
                    var bytesReceived =
                        await ReceiveAsyncTask(socket, buffer, bytesRead, needed - bytesRead).ConfigureAwait(false);
                    bytesRead += bytesReceived;

                    if(bytesReceived <= 0)
                        break;
                }
                
                await Task.Factory.StartNew(() => 
                {
                    BDPacket packet = ProcessPacket(buffer);
                    if(SMSG)
                    {
                        if(ServerToGamePacket != null) ServerToGamePacket(this, packet);
                    }
                    else
                    {
                        if(GameToServerPacket != null) GameToServerPacket(this, packet);
                    }
                });
            }
            catch(Exception e)
            {
                Logger.Log("Server", "{0}", Logger.LogLevel.Error, e);
            }

            if(SMSG)
                socket.BeginReceive(SMSG_Buffer, 0, 2, SocketFlags.None, new AsyncCallback(serverReceivedCallback), null);
            else
                socket.BeginReceive(CMSG_Buffer, 0, 2, SocketFlags.None, new AsyncCallback(connectionReceiveCallback), null);
        }
        
        private BDPacket ProcessPacket(byte[] buffer)
        {
            var packet = new BDPacket(buffer);

            if(packet.PacketId == 0x03EB)
                cryptoTransformer = new BDTransformer(packet.ToArray().Extract(5));

            if(packet.IsEncrypted)
                packet.Transform(ref cryptoTransformer, false);

            return packet;
        }

        private Task<int> ReceiveAsyncTask(Socket con, byte[] destination, int offset, int length)
        {
            return Task<int>.Factory.FromAsync(
                con.BeginReceive(destination, offset, length, SocketFlags.None, null, con),
                con.EndReceive
            );
        }

    }
}