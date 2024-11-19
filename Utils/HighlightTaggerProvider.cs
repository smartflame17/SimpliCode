using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace simplicode.Utils
{
    [Export(typeof(ITaggerProvider))]
    [ContentType("text")] // 모든 텍스트 편집기에 적용
    [TagType(typeof(TextMarkerTag))]
    public class HighlightTaggerProvider : ITaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            // HighlightTagger를 텍스트 버퍼에 등록
            return buffer.Properties.GetOrCreateSingletonProperty(() => new HighlightTagger(buffer)) as ITagger<T>;
        }
    }
}
