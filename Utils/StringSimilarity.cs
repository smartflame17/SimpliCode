using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using F23.StringSimilarity;

namespace simplicode.Utils
{
    public class StringSimilarity
    {
        //default value is 1.0, meaning only exactly same lines are considered similar
        double threshold = 1.0;

        //default value is 5, any line size less than this will be ignored
        int minBlockSize = 5;

        public StringSimilarity() { }

        //constructor with default value
        public StringSimilarity(double threshold = 1.0, int minBlockSize = 5)
        {
            this.threshold = threshold;
            this.minBlockSize = minBlockSize;
        }
        //setter for threshold
        public void SetThreshold(double threshold) { this.threshold = threshold; }
        //setter for blockSize
        public void SetMinBlockSize(int blockSize) { this.minBlockSize = blockSize; }


        //gets the two most similar lines in a document(list of strings) and its similarity (normal value between 0 and 1)
        public Tuple<List<string>, double> GetTwoMostSimilarLines(List<string> lines)
        {
            List<string> twoStrings = new List<string>();
            double max_similarity = 0;

            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].Length < minBlockSize) continue;
                if (!IsValidLine(lines[i])) continue;
                for (int j = i + 1; j < lines.Count; j++)
                {
                    if (lines[j].Length < minBlockSize) continue;
                    if (!IsValidLine(lines[j])) continue;
                    var algo = new RatcliffObershelp();
                    double similarity = algo.Similarity(lines[i], lines[j]);

                    if (similarity >= this.threshold)
                    {
                        if (similarity >= max_similarity)
                        {
                            twoStrings.Clear();
                            max_similarity = similarity;
                            twoStrings.Add(lines[i]);
                            twoStrings.Add(lines[j]);
                        }
                    }

                }
            }
            var result = new Tuple<List<string>, double>(twoStrings, max_similarity);
            return result;
        }
        //Returns whether line is a function declaration which uses :: for namespace 
        public bool IsFunctionDeclaration(string str)
        {
            if (str == null) return false;
            Regex isFunction = new Regex("[A-Za-z]+.*::.*\\(.*\\)", RegexOptions.IgnoreCase);

            return isFunction.IsMatch(str);
        }
        //Returns whether line is a one-line comment
        public bool IsComment(string str)
        {
            if (str == null) return false;
            Regex isComment = new Regex(@"//", RegexOptions.IgnoreCase);

            return isComment.IsMatch(str);
        }
        //Returns whether line is an '#include' or '#pragma' for header files and warnings using regex
        public bool IsInclude(string str)
        {
            if (str == null) return false;
            Regex hasSharp = new Regex("\\B#([A-Za-z0-9]{2,})(?![~!@#$%^&*()=+_`\\-\\|\\/'\\[\\]\\{\\}]|[?.,]*\\w)", RegexOptions.IgnoreCase);

            return hasSharp.IsMatch(str);
        }
        //Returns whether line is an valid string for string similarity
        public bool IsValidLine(string str)
        {
            if (str == null) return false;
            if (IsFunctionDeclaration(str) || IsComment(str) || IsInclude(str)) return false;
            else return true;
        }
    }
}
