using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LetterWriter.Markup;
using LetterWriter.Tests.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LetterWriter.Tests
{
    [TestClass]
    public class TextFormatterTests
    {
        private TextFormatter _formatter = new ConsoleTextFormatter();
        private LetterWriterMarkupParser _markupParser = new ConsoleMarkupParser();

        [TestMethod]
        public void Empty()
        {
            var textSource = this._markupParser.Parse("");
            var textLines = this.FormatText(textSource, 6);

            textLines.Length.Is(0);
        }

        [TestMethod]
        public void Alphabet_01()
        {
            var textSource = this._markupParser.Parse("Hauhau");
            var textLines = this.FormatText(textSource, 100);

            textLines.Length.Is(1);
            textLines[0].PlacedGlyphs.Length.Is(6);
        }

        [TestMethod]
        public void Alphabet_02()
        {
            var textSource = this._markupParser.Parse("Hauhaumaumau");
            var textLines = this.FormatText(textSource, 6);

            ValidateTextWithoutRuby(textLines, @"
Hauhau
maumau
");
        }

        [TestMethod]
        public void Alphabet_03()
        {
            var textSource = this._markupParser.Parse("Hauhau maumau");
            var textLines = this.FormatText(textSource, 6);

            ValidateTextWithoutRuby(textLines, @"
Hauhau
maumau
");
        }

        [TestMethod]
        public void Alphabet_04()
        {
            var textSource = this._markupParser.Parse("Hauhau1 maumau");
            var textLines = this.FormatText(textSource, 6);

            ValidateTextWithoutRuby(textLines, @"
Hauhau
1
maumau
");
        }

        [TestMethod]
        public void Alphabet_With_Ruby_01()
        {
            var textSource = this._markupParser.Parse("Hauhaumaumau<ruby value=\"moge\">hoge</ruby>fuga");
            var textLines = this.FormatText(textSource, 6);

            ValidateTextWithoutRuby(textLines, @"
Hauhau
maumau
hoge
fuga
");
        }

        [TestMethod]
        public void Alphabet_With_Ruby_02()
        {
            var textSource = this._markupParser.Parse("<ruby value=\"moge\">hoge</ruby>Hauhaumaumaufuga");
            var textLines = this.FormatText(textSource, 6);

            ValidateTextWithoutRuby(textLines, @"
hoge
Hauhau
maumau
fuga
");
        }

        [TestMethod]
        public void Alphabet_With_Ruby_03()
        {
            var textSource = this._markupParser.Parse("<ruby value=\"moge\">hogehoge</ruby>Hauhaumaumaufuga");
            var textLines = this.FormatText(textSource, 6);

            ValidateTextWithoutRuby(textLines, @"
hogehoge
Hauhau
maumau
fuga
");
        }

        [TestMethod]
        public void Japanese_禁則句読点_行頭禁則_01()
        {
            var textSource = this._markupParser.Parse("現在、大宮家にホームステイ中。");
            var textLines = this.FormatText(textSource, 4); // ConsoleTextFormatterでは日本語文字1文字は2の幅をとることになっているので倍になる

            ValidateTextWithoutRuby(textLines, @"
現
在、
大宮
家に
ホー
ムス
テイ
中。
");
        }

        [TestMethod]
        public void Japanese_禁則句読点_行頭禁則_02()
        {
            var textSource = this._markupParser.Parse("涼風青葉「今日も一日がんばるぞい!」");
            var textLines = this.FormatText(textSource, 8); // ConsoleTextFormatterでは日本語文字1文字は2の幅をとることになっているので倍になる

            // JIS強い禁則オプションに応じる
            ValidateTextWithoutRuby(textLines, @"
涼風青葉
「今日も
一日がん
ばるぞ
い!」
");

        }

        [TestMethod]
        public void Japanese_禁則句読点_行末禁則_01()
        {
            var textSource = this._markupParser.Parse("涼風青葉「今日も一日がんばるぞい!」");
            var textLines = this.FormatText(textSource, 10); // ConsoleTextFormatterでは日本語文字1文字は2の幅をとることになっているので倍になる

            // 行末に「が来てはいけない
            ValidateTextWithoutRuby(textLines, @"
涼風青葉
「今日も一
日がんばる
ぞい!」
");

        }


        [DebuggerStepThrough]
        private void ValidateTextWithoutRuby(TextLine[] textLines, string expected)
        {
            var expectedLines = expected.Trim().Split('\n').Select(x => x.Trim()).ToArray();

            textLines.Length.Is(expectedLines.Length);

            for (var i = 0; i < expectedLines.Length; i++)
            {
                var joinedString = String.Join("", textLines[i].PlacedGlyphs
                    .Where(x => x.Y == 0) // Rubyは-1になってるのでそれを除く(地の文はY=0)
                    .Select(x => x.Glyph)
                    .OfType<Glyph>()
                    .Select(x => x.Character));
                joinedString.Is(expectedLines[i], "行:" + i);
            }
        }

        private TextLine[] FormatText(TextSource textSource, int width)
        {
            var textLines = new List<TextLine>();
            var state = new TextLineBreakState();

            TextLine textLine;
            while ((textLine = this._formatter.FormatLine(textSource, width, state)) != null)
            {
                textLines.Add(textLine);
            }

            return textLines.ToArray();
        }
    }
}
