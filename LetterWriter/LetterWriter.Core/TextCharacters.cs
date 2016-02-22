using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LetterWriter
{
    [DebuggerDisplay("TextCharacters: {RawCharacters,nq}")]
    public class TextCharacters : TextRun
    {
        public override int Length
        {
            get { return this.RawCharacters.Length; }
        }

        public string RawCharacters { get; private set; }

        public TextCharacters(string text)
        {
            if (text.Contains("\n"))
            {
                throw new ArgumentException("テキストに改行を含めることはできません。LineBreakを利用してください。");
            }

            this.RawCharacters = text;
        }

        public override void GetCharacters(GlyphProvider glyphProvider, TextModifierScope textModifierScope, IList<IGlyph> buffer)
        {
            glyphProvider.GetGlyphsFromString(textModifierScope, this.RawCharacters, buffer);
        }
    }
}