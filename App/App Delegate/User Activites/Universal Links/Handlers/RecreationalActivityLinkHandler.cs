using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class RecreationalActivityLinkHandler : UniversalLinkHandler
{
    // Define a struct to hold parameters
    private struct Parameters
    {
        public string Id { get; }

        public Parameters(string id)
        {
            Id = id;
        }
    }

    // Implement the handleUniversalLink method to handle universal links
    public void HandleUniversalLink(List<UriQueryItem> queryItems, UniversalLinkVersion version)
    {
        Parameters parameters = ParseQueryItems(queryItems);
        if (parameters.Id == null)
        {
            // Failed to parse the expected parameters
            Task.Run(() => ActivityDownloadDidFail());
            return;
        }

        if (AuthoredActivityLoader.Shared.ActivityExists(parameters.Id))
        {
            // If the recreational activity is already downloaded, transition to the UI to show the list of downloaded activities
            Task.Run(() => ProcessedActivityDeepLink());
        }
        else
        {
            // Otherwise, download the recreational activity
            DownloadRecreationalActivity(parameters.Id, version);
        }
    }

    // Parse query items to extract parameters
    private Parameters ParseQueryItems(List<UriQueryItem> queryItems)
    {
        if (queryItems == null)
        {
            Console.WriteLine("Recreational activity universal link is invalid");
            return new Parameters();
        }

        string id = queryItems.Find(item => item.Key == "id")?.Value;
        if (id == null)
        {
            Console.WriteLine("Recreational activity universal link is invalid - 'id' is required");
            return new Parameters();
        }

        return new Parameters(id);
    }

    // Handle success in processing the recreational activity
    private void ProcessedActivityDeepLink()
    {
        NotificationCenter.Default.Post(Notification.ActivityProcessedDeepLink, this);
    }

    // Handle failure to download the recreational activity
    private void ActivityDownloadDidFail()
    {
        NotificationCenter.Default.Post(Notification.ActivityDownloadDidFail, this);
    }

    // Download the recreational activity asynchronously
    private async void DownloadRecreationalActivity(string id, UniversalLinkVersion version)
    {
        try
        {
            await AuthoredActivityLoader.Shared.Add(id, version);
            ProcessedActivityDeepLink();
        }
        catch (Exception)
        {
            ActivityDownloadDidFail();
        }
    }
}
