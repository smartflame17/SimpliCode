using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell;
using simplicode.UI;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace simplicode
{
    [Guid("55F8D2E8-EC28-483B-8CC5-CA587E9FF7BF")] // 고유 GUID
    public class MyToolWindowPane : ToolWindowPane
    {
        public MyToolWindowPane()
        {
            this.Caption = "Duplicate Detector";
            this.Content = new simplicode.UI.MyToolWindowControl(); // 툴 창에 표시할 WPF Control
        }
    }
}
