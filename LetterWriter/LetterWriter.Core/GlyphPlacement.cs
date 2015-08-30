using System.Diagnostics;

namespace LetterWriter
{
    [DebuggerDisplay("GlyphPlacement: {Glyph,nq} | ({X,nq}, {Y,nq})")]
    public class GlyphPlacement
    {
        public IGlyph Glyph { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Index { get; set; }

        public static readonly GlyphPlacement Empty;

        static GlyphPlacement()
        {
            Empty = new GlyphPlacement(LetterWriter.Glyph.Empty, 0, 0, 0);
        }

        public GlyphPlacement(IGlyph glyph, int x, int y, int index)
        {
            this.Glyph = glyph;
            this.X = x;
            this.Y = y;
            this.Index = index;
        }
    }
}