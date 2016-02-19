using System;
using System.Globalization;
using System.Linq;

namespace LetterWriter
{
    public class LineBreakRule
    {
        protected char[] LineBreakRuleNotAllowedAtBeginOfLine = ")]）｝〕〉》」』】〙〗〟’”｠»ヽヾーァィゥェォッャュョヮヵヶぁぃぅぇぉっゃゅょゎゕゖㇰㇱㇲㇳㇴㇵㇶㇷㇸㇹㇺㇻㇼㇽㇾㇿ々〻‐゠–～！？?!‼⁇⁈⁉・、:;,。.".ToCharArray();
        protected char[] LineBreakRuleNotAllowedAtEndOfLine = "([（｛〔〈《「『【〘〖〝‘“｟«".ToCharArray();

        public virtual bool IsLineBreakRuleNotAllowedAtBeginOfLine(IGlyph g)
        {
            return (g is Glyph) && LineBreakRuleNotAllowedAtBeginOfLine.Contains(((Glyph)g).Character);
        }

        public virtual bool IsLineBreakRuleNotAllowedAtEndOfLine(IGlyph g)
        {
            return (g is Glyph) && LineBreakRuleNotAllowedAtEndOfLine.Contains(((Glyph)g).Character);
        }

        public virtual bool CanWrap(IGlyph g)
        {
            var cat = Char.GetUnicodeCategory(((Glyph)g).Character);
            return !(
                this.IsLineBreakRuleNotAllowedAtBeginOfLine(g) ||
                cat == UnicodeCategory.DecimalDigitNumber ||
                cat == UnicodeCategory.LowercaseLetter ||
                cat == UnicodeCategory.UppercaseLetter
            );
        }
    }
}