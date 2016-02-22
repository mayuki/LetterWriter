using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using LetterWriter;
using LetterWriter.Markup;

namespace LetterWriter.SampleConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var markupParser = new ConsoleMarkupParser();
            var text = @"デフォルメ（二頭身キャラ）になり、「<ruby value='びもうと'>美妹</ruby>」からグータラな生活を好む「<ruby value='ひもうと'>干物妹</ruby>」と化す。";
            //text = @"&amp;&#x0023;&#35;だから!迷いながら　また<color value='Blue'><ruby value='えが&amp;'>画</ruby></color>こう　ドラマティック!<br /><b>夢は<color value=""Yellow""><ruby value=""むげん"" color=""Red"">∞</ruby></color></b>だよ　誰も　ストップできない。";
            text = @"Frequently ends her lines with “<color value='yellow'><ruby color='white' value='Nanodesu'>なのです</ruby></color>”, which is often used as a moe variation of '<ruby value='Desu' size='16'>です</ruby>'. Despite this usage, 'なのです' is not equivalent to 'です' and cannot be freely substituted.
<br />
<br />
<ruby value='Common Language Runtime' color='gray'>CLR</ruby>
<br />
<ruby value='統合言語クエリ'>LINQ</ruby> , <ruby value='LINQ'>統合言語クエリ</ruby>
<br />
<br />
名門・<ruby value='あらやだ'>荒矢田</ruby>高校に通っている容姿端麗かつ品行方正、成績優秀・スポーツ万能で評判の女子高生。体の全ての数値が黄金比であり、神の体型と言われている。さらに老若男女問わずに好かれている人柄だが、家に帰ると頭身がデフォルメ（二頭身キャラ）になり、「<ruby value='びもうと'>美妹</ruby>」からグータラな生活を好む「<ruby value='ひもうと'>干物妹</ruby>」と化す。
";
            //text = "あいうえ<ruby value='びもうと'>美妹</ruby>。かきくけこ。";
            //text = "substituted.";
            //text = "あいうえ<ruby value='び'>美</ruby>」";
            //text = "<ruby color='white' value='Nanodesu'>なのです</ruby>";
            //text = "<ruby value=\"B\">A</ruby>》";
            //text = "hogemogeaaabc<ruby aa>hoge</ruby>";
            var textSource = markupParser.Parse(text);
            textSource = markupParser.Parse(text);
            textSource = markupParser.Parse(text);
            textSource = markupParser.Parse(text);

            var textSourceBuilder = new TextSourceBuilder();
            textSourceBuilder
//                .Add(new TextCharacters("ハウハウ「はうはう」"))

//                .Add(new TextCharacters(@"本作の主人公にしてメインヒロイン。兄：タイヘイとアパート『コーポ吉田』の201号室にて二人暮らし。作中での名前は「うまる」と平仮名表記である。髪は亜麻色のロングヘアー。誕生日は9月26日。父親は大企業の土間コーポレーションの社長。

//名門：荒矢田（あらやだ）高校に通っている容姿端麗かつ品行方正、スポーツ万能・成績優秀で評判の女子高生。老若男女問わずに好かれているが、家に帰ると頭身がデフォルメ（『ハイスクール!奇面組』のような二頭身キャラ）になり、「美妹（びもうと）」からグータラな生活を好む「干物妹（ひもうと）」と化す。

//ハムスターを2匹飼育しており[注 3]、家ではハムスターを模したフード[注 4]をかぶり[注 5]白い無地のインナーシャツに赤色のスパッツといったラフな格好で過ごしている。オシャレには関心が薄いようで、外行きの服はあまり持っていない。制服時はニーハイソックスを着用している。

//「ぬへへ」「ぬっふっふ」といった独特な笑い方をすることがある。"))
//                .Add(new LineBreak())
//                .Add(new TextCharacters("だから!迷いながら　また"))
//                .Add(new TextCharactersRubyGroup("画", "えが"))
//                .Add(new TextCharacters("こう　ドラマティック!"))
//                .Add(new LineBreak())
//                .Add(new TextCharacters("夢は"))
//                .Add(new ConsoleTextModifier() { Color = ConsoleColor.Yellow })
//                .Add(new TextCharactersRubyGroup("∞", "むげん"))
//                .Add(new TextEndOfSegment())
//                .Add(new TextCharacters("だよ　誰も　ストップできない。"))
//                .Add(new LineBreak())

//                .Add(new ConsoleTextModifier() { IsBold = true })
//                .Add(new ConsoleTextModifier() { Color = ConsoleColor.Red })
//                .Add(new TextCharacters("ハロー"))
//                .Add(new TextEndOfSegment())
//                .Add(new TextCharactersRubyGroup("コンニチハ。", "Hello"))
//                .Add(new TextEndOfSegment())
//                .Add(new LineBreak())

//                .Add(new TextCharactersRubyGroup("邪王", "じゃおう"))
//                .Add(new TextCharactersRubyGroup("真眼", "しんがん"))
//                .Add(new LineBreak())
//                .Add(new TextCharactersRubyGroup("CLR", "共通言語基盤"))
//                .Add(new LineBreak())
//                .Add(new TextCharactersRubyGroup("共通言語基盤", "CLR"))
//                .Add(new LineBreak())

//                .Add(new TextCharactersRubyGroup("CLR", "Common Language Runtime"))
//                .Add(new LineBreak())
//                .Add(new TextCharacters("She has brown hair with a small ponytail that is slightly disheveled in a similar manner to her sisters."))
                .Add(new TextCharacters("になり、「"))
                .Add(new TextCharactersRubyGroup("美妹", "びもうと"))
                .Add(new TextCharacters("」からグータラな生活を好む「"))
                .Add(new TextCharactersRubyGroup("干物妹", "ひもうと"))
                .Add(new TextCharacters("」と化す。"))
            ;

            //var textSource = textSourceBuilder.ToTextSource();

            var textFormatter = new ConsoleTextFormatter();
            var textLineBreakState = new TextLineBreakState();

            Console.WriteLine();
            Console.WriteLine();

            Console.WindowWidth = 100;

            var startAt = DateTime.Now;
            while (true)
            {
                var textLine = textFormatter.FormatLine(textSource, 40, textLineBreakState);
                if (textLine == null)
                    break;

                //Console.WriteLine(textLine);

                var top = Console.CursorTop;
                //foreach (var g in textLine.PlacedGlyphs)
                var placedGlyphs = textLine.PlacedGlyphs.OrderBy(x => x.Index).ToArray();
                for (var i = 0; i < placedGlyphs.Length; i++)
                {
                    var g = placedGlyphs[i];

                    var elapsedSecond = (DateTime.Now - startAt).TotalSeconds * 20;
                    if (g.Index > elapsedSecond)
                    {
                        i--;
                        Thread.Sleep(100);
                        continue;
                    }

                    Console.CursorLeft = g.X;
                    Console.CursorTop = top + g.Y;
                    Console.CursorLeft = g.X;
                    Console.ForegroundColor = ((ConsoleGlyph) g.Glyph).Color;
                    Console.Write(((Glyph)g.Glyph).Character.ToString());
                }

                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
            }

            Console.ReadLine();
        }
    }

    class ConsoleMarkupParser : LetterWriterMarkupParser
    {
        protected override IEnumerable<TextRun> VisitMarkupElement(Element element, string tagNameUpper)
        {
            if (tagNameUpper == "COLOR")
            {
                yield return new ConsoleTextModifier() { Color = (ConsoleColor) Enum.Parse(typeof (ConsoleColor), element.Attributes["Value"], true) };
                foreach (var x in base.VisitMarkupElement(element, tagNameUpper)) yield return x;
                yield return TextEndOfSegment.Default;
                yield break;
            }

            if (tagNameUpper == "RUBY")
            {
                //if (element.Attributes.ContainsKey("color"))
                //{ 
                //    return
                //        new TextRun[] { new ConsoleTextModifier() { RubyColor = (ConsoleColor) Enum.Parse(typeof (ConsoleColor), element.Attributes["Color"]) } }
                //        .Concat(base.VisitMarkupElement(element, tagNameUpper))
                //        .Concat(new TextRun[] { new TextEndOfSegment() })
                //        .ToArray();
                //}
                yield return new ConsoleTextModifier() { };
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

    class ConsoleGlyph : Glyph
    {
        public ConsoleColor Color { get; set; }

        public ConsoleGlyph(char c)
            : base(c, Regex.IsMatch(c.ToString(), "^[a-zA-Z,.<>;:\"'/?~!@#$%^&*()_+`1234567890-={}\\[\\] ]$") ? 1 : 2, 1)
        { }
    }

    class ConsoleGlyphProvider : GlyphProvider<ConsoleTextModifierScope>
    {
        private Dictionary<char, ConsoleGlyph> _cache = new Dictionary<char, ConsoleGlyph>(10 * 1024);
        protected override void GetGlyphsFromStringCore(ConsoleTextModifierScope textModifierScope, string value, IList<IGlyph> buffer)
        {
            var color = textModifierScope.Color ?? ((textModifierScope.IsBold ?? false) ? ConsoleColor.White : ConsoleColor.Gray);
            foreach (var x in value)
            {
                ConsoleGlyph glyph;
                if (!this._cache.TryGetValue(x, out glyph) || glyph.Color != color)
                {
                    glyph = new ConsoleGlyph(x) { Color = color };
                    this._cache[x] = glyph;
                }

                buffer.Add(glyph);
            }
        }
    }

    class ConsoleTextFormatter : TextFormatter
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

    class ConsoleTextModifier : TextModifier
    {
        public bool? IsBold { get; set; }
        public ConsoleColor? Color { get; set; }
        public ConsoleColor? RubyColor { get; set; }
    }

    class ConsoleTextModifierScope : TextModifierScope<ConsoleTextModifier>, IRubyTextModifierScope
    {
        protected new ConsoleTextModifierScope Parent { get { return (ConsoleTextModifierScope) base.Parent; } }

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
                    new ConsoleTextModifier() {Color = this.RubyColor, IsBold = this.IsBold});
            }
        }
    }
}
