using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace simplicode.Utils
{
    public static class StringUtils
    {
        //Returns whether line is a function declaration which uses :: for namespace 
        public static bool IsFunctionDeclaration(string str)
        {
            if (str == null) return false;
            Regex isFunction = new Regex("[A-Za-z]+.*::.*\\(.*\\)", RegexOptions.IgnoreCase);

            return isFunction.IsMatch(str);
        }

        //Returns whether line is a one-line comment
        public static bool IsComment(string str)
        {
            if (str == null) return false;
            Regex isComment = new Regex(@"//", RegexOptions.IgnoreCase);

            return isComment.IsMatch(str);
        }

        public static bool IsMultiLineComment(string str)
        {
            if (str == null) return false;
            Regex isStartofComment = new Regex("/\\*", RegexOptions.IgnoreCase);
            Regex isEndofComment = new Regex("\\*/", RegexOptions.IgnoreCase);

            return isStartofComment.IsMatch(str) || isEndofComment.IsMatch(str);
        }

        //Returns whether line is an '#include' or '#pragma' for header files and warnings using regex
        public static bool IsInclude(string str)
        {
            if (str == null) return false;
            Regex hasSharp = new Regex("\\B#([A-Za-z0-9]{2,})(?![~!@#$%^&*()=+_`\\-\\|\\/'\\[\\]\\{\\}]|[?.,]*\\w)", RegexOptions.IgnoreCase);

            return hasSharp.IsMatch(str);
        }

        //Returns whether line is an valid string for string similarity
        public static bool IsValidLine(string str)
        {
            if (str == null) return false;
            if (IsFunctionDeclaration(str) || IsComment(str) || IsInclude(str) || IsMultiLineComment(str)) return false;
            else return true;
        }

        //Returns line number of the scope (function) of the code corresponding to the current line
        public static int getCurrentScope(List<string> lines, int currentLineNum)
        {
            if (currentLineNum < 0 || currentLineNum > lines.Count)
            {
                throw new ArgumentException("Invalid Line Number");
            }
            while (!IsFunctionDeclaration(lines[currentLineNum]))
            {
                currentLineNum--;
            }
            return currentLineNum;
        }
    }
}
