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
            var glyph = g as Glyph;
            if (g != null)
            {
                for (var i = 0; i < this.LineBreakRuleNotAllowedAtBeginOfLine.Length; i++)
                {
                    if (this.LineBreakRuleNotAllowedAtBeginOfLine[i] == glyph.Character) return true;
                }
            }
            return false;
        }

        public virtual bool IsLineBreakRuleNotAllowedAtEndOfLine(IGlyph g)
        {
            var glyph = g as Glyph;
            if (g != null)
            {
                for (var i = 0; i < this.LineBreakRuleNotAllowedAtEndOfLine.Length; i++)
                {
                    if (this.LineBreakRuleNotAllowedAtEndOfLine[i] == glyph.Character) return true;
                }
            }
            return false;
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