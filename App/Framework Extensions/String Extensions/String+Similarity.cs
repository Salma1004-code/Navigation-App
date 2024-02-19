using System;
using System.Collections.Generic;
using System.Linq;

public static class StringSimilarityExtensions
{
    public static double TokenSort(this string str, string other)
    {
        var tokenA = Tokenize(str);
        var tokenB = Tokenize(other);

        return tokenA.TokenizedString.WeightedMinimumEditDistance(tokenB.TokenizedString);
    }

    public static double TokenSet(this string str, string other)
    {
        var tokenA = Tokenize(str);
        var tokenB = Tokenize(other);

        var intersection = tokenA.Intersection(tokenB);
        var editDistanceA = intersection.TokenizedString.WeightedMinimumEditDistance(tokenA.TokenizedString);
        var editDistanceB = intersection.TokenizedString.WeightedMinimumEditDistance(tokenB.TokenizedString);

        return (editDistanceA + editDistanceB) / 2.0;
    }

    private static Token Tokenize(string str, string separator = " ")
    {
        return new Token(str.ToLower(), separator);
    }

    private static double WeightedMinimumEditDistance(this string str, string other)
    {
        var editDistance = str.MinimumEditDistance(other);
        var maxDistance = Math.Max(str.Length, other.Length);

        if (maxDistance <= 0 || editDistance >= maxDistance)
            return 1.0;

        return (double)editDistance / maxDistance;
    }

    private static int MinimumEditDistance(this string str, string other)
    {
        if (string.IsNullOrWhiteSpace(str) || string.IsNullOrWhiteSpace(other))
            return int.MaxValue;

        if (str == other)
            return 0;

        var m = str.Length;
        var n = other.Length;
        var matrix = new int[m + 1, n + 1];

        for (int index = 1; index <= m; index++)
            matrix[index, 0] = index;

        for (int index = 1; index <= n; index++)
            matrix[0, index] = index;

        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (str[i] == other[j])
                    matrix[i + 1, j + 1] = matrix[i, j];
                else
                    matrix[i + 1, j + 1] = Math.Min(matrix[i, j] + 1,
                                                      Math.Min(matrix[i + 1, j] + 1,
                                                               matrix[i, j + 1] + 1));
            }
        }

        return matrix[m, n];
    }

    private class Token
    {
        public Token(string str, string separator)
        {
            Tokens = str.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public List<string> Tokens { get; }

        public Token Intersection(Token other)
        {
            var intersect = Tokens.Intersect(other.Tokens, StringComparer.OrdinalIgnoreCase).ToList();
            return new Token(string.Join(" ", intersect), " ");
        }

        public string TokenizedString => string.Join(" ", Tokens);
    }
}
