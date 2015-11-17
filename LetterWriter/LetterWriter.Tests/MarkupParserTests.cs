using System;
using LetterWriter.Markup;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LetterWriter.Tests
{
    [TestClass]
    public class MarkupParserTests
    {
        [TestMethod]
        public void Parse_Incompleted_Markup_01()
        {
            var markupParser = new LetterWriterMarkupParser();
            var result = markupParser.Parse("ab<ruby>c");

            result.TextRuns.Length.Is(2);
            result.TextRuns[0].IsInstanceOf<TextCharacters>().Is(x => x.RawCharacters == "ab");
            result.TextRuns[1].IsInstanceOf<TextCharactersRubyGroup>().Is(x => x.RawCharacters == "c");
        }

        [TestMethod]
        public void Parse_Incompleted_Markup_Attribute_01()
        {
            var markupParser = new LetterWriterMarkupParser();
            var result = markupParser.Parse("aa<ruby bb>cc</ruby>");

            result.TextRuns.Length.Is(2);
            result.TextRuns[0].IsInstanceOf<TextCharacters>().Is(x => x.RawCharacters == "aa");
            result.TextRuns[1].IsInstanceOf<TextCharactersRubyGroup>();//.Is(x => x.RawCharacters == "cc");
        }

        [TestMethod]
        public void Parse_Markup_01()
        {
            var markupParser = new LetterWriterMarkupParser();
            var result = markupParser.Parse("<ruby color='white' value='Nanodesu'>なのです</ruby>");

            result.TextRuns.Length.Is(1);
            result.TextRuns[0].IsInstanceOf<TextCharactersRubyGroup>().Is(x => x.RawCharacters == "なのです" && x.RawRubyCharacters == "Nanodesu");
        }


        [TestMethod]
        public void TreatNewLineAsLineBreakTest()
        {
            var markupParser = new LetterWriterMarkupParser() { TreatNewLineAsLineBreak = true };
            var result = markupParser.Parse("abc\ndef");

            result.TextRuns.Length.Is(3);
            result.TextRuns[0].IsInstanceOf<TextCharacters>().Is(x => x.RawCharacters == "abc");
            result.TextRuns[1].IsInstanceOf<LineBreak>();
            result.TextRuns[2].IsInstanceOf<TextCharacters>().Is(x => x.RawCharacters == "def");
        }

        [TestMethod]
        public void TreatNewLineAsLineBreakDisabledTest()
        {
            var markupParser = new LetterWriterMarkupParser() { TreatNewLineAsLineBreak = false };
            var result = markupParser.Parse("abc\ndef");

            result.TextRuns.Length.Is(1);
            result.TextRuns[0].IsInstanceOf<TextCharacters>().Is(x => x.RawCharacters == "abcdef");
        }
    }
}
