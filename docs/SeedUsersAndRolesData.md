## Seed Users And Roles Data
This is how to seed identity framework with users and roles.

1. Create a new **ASP.NET Core Web App**
2. From **Additional information** dialog select authentication type **Individual Accounts**
3. Install [AspNetCore.Identity.Stores](https://www.nuget.org/packages/AspNetCore.Identity.Stores/) package from NuGet manager
    > <code>Install-Package AspNetCore.Identity.Stores</code>
4. In [Startup.cs](../AspNetCore.Identity.Stores/SampleWebApplication/Startup.cs#L85) add the folowing code to **Configure(IApplicationBuilder app, IWebHostEnvironment env)** method with the folowing code
    ```csharp
    using AspNetCore.Identity.Stores;
    using Microsoft.AspNetCore.Identity;
    ```  
    
    ```csharp
    // Seed identity
    app.UseIdentitySeeding<IdentityUser, IdentityRole>(seeds =>
    {
        seeds.AddRole(new IdentityRole("Admin"))
                .AddRole(new IdentityRole("User"))
                .AddUser(new() { Email = "admin@sample.com", UserName = "admin@sample.com" }, "adminP@ssw0rd!", roles: new IdentityRole("Admin"))
                .AddUser(new() { Email = "user@sample.com", UserName = "user@sample.com" }, "userP@ssw0rd!", roles: new IdentityRole("User"));

        //Note: Username should be provided as its a required field in identity framework, and password should meet identity password requirements
    });
    ```  
## IdentitySeeds<TUser, TRole> Class
This class provide methods to define roles and users
- **AddRole**: Is used to add role and its identity claims if needed.
> <code>public IdentitySeeds<TUser, TRole> AddRole(TRole role, params Claim[] claims)</code>
- **AddUser**: Is used to add user and its identity claims and roles if needed.
> <code>public IdentitySeeds<TUser, TRole> AddUser(TUser user, string password, params Claim[] claims)</code>
> <code>public IdentitySeeds<TUser, TRole> AddUser(TUser user, string password, params TRole[] roles)</code>
> <code>public IdentitySeeds<TUser, TRole> AddUser(TUser user, string password, IEnumerable<Claim> claims = null, IEnumerable<TRole> roles = null)</code>

See more:
- [Example](../AspNetCore.Identity.Stores/SampleWebApplication/Startup.cs#L85)
- [Other storage providers](../README.md)