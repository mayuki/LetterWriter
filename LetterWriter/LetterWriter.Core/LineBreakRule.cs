using System;
using System.Globalization;
using System.Linq;

namespace LetterWriter
{
    public class LineBreakRule
    {
        protected char[] LineBreakRuleNotAllowedAtBeginOfLine = ")]）｝〕〉》」』】〙〗〟’”｠»ヽヾーァィゥェォッャュョヮヵヶぁぃぅぇぉっゃゅょゎゕゖㇰㇱㇲㇳㇴㇵㇶㇷㇸㇹㇺㇻㇼㇽㇾㇿ々〻‐゠–～！？?!‼⁇⁈⁉・、:;,。.".ToCharArray();
        protected char[] LineBreakRuleNotAllowedAtEndOfLine = "([（｛〔〈《「『【〘〖〝‘“｟«".ToCharArray();

        /// <summary>
        /// 英単語の途中で行を分割するかどうかを取得、設定します。
        /// </summary>
        public bool IsWordWrapBreakword { get; set; }

        public LineBreakRule()
        {
            this.IsWordWrapBreakword = true;
        }

        /// <summary>
        /// 指定した文字が行頭に来ることを許可されているかどうかを返します。
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public virtual bool IsLineBreakRuleNotAllowedAtBeginOfLine(IGlyph g)
        {
            return (g is Glyph) && LineBreakRuleNotAllowedAtBeginOfLine.Contains(((Glyph)g).Character);
        }

        /// <summary>
        /// 指定した文字が行末に来ることを許可されているかどうかを返します。
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public virtual bool IsLineBreakRuleNotAllowedAtEndOfLine(IGlyph g)
        {
            return (g is Glyph) && LineBreakRuleNotAllowedAtEndOfLine.Contains(((Glyph)g).Character);
        }

        /// <summary>
        /// 指定した文字が単語の途中にあるかどうかを返します。
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public virtual bool IsInWord(IGlyph g)
        {
            var cat = Char.GetUnicodeCategory(((Glyph)g).Character);

            return cat == UnicodeCategory.DecimalDigitNumber ||
                   cat == UnicodeCategory.LowercaseLetter ||
                   cat == UnicodeCategory.UppercaseLetter;
        }

        /// <summary>
        /// 指定した文字が改行可能文字かどうかを返します。
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public virtual bool CanWrap(IGlyph g)
        {
            return !(this.IsLineBreakRuleNotAllowedAtBeginOfLine(g));
        }

    }
}