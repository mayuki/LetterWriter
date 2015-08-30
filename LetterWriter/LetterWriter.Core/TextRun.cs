namespace LetterWriter
{
    public abstract class TextRun
    {
        public static readonly IGlyph[] EmptyGlyphs = new IGlyph[0];

        /// <summary>
        /// TextRunが持つGlyphの数を返します。例えばルビグループの場合にはルビの分のGlyphもカウントします。
        /// </summary>
        public virtual int TotalGlyphCount { get { return this.Length; } }
        
        /// <summary>
        /// TextRunのテキストの長さを返します。ルビグループなどの場合であれば親文字列の長さのみを返します。
        /// </summary>
        public abstract int Length { get; }

        public virtual bool CanWrap { get { return true; } }

        public abstract IGlyph[] GetCharacters(GlyphProvider glyphProvider, TextModifierScope textModifierScope);
    }
}