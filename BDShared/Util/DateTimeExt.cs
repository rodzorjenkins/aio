using System;

namespace BDShared.Util
{
    public static class DateTimeExt
    {

        public static int ToUnix(this DateTime dt)
        {
            return (int)(dt.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

    }
}