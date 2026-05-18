using ICSharpCode.AvalonEdit.Rendering;
using System.Windows;
using System.Windows.Controls;

namespace NotePadApp.Core.Services.IndexFormattingService
{
    public class IndexFormattingService : VisualLineElementGenerator
    {
        private readonly List<FormattedSegment> _segments;

        public IndexFormattingService(List<FormattedSegment> segments)
        {
            _segments = segments;
        }

        public override int GetFirstInterestedOffset(int startOffset)
        {
            var segment = _segments
                .Where(s => s.IsSuperscript || s.IsSubscript)
                .Where(s => s.EndOffset > startOffset)
                .OrderBy(s => s.StartOffset)
                .FirstOrDefault();

            return segment != null ? Math.Max(segment.StartOffset, startOffset) : -1;
        }

        public override VisualLineElement ConstructElement(int offset)
        {
            var segment = _segments
                .FirstOrDefault(s =>
                (s.IsSuperscript || s.IsSubscript) &&
                s.StartOffset <= offset &&
                s.EndOffset > offset);

            if (segment == null) return null;

            int length = segment.EndOffset - offset;
            string text = CurrentContext.Document.GetText(offset, length);

            double shift = CurrentContext.TextView.DefaultLineHeight * 0.3;

            var textBlock = new TextBlock
            {
                Text = text,
                FontSize = CurrentContext.TextView.DefaultLineHeight * 0.5,
                Margin = segment.IsSuperscript
                             ? new Thickness(0, shift, 0, 0)
                             : new Thickness(0, -shift, 0, 0)
            };

            return new InlineObjectElement(length, textBlock);
        }
    }
}
