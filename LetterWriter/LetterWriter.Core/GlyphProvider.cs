using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LetterWriter
{
    public abstract class GlyphProvider
    {
        public abstract IGlyph[] GetGlyphsFromString(TextModifierScope textModifierScope, string value);
    }

    public abstract class GlyphProvider<TTextModifierScope> : GlyphProvider where TTextModifierScope : TextModifierScope
    {
        public override IGlyph[] GetGlyphsFromString(TextModifierScope textModifierScope, string value)
        {
            return this.GetGlyphsFromStringCore((TTextModifierScope)textModifierScope, value);
        }

        protected abstract IGlyph[] GetGlyphsFromStringCore(TTextModifierScope textModifierScope, string value);
    }
}
