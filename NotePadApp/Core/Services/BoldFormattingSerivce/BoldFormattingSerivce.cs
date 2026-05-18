using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using NotePadApp.Core.Services;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

public class BoldFormattingService : DocumentColorizingTransformer
{
    private readonly List<FormattedSegment> _segments;

    public BoldFormattingService(List<FormattedSegment> segments)
    {
        _segments = segments;
    }

    protected override void ColorizeLine(DocumentLine line)
    {
        foreach (var seg in _segments)
        {
            if (!seg.IsBold) continue;

            int lineStart = line.Offset;
            int lineEnd = line.EndOffset;

            int overlapStart = Math.Max(seg.StartOffset, lineStart);
            int overlapEnd = Math.Min(seg.EndOffset, lineEnd);

            if (overlapStart >= overlapEnd) continue;

            ChangeLinePart(overlapStart, overlapEnd, element =>
            {
                element.TextRunProperties.SetTypeface(
                    new Typeface(
                        element.TextRunProperties.Typeface.FontFamily,
                        FontStyles.Normal,
                        FontWeights.Bold,   
                        FontStretches.Normal
                    )
                );
            });
        }
    }
}