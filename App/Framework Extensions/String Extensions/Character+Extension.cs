public static class CharExtensions
{
    public static bool IsDigit(this char c)
    {
        return char.IsDigit(c);
    }

    public static bool IsWhitespaceOrNewline(this char c)
    {
        return char.IsWhiteSpace(c);
    }

    public static UnicodeScalar UnicodeScalar(this char c)
    {
        return new UnicodeScalar(c);
    }
}
