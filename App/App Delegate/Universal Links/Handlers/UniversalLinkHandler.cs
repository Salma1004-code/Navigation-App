using System.Collections.Generic;

public interface UniversalLinkHandler
{
    // Declare a method for handling universal links
    void HandleUniversalLink(List<UriQueryItem> queryItems, UniversalLinkVersion version);
}
