using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell;
using System;
using System.Threading.Tasks;

namespace simplicode
{
    [Command(PackageIds.OpenToolWindowCommand)]
    internal sealed class OpenToolWindowCommand : BaseCommand<OpenToolWindowCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            // 디버깅용 메시지 출력
            //await VS.MessageBox.ShowAsync("OpenToolWindowCommand 버튼이 눌렸다!", "Info");

            try
            {
                ToolWindowPane window = await this.Package.ShowToolWindowAsync(
                    typeof(MyToolWindowPane),
                    0,
                    true,
                    this.Package.DisposalToken);

                if (window == null)
                {
                    throw new NotSupportedException("Cannot create tool window.");
                }
            }
            catch (Exception ex)
            {
                // 오류 발생 시 예외 메시지 출력
                await VS.MessageBox.ShowErrorAsync("툴 창 생성 실패", ex.Message);
            }
        }

    }
}
