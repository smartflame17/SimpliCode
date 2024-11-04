using EnvDTE;
using simplicode.Utils;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
namespace simplicode
{
    [Command(PackageIds.MyCommand)]
    internal sealed class GetSimilarLines : BaseCommand<GetSimilarLines>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            DocumentView docView = await VS.Documents.GetActiveDocumentViewAsync();
            if (docView?.TextView == null) return; //not a text window

            string buffer = docView.Document.TextBuffer.CurrentSnapshot.GetText();
            string docName = docView.Document.TextBuffer.GetFileName();
            List<string> lines = new List<string>(buffer.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)); //get current document text and parse it line by line


            //Set up StringSimilarity object and call method with current document text
            var threshold = OptionsPage.Instance.SimilarityThreshold;
            var blockSize = OptionsPage.Instance.CodeBlockSize;
            var stringSimilarity = new StringSimilarity(threshold, blockSize);
            var result = stringSimilarity.GetSimilarLineNums(lines);

            //Open up new Output window and write results
            
            Community.VisualStudio.Toolkit.OutputWindowPane pane = await VS.Windows.CreateOutputWindowPaneAsync("Duplicates for " + docName);

            if (result.Count == 0)
            {
                await pane.WriteLineAsync("No Similar Code Detected");
            }
            else
            {
                foreach (var similarBlock in result)
                {
                    string output = "Line ";
                    //await pane.WriteLineAsync
                    foreach (var line in similarBlock)
                    {
                        int actualLineNum = 1;
                        actualLineNum += line;
                        output += actualLineNum.ToString();
                        output += " ";
                    }
                    output += "are detected as similar code; Check it out.";
                    await pane.WriteLineAsync(output);
                }
            }

            //Notify user that detection is complete with warning box
            await VS.MessageBox.ShowWarningAsync("Detection Complete", "Check the Output Duplicates");
        }
    }
}
