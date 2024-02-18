using System;
using System.Collections.Generic;
using System.Linq;

public class Token
{
    private string Separator { get; }
    private HashSet<string> Tokens { get; }
    public string TokenizedString { get; }

    private Token(HashSet<string> tokens, string separator)
    {
        Separator = separator;
        Tokens = tokens;
        TokenizedString = string.Join(separator, tokens.OrderBy(token => token));
    }

    public Token(string str, string separator)
        : this(new HashSet<string>(str.Split(new[] { separator }, StringSplitOptions.None)), separator)
    {
    }

    public Token Intersection(Token other)
    {
        var intersectionTokens = Tokens.Intersect(other.Tokens);
        return new Token(new HashSet<string>(intersectionTokens), Separator);
    }
}
