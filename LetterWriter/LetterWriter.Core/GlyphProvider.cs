using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LetterWriter
{
    public abstract class GlyphProvider
    {
        public abstract void GetGlyphsFromString(TextModifierScope textModifierScope, string value, IList<IGlyph> buffer);
    }

    public abstract class GlyphProvider<TTextModifierScope> : GlyphProvider where TTextModifierScope : TextModifierScope
    {
        public override void GetGlyphsFromString(TextModifierScope textModifierScope, string value, IList<IGlyph> buffer)
        {
            this.GetGlyphsFromStringCore((TTextModifierScope)textModifierScope, value, buffer);
        }

        protected abstract void GetGlyphsFromStringCore(TTextModifierScope textModifierScope, string value, IList<IGlyph> buffer);
    }
}
