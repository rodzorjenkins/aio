using System;

namespace BDShared.Util.Attributes
{
    [AttributeUsage(AttributeTargets.All)]
    public class Developer : Attribute
    {

        public string Name { get; set; }
        public string Profile { get; set; }

        public Developer(string name)
        {
            Name = name;
        }

        public Developer(string name, string profile) : this(name)
        {
            Profile = profile;
        }

    }
}