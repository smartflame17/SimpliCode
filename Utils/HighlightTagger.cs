using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;

namespace simplicode.Utils
{
    public class HighlightTagger : ITagger<TextMarkerTag>
    {
        private readonly ITextBuffer _buffer;
        private readonly List<SnapshotSpan> _spans = new List<SnapshotSpan>();

        public HighlightTagger(ITextBuffer buffer)
        {
            _buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
        }

        // 이벤트: 태그 변경을 알림
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        // 스팬 리스트 설정
        public void SetSpans(IEnumerable<SnapshotSpan> spans)
        {
            _spans.Clear();
            _spans.AddRange(spans);

            // 모든 변경된 스팬에 대해 태그 변경 알림
            foreach (var span in _spans)
            {
                TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(span));
            }
        }

        // 태그 제공
        public IEnumerable<ITagSpan<TextMarkerTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            foreach (var span in _spans)
            {
                if (spans.IntersectsWith(new NormalizedSnapshotSpanCollection(span)))
                {
                    yield return new TagSpan<TextMarkerTag>(span, new TextMarkerTag("blue"));
                }
            }
        }
    }
}
