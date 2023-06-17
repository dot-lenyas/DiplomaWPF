using System;
using System.Text;

namespace SimplePascalParser
{
    class SampleIterating
    {
        private static String CreateIndent(int depth)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < depth; i++)
            {
                sb.Append(' ');
            }
            return sb.ToString();
        }
    }
}