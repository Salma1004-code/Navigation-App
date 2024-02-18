using System;

namespace Soundscape
{
    public enum AuthorizationStatus
    {
        NotDetermined,
        Authorized,
        Denied
    }

    public interface IAuthorizationProvider
    {
        AuthorizationStatus AuthorizationStatus { get; }
        void RequestAuthorization();
    }

    public static class AuthorizationProviderExtensions
    {
        public static bool IsAuthorized(this IAuthorizationProvider provider)
        {
            return provider.AuthorizationStatus == AuthorizationStatus.Authorized;
        }

        public static bool DidRequestAuthorization(this IAuthorizationProvider provider)
        {
            return provider.AuthorizationStatus != AuthorizationStatus.NotDetermined;
        }
    }
}
