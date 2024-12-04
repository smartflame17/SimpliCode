using Community.VisualStudio.Toolkit;
using EnvDTE;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using simplicode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace simplicode.UI
{
    public partial class MyToolWindowControl : UserControl
    {
        private List<List<int>> detectedBlocks = new List<List<int>>(); // 유사 코드 블록 저장
        private int currentIndex = 0; // 현재 하이라이트 중인 블록의 인덱스

        public MyToolWindowControl()
        {
            InitializeComponent();
        }

        private async void RunButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            await RunDuplicateDetectorAsync();
        }

        private async Task RunDuplicateDetectorAsync()
        {
            try
            {
                DocumentView docView = await VS.Documents.GetActiveDocumentViewAsync();
                if (docView?.TextView == null)
                {
                    await VS.MessageBox.ShowWarningAsync("No Active Document", "Please open a text document to run duplicate detector.");
                    return;
                }

                string buffer = docView.Document.TextBuffer.CurrentSnapshot.GetText();
                string docName = docView.Document.TextBuffer.GetFileName();
                List<string> lines = new List<string>(buffer.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None));

                var threshold = OptionsPage.Instance.SimilarityThreshold;
                var blockSize = OptionsPage.Instance.CodeBlockSize;
                //var SemanticAnalysis = OptionsPage.Instance.SemanticAnalysis;
                //var API_Key = OptionsPage.Instance.OpenAI_API_Key;
                var stringSimilarity = new StringSimilarity(threshold, blockSize);
                //detectedBlocks = stringSimilarity.GetSimilarLineNums(lines);
                detectedBlocks = stringSimilarity.GetSimilarLinesWithBlockSize(lines);

                // 출력 창 생성
                Community.VisualStudio.Toolkit.OutputWindowPane pane = await VS.Windows.CreateOutputWindowPaneAsync("Duplicates for " + docName);

                if (detectedBlocks.Count == 0)
                {
                    await pane.WriteLineAsync("No Similar Code Detected");
                    await VS.MessageBox.ShowWarningAsync("Detection Complete", "No duplicates were found.");
                    return;
                }

                // 출력 창에 결과 쓰기
                for (int i = 0; i < detectedBlocks.Count; i++)
                {
                    string output = $"Block {i + 1}: Line ";
                    foreach (var line in detectedBlocks[i])
                    {
                        output += $"{line + 1} "; // 0-based 인덱스를 1-based로 변환
                    }
                    output += "are similar.";
                    await pane.WriteLineAsync(output);
                }

                // 첫 번째 블록 처리
                currentIndex = 0;
                var firstBlock = detectedBlocks[currentIndex];
                HighlightBlock(docView.TextView, firstBlock);
                await VS.MessageBox.ShowAsync($"First Similar Block: Lines {string.Join(", ", firstBlock.Select(l => l + 1))}", "Info");
            }
            catch (Exception ex)
            {
                await VS.MessageBox.ShowErrorAsync("Error", $"An error occurred while running the detector: {ex.Message}");
            }
        }

        private void NextButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                if (detectedBlocks.Count == 0)
                {
                    _ = VS.MessageBox.ShowWarningAsync("No Results", "Run Duplicate Detector first to see results.");
                    return;
                }

                currentIndex = (currentIndex + 1) % detectedBlocks.Count;

                // 다음 블록 가져오기
                var nextBlock = detectedBlocks[currentIndex];
                DocumentView docView = VS.Documents.GetActiveDocumentViewAsync().Result;

                if (docView?.TextView != null)
                {
                    HighlightBlock(docView.TextView, nextBlock);
                    _ = VS.MessageBox.ShowAsync($"Next Similar Block: Lines {string.Join(", ", nextBlock.Select(l => l + 1))}", "Info");
                }
            }
            catch (Exception ex)
            {
                _ = VS.MessageBox.ShowErrorAsync("Error", $"An error occurred while highlighting the next block: {ex.Message}");
            }
        }

        private void HighlightBlock(IWpfTextView textView, List<int> block)
        {
            try
            {
                Microsoft.VisualStudio.Shell.ThreadHelper.JoinableTaskFactory.Run(async () =>
                {
                    await Microsoft.VisualStudio.Shell.ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    if (textView == null || textView.TextSnapshot == null)
                    {
                        throw new InvalidOperationException("TextView or TextSnapshot is null.");
                    }

                    var buffer = textView.TextBuffer;
                    var tagger = buffer.Properties.GetOrCreateSingletonProperty(() => new HighlightTagger(buffer));

                    if (tagger == null)
                    {
                        throw new InvalidOperationException("Failed to create or retrieve HighlightTagger.");
                    }

                    var snapshot = textView.TextSnapshot;

                    // 라인 번호 유효성 검증
                    var validBlock = block.Where(lineNum => lineNum >= 0 && lineNum < snapshot.LineCount).ToList();
                    if (validBlock.Count == 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(block), "No valid line numbers found for highlighting.");
                    }

                    var spans = validBlock.Select(lineNum => snapshot.GetLineFromLineNumber(lineNum))
                                          .Select(line => new SnapshotSpan(snapshot, line.Start, line.Length))
                                          .ToList();

                    tagger.SetSpans(spans); // 하이라이트 설정
                });
            }
            catch (Exception ex)
            {
                _ = VS.MessageBox.ShowErrorAsync("Highlight Error", $"An error occurred while highlighting: {ex.Message}");
            }
        }

    }
}
