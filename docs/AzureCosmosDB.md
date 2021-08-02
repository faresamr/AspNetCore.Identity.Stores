## Azure Cosmos DB
This is how to use Azure Cosmos DB as storage for ASP.NET identity.

1. Create a new **ASP.NET Core Web App**
2. From **Additional information** dialog select authentication type **Individual Accounts**
3. Install [AspNetCore.Identity.Stores.AzureCosmosDB](https://www.nuget.org/packages/AspNetCore.Identity.Stores.AzureCosmosDB/) package from NuGet manager
    > <code>Install-Package AspNetCore.Identity.Stores.AzureCosmosDB</code>
4. In [Startup.cs](../AspNetCore.Identity.Stores/SampleWebApplication/Startup.cs#L27) replace the code inside **ConfigureServices** method with the folowing code
    ```csharp
    using AspNetCore.Identity.Stores;
    using AspNetCore.Identity.Stores.AzureCosmosDB.Extensions;
    ```  
    
    ```csharp
    //Adds data protection services, needed to protect the identity personal data
    services.AddDataProtection();

    //Configure identity repository connection
    services.Configure<IdentityStoresOptions>(options => options
        .UseAzureCosmosDB(Configuration.GetConnectionString("DefaultConnection"), "MyDatabase"));

    services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
        .AddRoles<IdentityRole>()
        .AddAzureCosmosDbStores(); //Add Identity stores
        
    services.AddRazorPages();
    ```  
5. In [appsettings.json](../AspNetCore.Identity.Stores/SampleWebApplication/appsettings.json) update DefaultConnection with Azure Cosmos DB connection string.

## Configuration
As shown in the code snippet from point 4, **IdentityStoresOptions** has an extension method **UseAzureCosmosDB** to configure the connection and it has 3 parameters:
- connectionString: To specify the connection string of CosmosDB instance.
- databaseId: To specify the database which will be used.
- containerId (optinal): To specify the container which will be used, the default value is **AspNetIdentity**

> **Note**: If the specified database or container not exist, they will get created on startup.

See more:
- [Example](../AspNetCore.Identity.Stores/SampleWebApplication)
- [Other storage providers](../README.md)