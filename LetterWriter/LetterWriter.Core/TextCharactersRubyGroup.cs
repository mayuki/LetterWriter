using System;
using System.Diagnostics;

namespace LetterWriter
{
    [DebuggerDisplay("TextCharactersRubyGroup: {RawCharacters,nq} ({RawRubyCharacters,nq})")]
    public class TextCharactersRubyGroup : TextCharacters
    {
        public string RawRubyCharacters { get; private set; }

        public RubyPosition RubyPosition { get; set; }

        public override bool CanWrap { get { return false; } }

        public override int TotalGlyphCount
        {
            get
            {
                return this.Length + this.RawRubyCharacters.Length;
            }
        }

        public TextCharactersRubyGroup(string text, string rubyText)
            : base(text)
        {
            this.RawRubyCharacters = rubyText;
            this.RubyPosition = RubyPosition.Before;
        }

        public virtual IGlyph[] GetRubyCharacters(GlyphProvider glyphProvider, TextModifierScope textModifierScope)
        {
            var rubyTextModifierScope = textModifierScope as IRubyTextModifierScope;
            return glyphProvider.GetGlyphsFromString((rubyTextModifierScope != null) ? rubyTextModifierScope.RubyScope : textModifierScope, this.RawRubyCharacters);
        }
    }
}