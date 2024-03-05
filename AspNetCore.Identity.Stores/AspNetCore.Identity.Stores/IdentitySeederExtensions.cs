using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;

namespace AspNetCore.Identity.Stores;

public static class IdentitySeederExtensions
{
    [Obsolete($"[Deprecated] Use {nameof(UseIdentitySeedingAsync)}", true)]
    public static IHost UseIdentitySeeding<TUser, TRole>(this IHost app, Action<IdentitySeeds<TUser, TRole>> seeds)
        where TUser : IdentityUser<string>
        where TRole : IdentityRole<string> => throw new NotSupportedException();

    public static async Task UseIdentitySeedingAsync<TUser, TRole>(this IHost app, Action<IdentitySeeds<TUser, TRole>> seeds)
        where TUser : IdentityUser<string>
        where TRole : IdentityRole<string>
    {
        IdentitySeeds<TUser, TRole> identitySeeds = new();
        seeds(identitySeeds);
        using var scope = app.Services.CreateScope();

        if(scope.ServiceProvider.GetService<IStoreInitializer>() is IStoreInitializer storeInitializer)
        {
            await storeInitializer.InitializeAsync();
        }

        RoleManager<TRole>? roleManager = scope.ServiceProvider.GetService<RoleManager<TRole>>();
        if (roleManager is not null)
        {
            await AddRolesAsync(roleManager, identitySeeds);
        }

        await AddUsersAsync(scope.ServiceProvider.GetRequiredService<UserManager<TUser>>(), roleManager, identitySeeds);
    }

    private static async Task AddRolesAsync<TUser, TRole>(RoleManager<TRole> roleManager, IdentitySeeds<TUser, TRole> identitySeeds)
        where TUser : IdentityUser<string>
        where TRole : IdentityRole<string>
    {
        foreach (var role in identitySeeds.Roles)
        {
            if (role.Role.Name is string name && !await roleManager.RoleExistsAsync(name))
            {
                IdentityResult result = await roleManager.CreateAsync(role.Role);
                if (result.Succeeded)
                {
                    foreach (Claim claim in role.Claims)
                    {
                        await roleManager.AddClaimAsync(role.Role, claim);
                    }
                }
            }
        }
    }

    private static async Task AddUsersAsync<TUser, TRole>(UserManager<TUser> userManager, RoleManager<TRole>? roleManager, IdentitySeeds<TUser, TRole> identitySeeds)
        where TUser : IdentityUser<string>
        where TRole : IdentityRole<string>
    {
        foreach (var user in identitySeeds.Users)
        {
            if (user.User.Email is not string email || await userManager.FindByEmailAsync(email) != null)
            {
                continue;
            }
            IdentityResult result = await userManager.CreateAsync(user.User, user.Password);
            if (!result.Succeeded)
            {
                continue;
            }
            if (user.Claims?.Any() == true)
            {
                foreach (Claim claim in user.Claims)
                {
                    await userManager.AddClaimAsync(user.User, claim);
                }
            }
            if (roleManager is not null && user.Roles?.Any() == true)
            {
                foreach (TRole role in user.Roles)
                {
                    if (role.Name is not null)
                    {
                        if (!identitySeeds.Roles.Any(i => i.Role.Name == role.Name)
                            && !await roleManager.RoleExistsAsync(role.Name))
                        {
                            await roleManager.CreateAsync(role);
                        }
                        await userManager.AddToRoleAsync(user.User, role.Name);
                    }
                }
            }
        }
    }
}
