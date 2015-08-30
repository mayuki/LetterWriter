namespace LetterWriter
{
    public interface IGlyph
    {
        int Height { get; }
        int AdvanceWidth { get; }

        bool IsWhiteSpaceOrControl { get; }
    }
}