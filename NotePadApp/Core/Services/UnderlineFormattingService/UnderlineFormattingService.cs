using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using NotePadApp.Core.Services;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

public class UnderlineFormattingService : DocumentColorizingTransformer
{
    private readonly List<FormattedSegment> _segments;

    public UnderlineFormattingService(List<FormattedSegment> segments)
    {
        _segments = segments;
    }

    protected override void ColorizeLine(DocumentLine line)
    {
        foreach (var seg in _segments)
        {
            if (!seg.IsUnderline) continue;

            int lineStart = line.Offset;
            int lineEnd = line.EndOffset;

            int overlapStart = Math.Max(seg.StartOffset, lineStart);
            int overlapEnd = Math.Min(seg.EndOffset, lineEnd);

            if (overlapStart >= overlapEnd) continue;

            ChangeLinePart(overlapStart, overlapEnd, element =>
            {
                element.TextRunProperties.SetTextDecorations(
                    TextDecorations.Underline
                );
            });
        }
    }
}