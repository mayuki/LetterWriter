namespace LetterWriter
{
    public abstract class TextModifier : TextRun
    {
        public override int Length { get { return 0; } }
        public override IGlyph[] GetCharacters(GlyphProvider glyphProvider, TextModifierScope textModifierScope)
        {
            return EmptyGlyphs;
        }

        public float? Spacing { get; set; }
    }
}