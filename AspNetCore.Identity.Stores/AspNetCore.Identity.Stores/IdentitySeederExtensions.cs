using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Security.Claims;

namespace AspNetCore.Identity.Stores;

public static class IdentitySeederExtensions
{
    public static IApplicationBuilder UseIdentitySeeding<TUser, TRole>(this IApplicationBuilder app, Action<IdentitySeeds<TUser, TRole>> seeds)
        where TUser : IdentityUser<string>
        where TRole : IdentityRole<string>
    {
        IdentitySeeds<TUser, TRole> identitySeeds = new();
        seeds(identitySeeds);
        using (var scope = app.ApplicationServices.CreateScope())
        {
            RoleManager<TRole>? roleManager = scope.ServiceProvider.GetService<RoleManager<TRole>>();
            if (roleManager is not null)
            {
                AddRoles(roleManager, identitySeeds);
            }

            AddUsers(scope.ServiceProvider.GetRequiredService<UserManager<TUser>>(), roleManager, identitySeeds);
        }
        return app;
    }

    private static void AddRoles<TUser, TRole>(RoleManager<TRole> roleManager, IdentitySeeds<TUser, TRole> identitySeeds)
        where TUser : IdentityUser<string>
        where TRole : IdentityRole<string>
    {
        foreach (var role in identitySeeds.Roles)
        {
            if (role.Role.Name is string name && !roleManager.RoleExistsAsync(name).Result)
            {
                IdentityResult result = roleManager.CreateAsync(role.Role).Result;
                if (result.Succeeded)
                {
                    foreach (Claim claim in role.Claims)
                        roleManager.AddClaimAsync(role.Role, claim).Wait();
                }
            }
        }
    }

    private static void AddUsers<TUser, TRole>(UserManager<TUser> userManager, RoleManager<TRole>? roleManager, IdentitySeeds<TUser, TRole> identitySeeds)
        where TUser : IdentityUser<string>
        where TRole : IdentityRole<string>
    {
        foreach (var user in identitySeeds.Users)
        {
            if (user.User.Email is string email && userManager.FindByEmailAsync(email).Result == null)
            {
                IdentityResult result = userManager.CreateAsync(user.User, user.Password).Result;
                if (result.Succeeded)
                {
                    if (user.Claims?.Any() == true)
                    {
                        foreach (Claim claim in user.Claims)
                        {
                            userManager.AddClaimAsync(user.User, claim).Wait();
                        }
                    }
                    if (roleManager is not null && user.Roles?.Any() == true)
                    {
                        foreach (TRole role in user.Roles)
                        {
                            if (role.Name is not null)
                            {
                                if (!identitySeeds.Roles.Any(i => i.Role.Name == role.Name)
                                    && !roleManager.RoleExistsAsync(role.Name).Result)
                                {
                                    roleManager.CreateAsync(role).Wait();
                                }
                                userManager.AddToRoleAsync(user.User, role.Name).Wait();
                            }
                        }
                    }
                }
            }
        }
    }
}

public class IdentitySeeds<TUser, TRole>
    where TUser : IdentityUser<string>
    where TRole : IdentityRole<string>
{
    internal IdentitySeeds()
    {

    }

    internal Collection<UserDescriptor> Users { get; } = [];
    internal Collection<RoleDescriptor> Roles { get; } = [];

    public IdentitySeeds<TUser, TRole> AddUser(TUser user, string password, IEnumerable<Claim>? claims = null, IEnumerable<TRole>? roles = null)
    {
        Users.Add(new(user, password, claims, roles));
        return this;
    }
    public IdentitySeeds<TUser, TRole> AddUser(TUser user, string password, params Claim[] claims) => AddUser(user, password, claims, null);
    public IdentitySeeds<TUser, TRole> AddUser(TUser user, string password, params TRole[] roles) => AddUser(user, password, null, roles);

    public IdentitySeeds<TUser, TRole> AddRole(TRole role, params Claim[] claims)
    {
        Roles.Add(new(role, claims));
        return this;
    }

    internal record UserDescriptor(TUser User, string Password, IEnumerable<Claim>? Claims, IEnumerable<TRole>? Roles);
    internal record RoleDescriptor(TRole Role, IEnumerable<Claim> Claims);
}
