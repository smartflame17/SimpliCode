using simplicode.Utils;
using System.Collections.Generic;

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
            List<string> lines = new List<string>(buffer.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)); //get current document text and parse it line by line

            var stringSimilarity = new StringSimilarity(threshold : 0.7);
            stringSimilarity.GetTwoMostSimilarLines(lines);

            await VS.MessageBox.ShowWarningAsync("simplicode", "Button clicked");
        }
    }
}
