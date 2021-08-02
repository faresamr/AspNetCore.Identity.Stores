[![Publish Packages](https://github.com/faresamr/AspNetCore.Identity.Stores/actions/workflows/AspNetCore.Identity.Stores-publish.yml/badge.svg)](https://github.com/faresamr/AspNetCore.Identity.Stores/actions/workflows/AspNetCore.Identity.Stores-publish.yml)

# Getting started
This repo provide custom identity stores for ASP.NET to use different instead of Entity Framework
- Azure Storage Account
- Azure Cosmos DB

## Azure Storage Account
1. Create a new **ASP.NET Core Web App**
2. From **Additional information** dialog select authentication type **Individual Accounts**
3. Install [AspNetCore.Identity.Stores.AzureStorageAccount](https://www.nuget.org/packages/AspNetCore.Identity.Stores.AzureStorageAccount/) package from NuGet manager
    > <code>Install-Package AspNetCore.Identity.Stores.AzureStorageAccount</code>
4. In [Startup.cs](AspNetCore.Identity.Stores/SampleWebApplication/Startup.cs#L27) replace the code inside **ConfigureServices** method with the folowing code
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
5. In [appsettings.json](AspNetCore.Identity.Stores/SampleWebApplication/appsettings.json) update DefaultConnection with Azure Storage Account connection string
    > **_NOTE:_**  For local development you may use "UseDevelopmentStorage=true" as connection string to connect to Azure storage emulator.

## Azure Cosmos DB
1. Create a new **ASP.NET Core Web App**
2. From **Additional information** dialog select authentication type **Individual Accounts**
3. Install [AspNetCore.Identity.Stores.AzureCosmosDB](https://www.nuget.org/packages/AspNetCore.Identity.Stores.AzureCosmosDB/) package from NuGet manager
    > <code>Install-Package AspNetCore.Identity.Stores.AzureCosmosDB</code>
4. In [Startup.cs](AspNetCore.Identity.Stores/SampleWebApplication/Startup.cs#L27) replace the code inside **ConfigureServices** method with the folowing code
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
5. In [appsettings.json](AspNetCore.Identity.Stores/SampleWebApplication/appsettings.json) update DefaultConnection with Azure Cosmos DB connection string.
