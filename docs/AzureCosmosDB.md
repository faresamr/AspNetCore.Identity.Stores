## Azure Cosmos DB
This is how to use Azure Cosmos DB as storage for ASP.NET identity.

1. Create a new **ASP.NET Core Web App**
2. From **Additional information** dialog select authentication type **Individual Accounts**
3. Install [AspNetCore.Identity.Stores.AzureCosmosDB](https://www.nuget.org/packages/AspNetCore.Identity.Stores.AzureCosmosDB/) package from NuGet manager
    > <code>Install-Package AspNetCore.Identity.Stores.AzureCosmosDB</code>
4. In [Program.cs](https://github.com/faresamr/AspNetCore.Identity.Stores/tree/main/AspNetCore.Identity.Stores/SampleWebApplication/Program.cs#L24L30) replace DbContext and Identity initialization with the folowing code
    ```csharp
    using AspNetCore.Identity.Stores;
    using AspNetCore.Identity.Stores.AzureCosmosDB.Extensions;
    ```  
    
    ```csharp
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    //Configure identity repository connection
    builder.Services.Configure<IdentityStoresOptions>(options => options
                    .UseAzureCosmosDB(connectionString, databaseId: "MyDatabase"));

    builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddRoles<IdentityRole>()
                    .AddAzureCosmosDbStores(); //Add Identity stores
    ```  
5. In [appsettings.json](https://github.com/faresamr/AspNetCore.Identity.Stores/tree/main/AspNetCore.Identity.Stores/SampleWebApplication/appsettings.json) update DefaultConnection with Azure Cosmos DB connection string.

## Configuration
As shown in the code snippet from point 4, **IdentityStoresOptions** has an extension method **UseAzureCosmosDB** to configure the connection, and it has 3 parameters:
- connectionString: To specify the connection string of CosmosDB instance.
- databaseId: To specify the database which will be used.
- containerId (optinal): To specify the container which will be used, the default value is **AspNetIdentity**

> **Note**: If the specified database or container not exist, they will get created on startup if [UseIdentitySeedingAsync](https://github.com/faresamr/AspNetCore.Identity.Stores/tree/main/docs/SeedUsersAndRolesData.md) is user, otherwise an exception will be thrown.

See more:
- [Example](https://github.com/faresamr/AspNetCore.Identity.Stores/tree/main/AspNetCore.Identity.Stores/SampleWebApplication)
- [Other storage providers](https://github.com/faresamr/AspNetCore.Identity.Stores/tree/main/README.md)