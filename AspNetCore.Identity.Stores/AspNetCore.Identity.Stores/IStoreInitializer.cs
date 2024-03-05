namespace AspNetCore.Identity.Stores;

public interface IStoreInitializer
{
    Task InitializeAsync();
}
