public static class DateComponentsFormatterExtensions
{
    public static DateComponentsFormatter TimeElapsedFormatter
    {
        get
        {
            var formatter = new DateComponentsFormatter();
            formatter.AllowedUnits = DateComponentsFormatter.Unit.Second | DateComponentsFormatter.Unit.Minute;
            formatter.MaximumUnitCount = 0;
            formatter.UnitsStyle = DateComponentsFormatter.UnitsStyle.Positional;
            formatter.ZeroFormattingBehavior = DateComponentsFormatter.ZeroFormattingBehavior.Pad;

            return formatter;
        }
    }

    public static DateComponentsFormatter AccessibilityTimeElapsedFormatter
    {
        get
        {
            var formatter = new DateComponentsFormatter();
            formatter.AllowedUnits = DateComponentsFormatter.Unit.Second | DateComponentsFormatter.Unit.Minute | DateComponentsFormatter.Unit.Hour;
            formatter.MaximumUnitCount = 2;
            formatter.UnitsStyle = DateComponentsFormatter.UnitsStyle.Full;

            return formatter;
        }
    }
}
