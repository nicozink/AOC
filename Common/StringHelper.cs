using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.StringHelper
{
    /// <summary>
    /// Helperfunction for string types.
    /// </summary>
    public static class StringHelper
    {
        public static int CountOccurs(this string str, string subStr)
        {
            int count = 0;
            int index = 0;

            while ((index = str.IndexOf(subStr, index)) != -1)
            {
                index += subStr.Length;
                count++;
            }

            return count;
        }
    }
}
