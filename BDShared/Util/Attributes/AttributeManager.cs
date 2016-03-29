using System;

namespace BDShared.Util.Attributes
{
    public class AttributeManager
    {

        public static T GetAttribute<T>(Type element) where T : Attribute
        {
            return Attribute.GetCustomAttribute(element, typeof(T)) as T;
        }

    }
}