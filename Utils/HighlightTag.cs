namespace simplicode.Utils
{
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Utilities;
    using System.Windows.Media;
    using System.ComponentModel.Composition;

    public class HighlightTag : TextMarkerTag
    {
        public HighlightTag() : base("MarkerFormatDefinition/HighlightFormat") { }
    }

    [Export(typeof(EditorFormatDefinition))]
    [Name("MarkerFormatDefinition/HighlightFormat")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    public class HighlightFormatDefinition : MarkerFormatDefinition
    {
        public HighlightFormatDefinition()
        {
            this.ForegroundColor = Colors.White; // 텍스트 색상
            this.BackgroundColor = Colors.Yellow; // 하이라이트 색상
            this.DisplayName = "Highlight Similar Block"; // VS 옵션에 표시될 이름
        }
    }
}
