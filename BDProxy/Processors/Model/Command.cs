using System.Collections.Generic;

namespace BDProxy.Processors.Model
{
    public class Command
    {
        
        public string Name { get; set; }
        public List<string> Parameters { get; set; }
        public CommandExecutedCallback Callback { get; set; }

        public delegate void CommandExecutedCallback(Command command);

        public Command(string name, CommandExecutedCallback callBack)
        {
            Name = name;
            Parameters = new List<string>();
            Callback = callBack;
        }

    }
}