using System.Collections.Generic;
using System.Linq;

namespace LetterWriter.Markup
{
    public class LetterWriterMarkupParser : LetterWriterMarkupParserBase
    {
        protected override TextRun[] VisitMarkupElement(Element element, string tagNameUpper)
        {
            if (tagNameUpper == "RUBY")
            {
                return new TextRun[] { new TextCharactersRubyGroup(element.TextContent, element.GetAttribute("Value")) };
            }

            if (tagNameUpper == "BR")
            {
                return new TextRun[] {new LineBreak()};
            }

            return base.VisitMarkupElement(element, tagNameUpper);
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
            return new TextSource(this.VisitMarkupNode(rootNode));
        }

        protected virtual TextRun[] VisitMarkupNode(MarkupNode node)
        {
            if (node is TextNode)
            {
                if (this.TreatNewLineAsLineBreak)
                {
                    var text = ((TextNode)node).TextContent;
                    var textCharacters = new List<TextRun>();
                    var textSegments = text.Split('\n');
                    for (var i = 0; i < textSegments.Length; i++)
                    {
                        if (i != 0)
                        {
                            textCharacters.Add(new LineBreak());
                        }
                        textCharacters.Add(new TextCharacters(textSegments[i].Trim()));
                    }
                    return textCharacters.ToArray();
                }
                else
                {
                    return new TextRun[] { new TextCharacters(((TextNode)node).TextContent.Replace("\n", "").Replace("\r", "")) };
                }
            }

            if (node is Element)
            {
                var element = (Element)node;
                return this.VisitMarkupElement(element, element.TagName.ToUpper());
            }

            return node.Children.SelectMany(x => this.VisitMarkupNode(x)).ToArray();
        }

        protected virtual TextRun[] VisitMarkupElement(Element element, string tagNameUpper)
        {
            return element.Children.SelectMany(x => this.VisitMarkupNode(x)).ToArray();
        }
    }
}
