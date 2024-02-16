using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ShareMarkerLinkHandler : UniversalLinkHandler
{
    // Define keys for extracting data from query items
    private static class Keys
    {
        public const string Location = "Location";
        public const string Nickname = "Nickname";
        public const string Annotation = "Annotation";
    }

    // Implement the HandleUniversalLink method to handle universal links
    public void HandleUniversalLink(List<UriQueryItem> queryItems, UniversalLinkVersion version)
    {
        if (queryItems == null)
        {
            GDLogUniversalLinkError("Universal link is invalid - queryItems is null");
            DidFailToImportMarker();
            return;
        }

        MarkerParameters markerParameters = MarkerParameters.Parse(queryItems);
        if (markerParameters == null)
        {
            GDLogUniversalLinkError("Universal link is invalid - Failed to parse a MarkerParameters object from query items");
            DidFailToImportMarker();
            return;
        }

        Handle(markerParameters);
    }

    // Handle the marker parameters extracted from the universal link
    private async void Handle(MarkerParameters markerParameters)
    {
        // Fetch the underlying entity associated with the marker location
        try
        {
            var entity = await markerParameters.Location.FetchEntity();
            ImportMarker(markerParameters, entity);
        }
        catch (Exception error)
        {
            GDLogUniversalLinkError($"Error loading underlying entity: {error}");
            DidFailToImportMarker();
        }
    }

    // Notify that the marker import was successful
    private void ImportMarker(MarkerParameters markerParameters, POI location)
    {
        var userInfo = new Dictionary<string, object>
        {
            { Keys.Location, location }
        };

        if (!string.IsNullOrEmpty(markerParameters.Nickname))
        {
            userInfo[Keys.Nickname] = markerParameters.Nickname;
        }

        if (!string.IsNullOrEmpty(markerParameters.Annotation))
        {
            userInfo[Keys.Annotation] = markerParameters.Annotation;
        }

        NotificationCenter.Default.Post(Notification.DidImportMarker, this, userInfo);
    }

    // Notify that the marker import failed
    private void DidFailToImportMarker()
    {
        NotificationCenter.Default.Post(Notification.DidFailToImportMarker, this);
    }
}
