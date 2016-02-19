namespace LetterWriter
{
    public class LineBreak : TextRun
    {
        public static readonly LineBreak Default = new LineBreak();

        private LineBreak()
        {
        }

        public override int Length
        {
            get { return 0; }
        }

        public override IGlyph[] GetCharacters(GlyphProvider glyphProvider, TextModifierScope textModifierScope)
        {
            return EmptyGlyphs;
        }
    }
}