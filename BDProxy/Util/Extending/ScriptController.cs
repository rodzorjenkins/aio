using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BDShared.Util;
using CSScriptLibrary;

namespace BDProxy.Util.Extending
{
    internal class ScriptController
    {

        public List<Script> Scripts;
        public string ScriptLocation { get; set; }

        public ScriptController()
        {
            Scripts = new List<Script>();
            ScriptLocation = "scripts\\";
        }

        public ScriptController(string scriptLocation) : this()
        {
            ScriptLocation = scriptLocation;
        }

        public void LoadScripts()
        {
            string[] files = Directory.GetFiles(ScriptLocation, "*.cs");
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

            Logger.Log("ScriptController", "Loaded {0} script(s).", Logger.LogLevel.Script, Scripts.Count);
        }

        public bool Contains(string name)
        {
            return Scripts.Exists(t => t.Name.Equals(name));
        }

        public Script GetScriptByName(string name)
        {
            return Scripts.First(t => t.Name.Equals(name));
        }

        public void UnloadScripts()
        {
            Scripts.Clear();
        }

    }
}