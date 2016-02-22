using System.Collections.Generic;

namespace LetterWriter
{
    public class TextEndOfSegment : TextRun
    {
        public static readonly TextEndOfSegment Default = new TextEndOfSegment();

        private TextEndOfSegment()
        {
        }

        public override int Length { get { return 0; } }
        public override void GetCharacters(GlyphProvider glyphProvider, TextModifierScope textModifierScope, IList<IGlyph> buffer)
        {
        }
    }
}