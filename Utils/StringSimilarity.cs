using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using simplicode.Utils;
using F23.StringSimilarity;
using System.Diagnostics;

namespace simplicode.Utils
{
    public class StringSimilarity
    {
        //default value is 1.0, meaning only exactly same lines are considered similar
        double threshold = 1.0;

        //default value is 10, any line size less than this will be ignored
        int BlockSize = 10;

        private double minThreshold = 0.5;

        private int minBlockSize = 10;

        public StringSimilarity() { }

        //constructor with default value
        public StringSimilarity(double threshold = 1.0, int BlockSize = 10)
        {
            this.threshold = threshold;
            this.BlockSize = BlockSize;

            //added checking for illegal / breaking values
            if (threshold > 1.0 || threshold < minThreshold)
            {
                this.threshold = minThreshold;
            }
            if (BlockSize < minBlockSize || BlockSize < 0)
            {
                this.BlockSize = minBlockSize;
            }
        }
        //setter for threshold
        public void SetThreshold(double threshold) { this.threshold = threshold; }
        //setter for blockSize
        public void SetBlockSize(int blockSize) { this.BlockSize = blockSize; }


        //gets the two most similar lines in a document(list of strings) and its similarity (normal value between 0 and 1)
        public Tuple<List<string>, double> GetTwoMostSimilarLines(List<string> lines)
        {
            List<string> twoStrings = new List<string>();
            double max_similarity = 0;

            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].Length < BlockSize) continue;
                if (!StringUtils.IsValidLine(lines[i])) continue;
                for (int j = i + 1; j < lines.Count; j++)
                {
                    if (lines[j].Length < BlockSize) continue;
                    if (!StringUtils.IsValidLine(lines[j])) continue;

                    //currently used algorithm (subject to change), algorithm must produce normal double values (between 0 to 1)
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

        public List<List<int>> GetSimilarLineNums(List<string> lines) 
        { 
            List<List<int>> result = new List<List<int>>();
            //checks whether current line is already selected as result (to prevent multiple checking over same line)
            bool[] lineNums = new bool[lines.Count];

            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].Length < BlockSize) continue;
                if (!StringUtils.IsValidLine(lines[i])) continue;
                if (lineNums[i]) continue;
                List<int> sameNums = new List<int>();
                sameNums.Add(i);
                lineNums[i] = true;

                for (int j = i + 1; j < lines.Count; j++)
                {
                    if (lines[j].Length < BlockSize) continue;
                    if (!StringUtils.IsValidLine(lines[j])) continue;
                    if (lineNums[j]) continue;
                    //currently used algorithm (subject to change), algorithm must produce normal double values (between 0 to 1)
                    var algo = new RatcliffObershelp();
                    double similarity = algo.Similarity(lines[i], lines[j]);

                    if (similarity >= this.threshold)
                    {
                        sameNums.Add(j);
                        lineNums[j] = true;
                    }

                }
                if (sameNums.Count > 1)
                {
                    result.Add(sameNums);
                }
            }

            //needs post-processing for results
            //get the current scope of given line in code, use the scope to determine whether they are calls to similar yet different functions
            foreach (List<int> similarLines in result)
            {
                for(int i = 0; i < similarLines.Count; i++)
                {
                    for (int j = i + 1; j < similarLines.Count; j++)
                    {
                        if (StringUtils.getCurrentScope(lines, similarLines[i]) == StringUtils.getCurrentScope(lines, similarLines[j]))
                        {
                            //within same scope (function)
                            //TODO: Logic for adjusting similar code within same scope (should be stricter)
                        }
                        else
                        {
                            //different scope
                            //TODO: Logic for adjusting similar code within different scope (should allow more similar code a.k.a basic / universial method calls)
                        }
                    }
                }
            }

            return result;
        }

        public List<List<int>> GetSimilarLinesWithBlocksize(List<string> lines)
        {
            //TODO: parse lines by block size, and perform string silimarity check
            List<List<int>> result = new List<List<int>>();
            var chunkedLines = new List<(string Block, List<int> LineNums)>();

            //Turns lines into chunks with size of BlockSize
            for (int i = 0; i < lines.Count; i+= BlockSize)
            {
                int count = Math.Min(BlockSize, lines.Count - i);
                var currentBlockStrings = lines.GetRange(i, count);
                
                var LineNums = new List<int>();
                for(int j = 0; j < count; j++)
                {
                    //Line Numbers are 1-based
                    LineNums.Add(i+j+1);
                }
                string block = string.Join("", currentBlockStrings);
                chunkedLines.Add((block, LineNums));
            }

            //Perform string similarity calculation on each chunk
            for (int i = 0; i < chunkedLines.Count; i++)
            {
                for (int j = i + 1; j < chunkedLines.Count; j++)
                {
                    var algo = new RatcliffObershelp();
                    double similarity = algo.Similarity(chunkedLines[i].Block, chunkedLines[j].Block);

                    if (similarity < this.threshold)
                    {
                        //Chunks contain similar code
                        using (StringReader reader1 = new StringReader(chunkedLines[i].Block))
                        {
                            string line1;
                            int lineIndex1 = 0;

                            //get Line from block[i]
                            while((line1 = reader1.ReadLine()) != null)
                            {
                                using (StringReader reader2 = new StringReader(chunkedLines[j].Block))
                                {
                                    string line2;
                                    int lineIndex2 = 0;

                                    //get Line from block[j]
                                    while ((line2 = reader2.ReadLine()) != null)
                                    {
                                        similarity = algo.Similarity(line1, line2);

                                        if (similarity < this.threshold)
                                        {
                                            //TODO : append to result
                                        }
                                    }
                                }
                            }
                        }
                   
                    }
                }
            }
            return result;
        }
            
    }

        
}
