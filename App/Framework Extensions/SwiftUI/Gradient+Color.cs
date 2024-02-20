using System.Windows.Media;

public static class GradientExtensions
{
    public static LinearGradientBrush Purple
    {
        get
        {
            return new LinearGradientBrush
            {
                GradientStops = new GradientStopCollection
                {
                    new GradientStop((Color)ColorConverter.ConvertFromString("#2E1FCA"), 0),
                    new GradientStop((Color)ColorConverter.ConvertFromString("#7137CA"), 1)
                }
            };
        }
    }

    public static LinearGradientBrush Blue
    {
        get
        {
            return new LinearGradientBrush
            {
                GradientStops = new GradientStopCollection
                {
                    new GradientStop((Color)ColorConverter.ConvertFromString("#0782D0"), 0),
                    new GradientStop((Color)ColorConverter.ConvertFromString("#3E72BF"), 1)
                }
            };
        }
    }

    public static LinearGradientBrush DarkBlue
    {
        get
        {
            return new LinearGradientBrush
            {
                GradientStops = new GradientStopCollection
                {
                    new GradientStop((Color)ColorConverter.ConvertFromString("#064332"), 0),
                    new GradientStop((Color)ColorConverter.ConvertFromString("#194A9B"), 1)
                }
            };
        }
    }
}
