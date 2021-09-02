using System.Collections.Generic;

namespace Utils.Runtime
{
    public static class Misc
    {
        public static string mkString<T>(IEnumerable<T> list, string sep=", ", string ini="", string end="")
        {
            string str = ini;
            bool first = true;
            foreach (var element in list)
            {
                if (!first)
                {
                    str += sep;
                }
                else
                {
                    first = false;
                }

                str += element.ToString();
            }
            str += end;
        
            return str;
        }
    }
}

