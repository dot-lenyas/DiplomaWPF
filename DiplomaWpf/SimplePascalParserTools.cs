using System.Collections.Generic;
using PascalABCCompiler.SyntaxTree;
using PascalABCCompiler.Errors;
using QUT.Gppg;
using GPPGParserScanner;

namespace SimplePascalParser
{
    // Класс глобальных описаний и статических методов
    // для использования различными подсистемами парсера и сканера
    public static class PT // PT - parser tools
    {
        public static List<Error> Errors = new List<Error>();
        //public static int max_errors;
        public static string CurrentFileName;
        public static SourceContext ToSourceContext(LexLocation loc)
        {
            if (loc != null)
                return new SourceContext(loc.StartLine, loc.StartColumn + 1, loc.EndLine, loc.EndColumn);
            return null;
        }

        public static string CreateErrorString(params object[] args)
        {
            string[] ww = new string[args.Length - 1];
            for (int i = 1; i < args.Length; i++)
                ww[i - 1] = (string)args[i];
            string w = string.Join(" или ", ww);
            return string.Format("Синтаксическая ошибка: встречено {0}, а ожидалось {1}", args[0], w);
        }

        public static void AddError(string message, LexLocation loc)
        {
            Errors.Add(new SyntaxError(message, CurrentFileName, loc, null));
        }
    }
}