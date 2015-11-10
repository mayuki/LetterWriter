using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LetterWriter.Markup;
using LetterWriter.Unity.Components;
using UnityEngine;

namespace LetterWriter.Unity.Markup
{
    public class UnityMarkupParser : LetterWriter.Markup.LetterWriterMarkupParser
    {
        private Dictionary<string, Color> _colorTable = new Dictionary<string, Color>(StringComparer.OrdinalIgnoreCase)
        {
            {"red", new Color(1f, 0.0f, 0.0f, 1f)},
            {"green", new Color(0.0f, 1f, 0.0f, 1f)},
            {"blue", new Color(0.0f, 0.0f, 1f, 1f)},
            {"white", new Color(1f, 1f, 1f, 1f)},
            {"black", new Color(0.0f, 0.0f, 0.0f, 1f)},
            {"yellow", new Color(1f, 0.9215686f, 0.01568628f, 1f)},
            {"cyan", new Color(0.0f, 1f, 1f, 1f)},
            {"magenta", new Color(1f, 0.0f, 1f, 1f)},
            {"gray", new Color(0.5f, 0.5f, 0.5f, 1f)},
            {"grey", new Color(0.5f, 0.5f, 0.5f, 1f)},
            {"clear", new Color(0.0f, 0.0f, 0.0f, 0.0f)},
        };


        protected override TextRun[] VisitMarkupElement(Element element, string tagNameUpper)
        {
            if (tagNameUpper == "RUBY")
            {
                Color? color = null;
                float? scale = null;

                if (element.Attributes.ContainsKey("color"))
                {
                    color = this._colorTable[element.Attributes["Color"]];
                }
                if (element.Attributes.ContainsKey("scale"))
                {
                    var tmpSize = 0f;
                    if (Single.TryParse(element.Attributes["Scale"], out tmpSize))
                    {
                        scale = tmpSize;
                    }
                }

                return
                        new TextRun[] { new UnityTextModifier() { RubyColor = color, RubyFontScale = scale } }
                        .Concat(base.VisitMarkupElement(element, tagNameUpper))
                        .Concat(new TextRun[] { new TextEndOfSegment() })
                        .ToArray();
            }

            if (tagNameUpper == "COLOR")
            {
                if (!_colorTable.ContainsKey(element.Attributes["Value"]))
                    return base.VisitMarkupElement(element, tagNameUpper);

                return
                    new TextRun[]
                    {
                        new UnityTextModifier() { Color = this._colorTable[element.Attributes["Value"]] }
                    }
                    .Concat(base.VisitMarkupElement(element, tagNameUpper))
                    .Concat(new TextRun[] { new TextEndOfSegment() })
                    .ToArray();
            }
            if (tagNameUpper == "SIZE")
            {
                var size = 0;
                if (Int32.TryParse(element.Attributes["Value"], out size))
                {
                    return
                        new TextRun[]
                        {
                            new UnityTextModifier() { FontSize = size }
                        }
                        .Concat(base.VisitMarkupElement(element, tagNameUpper))
                        .Concat(new TextRun[] { new TextEndOfSegment() })
                        .ToArray();
                }
                else
                {
                    return base.VisitMarkupElement(element, tagNameUpper);
                }
            }

            if (tagNameUpper == "B")
            {
                return new TextRun[]
                    {
                        new UnityTextModifier() { FontStyle = FontStyle.Bold }
                    }
                    .Concat(base.VisitMarkupElement(element, tagNameUpper))
                    .Concat(new TextRun[] { new TextEndOfSegment() })
                    .ToArray();
            }

            return base.VisitMarkupElement(element, tagNameUpper);
        }
    }

}
