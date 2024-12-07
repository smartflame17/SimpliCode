using System.Runtime.InteropServices;
using EnvDTE;
using simplicode.Utils;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;

namespace simplicode
{
    [Command(PackageIds.SemanticAnalysis)]
    internal sealed class SemanticAnalysis : BaseCommand<SemanticAnalysis>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            await VS.MessageBox.ShowWarningAsync("Command1", "Button clicked");
        }
    }
}
