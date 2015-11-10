using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LetterWriter.Unity.Components;
using UnityEngine;

namespace LetterWriter.Unity
{
    public class UnityGlyphProvider : GlyphProvider<UnityTextModifierScope>
    {
        public Font Font { get; set; }

        public UnityGlyphProvider(Font font)
        {
            this.Font = font;
        }

        protected override IGlyph[] GetGlyphsFromStringCore(UnityTextModifierScope textModifierScope, string value)
        {
            var fontSize = textModifierScope.FontSize ?? 24;
            var fontStyle = textModifierScope.FontStyle ?? FontStyle.Normal;

            var textGenerator = new TextGenerator();
            if (!textGenerator.Populate(value + "…M", new TextGenerationSettings() { font = this.Font, fontSize = fontSize, fontStyle = fontStyle }))
            {
                throw new Exception("TextGenerator.Populate failed");
            }

            this.Font.RequestCharactersInTexture(value, fontSize, fontStyle);

            return value.Select(c =>
            {
                if (Char.IsControl(c)) return null;

                CharacterInfo characterInfo;
                if (!this.Font.GetCharacterInfo(c, out characterInfo, fontSize, fontStyle))
                {
                    throw new Exception("this.Font.GetCharacterInfo failed: " + c);
                }
                return new UnityGlyph(c, characterInfo, textModifierScope.Color, fontSize);
            }).Where(x => x != null).ToArray();
        }
    }

}
