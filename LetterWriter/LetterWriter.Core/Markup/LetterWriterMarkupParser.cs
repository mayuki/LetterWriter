using System.Collections.Generic;
using System.Linq;

namespace LetterWriter.Markup
{
    public class LetterWriterMarkupParser : LetterWriterMarkupParserBase
    {
        protected override IEnumerable<TextRun> VisitMarkupElement(Element element, string tagNameUpper)
        {
            if (tagNameUpper == "RUBY")
            {
                yield return new TextCharactersRubyGroup(element.TextContent, element.GetAttribute("Value"));
                yield break;
            }

            if (tagNameUpper == "BR")
            {
                yield return LineBreak.Default;
                yield break;
            }

            foreach (var x in base.VisitMarkupElement(element, tagNameUpper)) yield return x;
        }
    }

    public class LetterWriterMarkupParserBase : LightweightMarkupParser
    {
        public new TextSource Parse(string value)
        {
            return this.ConvertNodeToTextSource(((LightweightMarkupParser) this).Parse(value));
        }

        protected virtual TextSource ConvertNodeToTextSource(MarkupNode rootNode)
        {
            return new TextSource(this.VisitMarkupNode(rootNode).ToArray());
        }

        protected virtual IEnumerable<TextRun> VisitMarkupNode(MarkupNode node)
        {
            if (node is TextNode)
            {
                if (this.TreatNewLineAsLineBreak)
                {
                    var text = ((TextNode)node).TextContent;
                    //var textCharacters = new List<TextRun>();
                    var textSegments = text.Split('\n');
                    for (var i = 0; i < textSegments.Length; i++)
                    {
                        if (i != 0)
                        {
                            yield return LineBreak.Default;
                        }
                        yield return new TextCharacters(textSegments[i].Trim());
                    }
                    yield break;
                }
                else
                {
                    yield return new TextCharacters(((TextNode)node).TextContent.Replace("\n", "").Replace("\r", ""));
                    yield break;
                }
            }

            if (node is Element)
            {
                var element = (Element)node;
                foreach (var x in this.VisitMarkupElement(element, element.TagName.ToUpper())) yield return x;
                yield break;
            }

            for (var i = 0; i < node.Children.Count; i++)
            {
                foreach (var x in this.VisitMarkupNode(node.Children[i])) yield return x;
            }
        }

        protected virtual IEnumerable<TextRun> VisitMarkupElement(Element element, string tagNameUpper)
        {
            for (var i = 0; i < element.Children.Count; i++)
            {
                foreach (var x in this.VisitMarkupNode(element.Children[i])) yield return x;
            }
        }
    }
}
