namespace Soundscape
{
    public interface IAsyncAuthorizationProvider : IAuthorizationProvider
    {
        IAsyncAuthorizationProviderDelegate AuthorizationDelegate { get; set; }
    }
}
