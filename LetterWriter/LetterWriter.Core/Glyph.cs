using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace LetterWriter
{
    [DebuggerDisplay("Glyph: {Character,nq}")]
    public class Glyph : IGlyph
    {
        public static readonly IGlyph Empty;

        public bool IsWhiteSpaceOrControl
        {
            get { return Char.IsControl(this.Character) || Char.IsWhiteSpace(this.Character); }
        }

        public int Height { get; private set; }
        public int AdvanceWidth { get; private set; }
        public char Character { get; private set; }

        static Glyph()
        {
            Empty = new Glyph(' ', 0, 0);
        }

        public Glyph(char character, int advanceWidth, int height)
        {
            this.Character = character;
            this.AdvanceWidth = advanceWidth;
            this.Height = height;
        }

        public override string ToString()
        {
            return "Glyph: " + this.Character;
        }
    }
}