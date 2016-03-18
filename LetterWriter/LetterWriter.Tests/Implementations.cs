using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LetterWriter.Markup;

/// <summary>
/// テストに必要な各種の仮の実装を含む名前空間です。
/// </summary>
namespace LetterWriter.Tests.Implementations
{
    public class ConsoleMarkupParser : LetterWriterMarkupParser
    {
        protected override IEnumerable<TextRun> VisitMarkupElement(Element element, string tagNameUpper)
        {
            if (tagNameUpper == "COLOR")
            {
                yield return new ConsoleTextModifier() { Color = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), element.Attributes["Value"], true) };
                foreach (var x in base.VisitMarkupElement(element, tagNameUpper)) yield return x;
                yield return TextEndOfSegment.Default;
                yield break;
            }

            if (tagNameUpper == "B")
            {
                yield return new ConsoleTextModifier() { IsBold = true };
                foreach (var x in base.VisitMarkupElement(element, tagNameUpper)) yield return x;
                yield return TextEndOfSegment.Default;
                yield break;
            }

            foreach (var x in base.VisitMarkupElement(element, tagNameUpper)) yield return x;
        }
    }

    public class ConsoleGlyph : Glyph
    {
        public ConsoleColor Color { get; set; }

        public ConsoleGlyph(char c)
            : base(c, Regex.IsMatch(c.ToString(), "^[a-zA-Z,.<>;:\"'/?~!@#$%^&*()_+`1234567890-={}\\[\\] ]$") ? 1 : 2, 1)
        { }
    }

    public class ConsoleGlyphProvider : GlyphProvider<ConsoleTextModifierScope>
    {
        protected override void GetGlyphsFromStringCore(ConsoleTextModifierScope textModifierScope, string value, IList<IGlyph> buffer)
        {
            foreach (var x in value)
            {
                buffer.Add(new ConsoleGlyph(x) { Color = textModifierScope.Color ?? ((textModifierScope.IsBold ?? false) ? ConsoleColor.White : ConsoleColor.Gray) });
            }
        }
    }

    public class ConsoleTextFormatter : TextFormatter
    {
        public ConsoleTextFormatter()
        {
            this.Initialize();
        }

        public override TextModifierScope CreateTextModifierScope(TextModifierScope parent, TextModifier textModifier)
        {
            return new ConsoleTextModifierScope((ConsoleTextModifierScope)parent, (ConsoleTextModifier)textModifier);
        }

        public override GlyphProvider CreateGlyphProvider()
        {
            return new ConsoleGlyphProvider();
        }
    }

    public class ConsoleTextModifier : TextModifier
    {
        public bool? IsBold { get; set; }
        public ConsoleColor? Color { get; set; }
        public ConsoleColor? RubyColor { get; set; }
    }

    public class ConsoleTextModifierScope : TextModifierScope<ConsoleTextModifier>, IRubyTextModifierScope
    {
        protected new ConsoleTextModifierScope Parent { get { return (ConsoleTextModifierScope)base.Parent; } }

        public bool? IsBold
        {
            get { return this.TextModifier.IsBold ?? ((this.Parent != null) ? this.Parent.IsBold : null); }
            set { this.TextModifier.IsBold = value; }
        }

        public ConsoleColor? Color
        {
            get { return this.TextModifier.Color ?? ((this.Parent != null) ? this.Parent.Color : null); }
            set { this.TextModifier.Color = value; }
        }

        public ConsoleColor? RubyColor
        {
            get { return this.TextModifier.RubyColor ?? ((this.Parent != null) ? this.Parent.RubyColor : null); }
            set { this.TextModifier.RubyColor = value; }
        }

        public override void Apply(ConsoleTextModifier textModifier)
        {
            base.Apply(textModifier);

            this.IsBold = textModifier.IsBold;
            this.Color = textModifier.Color;
            this.RubyColor = textModifier.RubyColor;
        }

        public ConsoleTextModifierScope()
            : this(null, null)
        {
        }

        public ConsoleTextModifierScope(ConsoleTextModifierScope parent, ConsoleTextModifier textModifier)
            : base(parent, textModifier)
        {
        }

        public TextModifierScope RubyScope
        {
            get
            {
                return new ConsoleTextModifierScope(this,
                    new ConsoleTextModifier() { Color = this.RubyColor, IsBold = this.IsBold });
            }
        }
    }
}
