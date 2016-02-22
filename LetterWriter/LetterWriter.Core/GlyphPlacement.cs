using System;
using System.Diagnostics;

namespace LetterWriter
{
    [DebuggerDisplay("GlyphPlacement: {Glyph,nq} | ({X,nq}, {Y,nq})")]
    public class GlyphPlacement : IEquatable<GlyphPlacement>
    {
        public virtual IGlyph Glyph { get; set; }
        public virtual int X { get; set; }
        public virtual int Y { get; set; }
        public virtual int Index { get; set; }

        public static readonly GlyphPlacement Empty = new GlyphPlacement(LetterWriter.Glyph.Empty, 0, 0, 0);

        public GlyphPlacement(IGlyph glyph, int x, int y, int index)
        {
            this.Glyph = glyph;
            this.X = x;
            this.Y = y;
            this.Index = index;
        }

        public bool Equals(GlyphPlacement other)
        {
            return this.Glyph == other.Glyph &&
                   this.Index == other.Index &&
                   this.X == other.X &&
                   this.Y == other.Y;
        }
    }
}