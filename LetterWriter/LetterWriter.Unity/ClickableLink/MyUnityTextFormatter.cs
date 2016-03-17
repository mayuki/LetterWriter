using LetterWriter;
using LetterWriter.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Features.LetterWriter
{
    public class MyUnityTextFormatter : UnityTextFormatter
    {
        public MyUnityTextFormatter(Font font, int fontSize, Color color)
            : base(font, fontSize, color)
        {
        }

        public override TextModifierScope CreateTextModifierScope(TextModifierScope parent, TextModifier textModifier)
        {
            return new MyTextModifierScope((UnityTextModifierScope)parent, (UnityTextModifier)(textModifier ?? this.CreateDefaultTextModifier()));
        }
        public override GlyphPlacement CreateGlyphPlacement(TextModifierScope currenTextModifierScope, IGlyph glyph, int x, int y, int index, int indexInTextRun, int textRunLength)
        {
            return new MyGlyphPlacement(glyph, x, y, index, ((currenTextModifierScope is MyTextModifierScope) ? ((MyTextModifierScope)currenTextModifierScope).Href : null));
        }

        public override TextModifier CreateDefaultTextModifier()
        {
            return new MyTextModifier
            {
                FontSize = this.FontSize,
                FontStyle = FontStyle.Normal,
                Spacing = 0,
                RubyFontScale = 0.5f,
                Color = this.Color
            };
        }
    }

    public class MyGlyphPlacement : GlyphPlacement
    {
        public string Href { get; set; }
        public MyGlyphPlacement(IGlyph glyph, int x, int y, int index, string href) : base(glyph, x, y, index)
        {
            this.Href = href;
        }
    }

    public class MyTextModifier : UnityTextModifier
    {
        public string Href { get; set; }
    }

    public class MyTextModifierScope : UnityTextModifierScope
    {
        protected new MyTextModifierScope Parent { get { return (MyTextModifierScope)base.Parent; } }
        public string Href
        {
            get { return ((MyTextModifier)this.TextModifier).Href ?? ((this.Parent != null) ? this.Parent.Href : null); }
            set { ((MyTextModifier)this.TextModifier).Href = value; }
        }

        public MyTextModifierScope(TextModifierScope<UnityTextModifier> parent, UnityTextModifier textModifier) : base(parent, textModifier)
        {
            this.TextModifier = new MyTextModifier();
            if (textModifier != null)
            {
                this.Apply(textModifier);
            }
        }

        public override void Apply(UnityTextModifier textModifier)
        {
            base.Apply(textModifier);

            if (textModifier is MyTextModifier && this.TextModifier is MyTextModifier)
            {
                this.Href = ((MyTextModifier)textModifier).Href;
                if (this.Href != null)
                {
                    this.Color = UnityEngine.Color.blue;
                }
            }
        }
    }
}
