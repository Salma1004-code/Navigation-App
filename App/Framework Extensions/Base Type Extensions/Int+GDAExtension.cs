public static class IntExtensions
{
    private static string[] unitsMap = { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
    private static string[] tensMap = { "Zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

    public static string SpellOutNumber(this int number)
    {
        if (number == 0)
            return unitsMap[0];

        if (number < 0)
            return $"Minus {SpellOutNumber(-number)}";

        var parts = new List<string>();

        if ((number / 1000000) > 0)
        {
            parts.Add($"{SpellOutNumber(number / 1000000)} Million");
            number %= 1000000;
        }

        if ((number / 1000) > 0)
        {
            parts.Add($"{SpellOutNumber(number / 1000)} Thousand");
            number %= 1000;
        }

        if ((number / 100) > 0)
        {
            parts.Add($"{SpellOutNumber(number / 100)} Hundred");
            number %= 100;
        }

        if (number > 0)
        {
            if (parts.Count != 0)
                parts.Add("And");

            if (number < 20)
                parts.Add(unitsMap[number]);
            else
            {
                var lastPart = tensMap[number / 10];
                if ((number % 10) > 0)
                    lastPart += $"-{unitsMap[number % 10]}";

                parts.Add(lastPart);
            }
        }

        return string.Join(" ", parts);
    }
}
