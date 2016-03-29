using System;
using System.Collections.Generic;
using System.IO;
using BDShared.Util;
using CSScriptLibrary;

namespace BDProxy.Util.Extending
{
    internal class ScriptController
    {

        public List<Script> Scripts;

        public ScriptController()
        {
            Scripts = new List<Script>();
        }

        public void LoadScripts()
        {
            string[] files = Directory.GetFiles("scripts\\", "*.cs");
            Scripts.Clear();
            foreach(var file in files)
            {
                try
                {
                    Script plugin = CSScript.Evaluator.LoadFile<Script>(file);
                    Scripts.Add(plugin);
                }
                catch(Exception e)
                {
                    Logger.Log("ScriptController", "{0}", Logger.LogLevel.Error, e);
                }
            }

            Logger.Log("ScriptController", "Loaded {0} script(s).", Logger.LogLevel.Normal, Scripts.Count);
        }

        public void UnloadScripts()
        {
            Scripts.Clear();
        }

    }
}