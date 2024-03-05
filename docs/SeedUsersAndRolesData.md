## Seed Users And Roles Data
This is how to seed identity framework with users and roles.

1. Create a new **ASP.NET Core Web App**
2. From **Additional information** dialog select authentication type **Individual Accounts**
3. Install [AspNetCore.Identity.Stores](https://www.nuget.org/packages/AspNetCore.Identity.Stores/) package from NuGet manager
    > <code>Install-Package AspNetCore.Identity.Stores</code>
4. Add the folowing code to [Program.cs](https://github.com/faresamr/AspNetCore.Identity.Stores/tree/main/AspNetCore.Identity.Stores/SampleWebApplication/Program.cs#L62L72)
    ```csharp
    using AspNetCore.Identity.Stores;
    using Microsoft.AspNetCore.Identity;
    ```  
    
    ```csharp
    // Seed identity
    await app.UseIdentitySeedingAsync<IdentityUser, IdentityRole>(seeds =>
    {
        seeds
            .AddRole(role: new IdentityRole("Admin"))
            .AddRole(role: new IdentityRole("User"))
            .AddUser(user: new() { Email = "admin@sample.com", UserName = "admin@sample.com", EmailConfirmed = true }, password: "adminP@ssw0rd!", roles: new IdentityRole("Admin"))
            .AddUser(user: new() { Email = "user@sample.com", UserName = "user@sample.com", EmailConfirmed = true }, password: "userP@ssw0rd!", roles: new IdentityRole("User"));

        //Note: Username should be provided as its a required field in identity framework and email should be marked as confirmed to allow login, also password should meet identity password requirements
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
- [Example](https://github.com/faresamr/AspNetCore.Identity.Stores/tree/main/AspNetCore.Identity.Stores/SampleWebApplication/Program.cs#L62L72)
- [Other storage providers](https://github.com/faresamr/AspNetCore.Identity.Stores/tree/main/README.md)