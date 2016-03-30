﻿using System;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using BDProxy.Core;
using BDProxy.Network;
using BDProxy.Processors;
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

        public Program()
        {
            TitleArt();
            StartServers();

#if DEBUG
            scriptController = new ScriptController("../../Util/Extending/Scripts/");
#else
            scriptController = new ScriptController("scripts/");
#endif
            scriptController.LoadScripts();
            scriptController.Scripts.ForEach(t => t.Load());

            while(true)
            {
                new Thread(() =>
                {
                    while(true)
                    {
                        try
                        {
                            scriptController.Scripts.ForEach(t => t.Tick());
                        }
                        catch(Exception) { break; }
                    }
                }).Start();

                Thread.Sleep(1);

                string input = Console.ReadLine();
                if(input.Equals("reload"))
                {
                    scriptController.Scripts.ForEach(t => t.Unload());
                    scriptController.LoadScripts();
                    scriptController.Scripts.ForEach(t => t.Load());
                }
                else
                    break;
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

            if(e.PacketId == 0xEA8)
            {
                var messageLen = e.GetUShort(11) - 2;
                string message = e.GetString(Encoding.Unicode, messageLen, 13);

                if(message.StartsWith("/"))
                {
                    string commandName = message.Replace("/", "").Split(' ')[0];
                    foreach(var command in CommandProcessor.GetCommandsByName(commandName))
                    {
                        string[] parameters = message.Replace("/", "").Split(' ');
                        command.Parameters.Clear();
                        for(int i = 1; i < parameters.Length; i++)
                            command.Parameters.Add(parameters[i]);
                        command.Callback(command);
                    }
                }
            }
            else
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