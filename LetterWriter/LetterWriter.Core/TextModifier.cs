using System.Collections.Generic;

namespace LetterWriter
{
    public abstract class TextModifier : TextRun
    {
        public override int Length { get { return 0; } }
        public override void GetCharacters(GlyphProvider glyphProvider, TextModifierScope textModifierScope, IList<IGlyph> buffer)
        {
        }

        public float? Spacing { get; set; }
    }
}