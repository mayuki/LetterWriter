using System.Collections.Generic;

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

        public override void GetCharacters(GlyphProvider glyphProvider, TextModifierScope textModifierScope, IList<IGlyph> buffer)
        {
        }
    }
}