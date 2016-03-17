using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using LetterWriter;
using LetterWriter.Markup;
using LetterWriter.Unity;
using LetterWriter.Unity.Components;
using LetterWriter.Unity.Markup;

namespace Assets.Scripts.Features.LetterWriter
{
    public class MyLetterWriterExtensibilityProvider : LetterWriterExtensibilityProvider
    {
        public override LetterWriterMarkupParser CreateMarkupParser()
        {
            return new MyUnityMarkupParser();
        }

        public override TextFormatter CreateTextFormatter(Font font, int fontSize, Color color)
        {
            return new MyUnityTextFormatter(font, fontSize, color);
        }
    }
}
