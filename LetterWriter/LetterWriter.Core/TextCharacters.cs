using System;
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
                throw new ArgumentException("�e�L�X�g�ɉ��s���܂߂邱�Ƃ͂ł��܂���BLineBreak�𗘗p���Ă��������B");
            }

            this.RawCharacters = text;
        }

        public override IGlyph[] GetCharacters(GlyphProvider glyphProvider, TextModifierScope textModifierScope)
        {
            return glyphProvider.GetGlyphsFromString(textModifierScope, this.RawCharacters);
        }
    }
}