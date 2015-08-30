namespace LetterWriter
{
    public class TextLineBreakState
    {
        // public TextModifier TextModifierScope { get; set; }
        public int TextRunIndex { get; set; }
        public int Position { get; set; }
        public int GlyphLastIndex { get; set; }
        public TextModifierScope TextModifierScope { get; set; }

        public TextLineBreakState()
        {
            this.Position = -1; // Nextで進める都合-1から初めておく
        }
    }
}