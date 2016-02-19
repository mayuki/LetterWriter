namespace LetterWriter
{
    public class TextEndOfSegment : TextRun
    {
        public static readonly TextEndOfSegment Default = new TextEndOfSegment();

        private TextEndOfSegment()
        {
        }

        public override int Length { get { return 0; } }
        public override IGlyph[] GetCharacters(GlyphProvider glyphProvider, TextModifierScope textModifierScope)
        {
            return EmptyGlyphs;
        }
    }
}