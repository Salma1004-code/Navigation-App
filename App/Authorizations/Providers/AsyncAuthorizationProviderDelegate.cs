namespace Soundscape
{
    public interface IAsyncAuthorizationProviderDelegate
    {
        void AuthorizationDidChange(AuthorizationStatus authorization);
    }

    // Note: In C#, there's no direct equivalent to AnyObject. But since interfaces in C# can only be implemented by classes,
    // we don't need to specify any constraint for class types.
}
