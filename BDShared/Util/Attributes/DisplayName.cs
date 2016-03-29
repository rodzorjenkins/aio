using System;

namespace BDShared.Util.Attributes
{
    [AttributeUsage(AttributeTargets.All)]
    public class DisplayName : Attribute
    {

        public string Name { get; set; }

        public DisplayName(string name)
        {
            Name = name;
        }

    }
}