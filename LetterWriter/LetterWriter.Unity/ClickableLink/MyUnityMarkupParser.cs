using LetterWriter.Unity.Markup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LetterWriter;
using LetterWriter.Markup;
using LetterWriter.Unity;

namespace Assets.Scripts.Features.LetterWriter
{
    public class MyUnityMarkupParser : UnityMarkupParser
    {
        protected override TextRun[] VisitMarkupElement(Element element, string tagNameUpper)
        {
            if (tagNameUpper == "A")
            {
                var value = element.GetAttribute("Href");

                return
                    new TextRun[]
                    {
                        new MyTextModifier() { Href = value }
                    }
                    .Concat(base.VisitMarkupElement(element, tagNameUpper))
                    .Concat(new TextRun[] { new TextEndOfSegment() })
                    .ToArray();
            }

            return base.VisitMarkupElement(element, tagNameUpper);
        }
    }

}
