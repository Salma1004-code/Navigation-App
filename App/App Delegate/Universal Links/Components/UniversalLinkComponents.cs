using System;
using System.Collections.Generic;
using System.Web;

public class UniversalLinkComponents
{
    // Define a private static field to hold the host
    private static readonly string host = "https://soundscape-app.yourservicesdomain.com";

    // Define properties to hold path components and query items of the universal link
    public UniversalLinkPathComponents PathComponents { get; }
    public List<UriQueryItem> QueryItems { get; }

    // Define a property to generate the complete URL using the host, path, and query items
    public Uri Url
    {
        get
        {
            string path = PathComponents.VersionedPath;

            // Combine host and path to form the complete URL
            UriBuilder uriBuilder = new UriBuilder(new Uri(host));
            uriBuilder.Path += path;

            // Add query items to the URL
            if (QueryItems != null && QueryItems.Count > 0)
            {
                uriBuilder.Query = string.Join("&", QueryItems);
            }

            return uriBuilder.Uri;
        }
    }

    // Initialize UniversalLinkComponents from a given URL
    public UniversalLinkComponents(Uri url)
    {
        // Parse URL components
        string path = HttpUtility.UrlDecode(url.AbsolutePath);
        PathComponents = new UniversalLinkPathComponents(path);
        QueryItems = ParseQueryItems(url.Query);
    }

    // Initialize UniversalLinkComponents with given path and parameters
    public UniversalLinkComponents(string path, List<UriQueryItem> queryItems)
    {
        PathComponents = new UniversalLinkPathComponents(path);
        QueryItems = queryItems;
    }

    // Method to parse query items from URL
    private List<UriQueryItem> ParseQueryItems(string query)
    {
        if (string.IsNullOrEmpty(query))
            return null;

        var queryItems = new List<UriQueryItem>();
        var queryParams = HttpUtility.ParseQueryString(query);
        foreach (string key in queryParams.AllKeys)
        {
            queryItems.Add(new UriQueryItem(key, queryParams[key]));
        }
        return queryItems;
    }
}

public class UniversalLinkPathComponents
{
    public string VersionedPath { get; }

    public UniversalLinkPathComponents(string path)
    {
        // Assuming the versioned path is extracted from the URL path
        VersionedPath = path;
    }
}

public class UriQueryItem
{
    public string Key { get; }
    public string Value { get; }

    public UriQueryItem(string key, string value)
    {
        Key = key;
        Value = value;
    }

    public override string ToString()
    {
        return $"{HttpUtility.UrlEncode(Key)}={HttpUtility.UrlEncode(Value)}";
    }
}
