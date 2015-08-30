using System;
using System.Linq;

namespace LetterWriter
{
    public class TextLine
    {
        public GlyphPlacement[] PlacedGlyphs { get; set; }

        public override string ToString()
        {
            return "TextLine: " + String.Join("", this.PlacedGlyphs.Select(x => x.Glyph).OfType<Glyph>().Select(x => x.Character.ToString()).ToArray());
        }
    }
}