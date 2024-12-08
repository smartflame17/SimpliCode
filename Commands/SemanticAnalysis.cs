using System.Runtime.InteropServices;
using EnvDTE;
using simplicode.Utils;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using OpenAI.Chat;

namespace simplicode
{
    [Command(PackageIds.SemanticAnalysis)]
    internal sealed class SemanticAnalysis : BaseCommand<SemanticAnalysis>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            DocumentView docView = await VS.Documents.GetActiveDocumentViewAsync();
            if (docView?.TextView == null) return; //not a text window

            string buffer = docView.Document.TextBuffer.CurrentSnapshot.GetText();
            string docName = docView.Document.TextBuffer.GetFileName();
            List<string> lines = new List<string>(buffer.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)); //get current document text and parse it line by line
            var API_KEY = OptionsPage.Instance.OpenAI_API_Key;
            var flag = OptionsPage.Instance.SemanticAnalysis;

            if (!flag)
            {
                await VS.MessageBox.ShowWarningAsync("Enable Semantic Analysis in settings");
            }
            else
            {
                if (API_KEY != null)
                {
                    try
                    {
                        ChatClient client = new(model: "gpt-4o", apiKey: Environment.GetEnvironmentVariable(API_KEY));
                        ChatCompletion completion = await client.CompleteChatAsync("Say 'this is a test.'");
                        await VS.MessageBox.ShowWarningAsync(completion.Content[0].Text);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Validation failed: {ex.Message}");
                        return;
                    }
                }
                else
                {
                    await VS.MessageBox.ShowWarningAsync("Invalid API Key");
                }
            }
            
        }
    }
}
