using System.Collections.Generic;
using System.Web;

public interface IUniversalLinkParameters
{
    // Constructor to parse query items
    void ParseQueryItems(List<UriQueryItem> queryItems);
    
    // Property to get query items
    List<UriQueryItem> QueryItems { get; }
}

public static class UniversalLinkParametersExtensions
{
    // Compute property to return percent encoded query items
    public static List<UriQueryItem> PercentEncodedQueryItems(this IUniversalLinkParameters parameters)
    {
        var percentEncodedQueryItems = new List<UriQueryItem>();
        
        // Define allowed characters for percent encoding
        var allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        
        // Iterate through query items
        foreach (var item in parameters.QueryItems)
        {
            // Manually encode the value of the existing URLQueryItem
            var encodedValue = HttpUtility.UrlEncode(item.Value);
            // Append the encoded value to percentEncodedQueryItems
            percentEncodedQueryItems.Add(new UriQueryItem(item.Name, encodedValue));
        }
        
        return percentEncodedQueryItems;
    }
}
