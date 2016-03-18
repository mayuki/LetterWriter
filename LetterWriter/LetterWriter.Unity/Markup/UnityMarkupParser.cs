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


        protected override IEnumerable<TextRun> VisitMarkupElement(Element element, string tagNameUpper)
        {
            switch (tagNameUpper)
            {
                case "RUBY":
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

                    yield return new UnityTextModifier() { RubyColor = color, RubyFontScale = scale };
                    foreach (var x in base.VisitMarkupElement(element, tagNameUpper)) yield return x;
                    yield return TextEndOfSegment.Default;
                    break;

                case "COLOR":
                    var value = element.GetAttribute("Value");
                    if (!_colorTable.ContainsKey(value))
                    {
                        foreach (var x in base.VisitMarkupElement(element, tagNameUpper)) yield return x;
                    }
                    else
                    {
                        yield return new UnityTextModifier() { Color = this._colorTable[value] };
                        foreach (var x in base.VisitMarkupElement(element, tagNameUpper)) yield return x;
                        yield return TextEndOfSegment.Default;
                    }
                    break;

                case "SIZE":
                    var size = 0;
                    if (Int32.TryParse(element.GetAttribute("Value"), out size))
                    {
                        yield return new UnityTextModifier() { FontSize = size };
                        foreach (var x in base.VisitMarkupElement(element, tagNameUpper)) yield return x;
                        yield return TextEndOfSegment.Default;
                    }
                    else
                    {
                        foreach (var x in base.VisitMarkupElement(element, tagNameUpper)) yield return x;
                    }
                    break;

                case "B":
                    yield return new UnityTextModifier() { FontStyle = FontStyle.Bold };
                    foreach (var x in base.VisitMarkupElement(element, tagNameUpper)) yield return x;
                    yield return TextEndOfSegment.Default;
                    break;

                default:
                    foreach (var x in base.VisitMarkupElement(element, tagNameUpper)) yield return x;
                    break;
            }
        }
    }

}
