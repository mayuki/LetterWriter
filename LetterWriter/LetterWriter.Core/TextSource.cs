namespace LetterWriter
{
    public class TextSource
    {
        public TextRun[] TextRuns { get; private set; }

        public TextSource(TextRun[] textRuns)
        {
            this.TextRuns = textRuns;
        }
    }
}