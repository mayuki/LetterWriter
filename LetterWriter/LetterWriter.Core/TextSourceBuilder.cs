using System.Collections.Generic;

namespace LetterWriter
{
    public class TextSourceBuilder
    {
        private List<TextRun> _textRuns = new List<TextRun>();

        public TextSourceBuilder Add(TextRun textRun)
        {
            this._textRuns.Add(textRun);

            return this;
        }

        public TextSource ToTextSource()
        {
            return new TextSource(_textRuns.ToArray());
        }
    }
}