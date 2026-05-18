namespace NotePadApp.Core.Services
{
    public class FormattedSegment
    {
        public int StartOffset { get; set; }
        public int EndOffset { get; set; }
        public bool IsBold { get; set; }
        public bool IsItalic { get; set; }
        public bool IsUnderline { get; set; }
        public bool IsStrikethrough { get; set; }
        public bool IsSuperscript { get; set; }

        public bool IsSubscript { get; set; }
    }
}
