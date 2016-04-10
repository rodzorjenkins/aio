using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using BDProxy.Core;
using BDProxy.Network;
using BDProxy.Processors;
using BDProxy.Processors.Model;
using BDProxy.Util.Extending;
using BDShared.Network.Model;
using BDShared.Util;

namespace BDProxy
{
    class Program
    {

        private ScriptController scriptController;

        static void Main(string[] args)
        {
            Console.Title = string.Format("BDProxy | v{0}", Assembly.GetExecutingAssembly().GetName().Version);
            new Program();
        }        

        private void TitleArt()
        {
            Console.WriteLine(@" ____  ____      ____  ____   __  _  _  _  _ ");
            Console.WriteLine(@"(  _ \(    \ ___(  _ \(  _ \ /  \( \/ )( \/ )");
            Console.WriteLine(@" ) _ ( ) D ((___)) __/ )   /(  O ))  (  )  / ");
            Console.WriteLine(@"(____/(____/    (__)  (__\_) \__/(_/\_)(__/  ");
            Console.WriteLine();
            Console.WriteLine("] Coded by Johannes Jacobs");
        }

        private void SetupCommands()
        {
            CommandProcessor.Register("reload", reloadScriptsCallback);
        }

        private void reloadScriptsCallback(Command command)
        {
            if(scriptController == null)
                return;

            scriptController.Scripts.ForEach(t => t.Unload());
            scriptController.LoadScripts();
            scriptController.Scripts.ForEach(t => t.Load());

            WorldProcessor.CreateWorldNotice("[ScriptController] Scripts have been reloaded.");
        }

        public Program()
        {
            TitleArt();            
            SetupCommands();

            scriptController = new ScriptController("../../Util/Extending/Scripts/");
            scriptController.LoadScripts();
            scriptController.Scripts.ForEach(t => t.Load());

            StartServers();

            while(true)
            {
                

                Thread.Sleep(1);
            }

            scriptController.Scripts.ForEach(t => t.Unload());
            scriptController.UnloadScripts();

            Console.ReadLine();
        }

        private void StartServers()
        {
            MainContext.loginProxy = new TcpProxy();
            MainContext.loginProxy.ServerListening += LoginProxy_ServerListening;
            MainContext.loginProxy.ConnectionAccepted += LoginProxy_ConnectionAccepted;
            MainContext.loginProxy.GameToServerPacket += LoginProxy_GameToServerPacket;
            MainContext.loginProxy.ServerToGamePacket += LoginProxy_ServerToGamePacket;
            MainContext.loginProxy.StartServer(new IPEndPoint(IPAddress.Any, 8888));

            MainContext.gameProxy = new TcpProxy();
            MainContext.gameProxy.ServerListening += GameProxy_ServerListening;
            MainContext.gameProxy.ConnectionAccepted += GameProxy_ConnectionAccepted;
            MainContext.gameProxy.GameToServerPacket += GameProxy_GameToServerPacket;
            MainContext.gameProxy.ServerToGamePacket += GameProxy_ServerToGamePacket;
            MainContext.gameProxy.StartServer(new IPEndPoint(IPAddress.Any, 8889));
        }


        #region "LoginProxy"
        private void LoginProxy_ServerToGamePacket(object sender, BDPacket e)
        {
            foreach(Script plugin in scriptController.Scripts)
                e = plugin.Login_SMSG(e);

            MainContext.loginProxy.SendToGame(e);
        }

        private void LoginProxy_GameToServerPacket(object sender, BDPacket e)
        {
            foreach(Script plugin in scriptController.Scripts)
                e = plugin.Login_CMSG(e);

            MainContext.loginProxy.SendToServer(e);
        }

        private void LoginProxy_ConnectionAccepted(object sender, EventArgs e)
        {
            Logger.Log("LoginProxy", "Game client has been accepted.", Logger.LogLevel.Normal);
            MainContext.loginProxy.ConnectClient(new IPEndPoint(IPAddress.Parse("37.48.82.146"), 8888));
        }

        private void LoginProxy_ServerListening(object sender, EventArgs e)
        {
            Logger.Log("LoginProxy", "Attempt to bind socket to 0.0.0.0:8888!", Logger.LogLevel.Info);
        }
        #endregion

        #region "GameProxy"
        private void GameProxy_ServerToGamePacket(object sender, BDPacket e)
        {
            foreach(Script plugin in scriptController.Scripts)
                e = plugin.Game_SMSG(e);
            
            MainContext.gameProxy.SendToGame(e);
        }

        private void GameProxy_GameToServerPacket(object sender, BDPacket e)
        {
            foreach(Script plugin in scriptController.Scripts)
                e = plugin.Game_CMSG(e);

            if(e.PacketId == 0xCEE)
                MainContext.IsPlayerIngame = true;
            
            if(e.PacketId == 0xEA8)
            {
                var messageLen = e.GetUShort(11) - 2;
                string message = e.GetString(Encoding.Unicode, messageLen, 13);

                if(message.StartsWith("/"))
                {
                    string commandName = message.Replace("/", "").Split(' ')[0];

                    List<Command> list = new List<Command>(CommandProcessor.GetCommandsByName(commandName));
                    if(list.Count > 0)
                    {
                        for(int i = 0; i < list.Count; i++)
                        {
                            var command = list[i];
                            string[] parameters = message.Replace("/", "").Split(' ');
                            command.Parameters.Clear();
                            for(int j = 1; j < parameters.Length; j++)
                                command.Parameters.Add(parameters[j]);
                            command.Callback(command);
                        }
                    }
                    else
                        WorldProcessor.CreateWorldNotice("[CommandProcessor] Command does not exist.");
                    return;
                }
            }
            MainContext.gameProxy.SendToServer(e);
        }

        private void GameProxy_ConnectionAccepted(object sender, EventArgs e)
        {
            Logger.Log("GameProxy", "Game client has been accepted.", Logger.LogLevel.Normal);
            MainContext.gameProxy.ConnectClient(new IPEndPoint(IPAddress.Parse("37.48.82.139"), 8889));
        }

        private void GameProxy_ServerListening(object sender, EventArgs e)
        {
            Logger.Log("GameProxy", "Attempt to bind socket to 0.0.0.0:8889!", Logger.LogLevel.Info);
        }
        #endregion


    }
}