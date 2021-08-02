# Azure Storage Account
This is how to use Azure storage account as storage for ASP.NET identity.

1. Create a new **ASP.NET Core Web App**
2. From **Additional information** dialog select authentication type **Individual Accounts**
3. Install [AspNetCore.Identity.Stores.AzureStorageAccount](https://www.nuget.org/packages/AspNetCore.Identity.Stores.AzureStorageAccount/) package from NuGet manager.
4. In [Startup.cs](../AspNetCore.Identity.Stores/SampleWebApplication/Startup.cs#L27) replace the code inside **ConfigureServices** method with the folowing code
    ```csharp
    using AspNetCore.Identity.Stores;
    using AspNetCore.Identity.Stores.AzureStorageAccount.Extensions;
    ```  
    
    ```csharp
    //Adds data protection services, needed to protect the identity personal data
    services.AddDataProtection();

    //Configure identity repository connection
    services.Configure<IdentityStoresOptions>(options => options
        .UseAzureStorageAccount(Configuration.GetConnectionString("DefaultConnection")));

    services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
        .AddRoles<IdentityRole>()
        .AddAzureStorageAccountStores(); //Add Identity stores
        
    services.AddRazorPages();
    ```  
5. In [appsettings.json](../AspNetCore.Identity.Stores/SampleWebApplication/appsettings.json) update DefaultConnection with Azure Storage Account connection string
    > **_NOTE:_**  For local development you may use "UseDevelopmentStorage=true" as connection string to connect to Azure storage emulator.

## Configuration
As shown in the code snippet from point 4, **IdentityStoresOptions** has an extension method **UseAzureStorageAccount** to configure the connection and it has 2 parameters:
- connectionString: To specify the connection string of CosmosDB instance.
- tableName (optinal): To specify the table which will be used, the default value is **AspNetIdentity**

> **Note**: If the specified table not exists, it will get created on startup.

See more:
- [Example](../AspNetCore.Identity.Stores/SampleWebApplication)
- [Other storage providers](../README.md)