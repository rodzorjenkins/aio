using System.Collections.Generic;
using System.Linq;
using BDProxy.Processors.Model;

namespace BDProxy.Processors
{
    public class CommandProcessor
    {

        private static List<Command> commands;

        static CommandProcessor()
        {
            commands = new List<Command>();
        }

        public static bool Register(string name, Command.CommandExecutedCallback callback)
        {
            if(!commands.Exists(t => t.Name == name))
                commands.Add(new Command(name, callback));
            else
                return false;
            return true;
        }

        public static bool Deregister(string name)
        {
            if(commands.Exists(t => t.Name == name))
                commands.Remove(commands.First(t => t.Name == name));
            else
                return false;
            return true;
        }

        public static IEnumerable<Command> GetCommandsByName(string name)
        {
            return commands.Where(t => t.Name == name);
        }
        
    }
}