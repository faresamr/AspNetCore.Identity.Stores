using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Claims;

namespace AspNetCore.Identity.Stores
{
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
                AddRoles(scope.ServiceProvider, identitySeeds);
                AddUsers(scope.ServiceProvider, identitySeeds);
            }
            return app;
        }

        private static void AddRoles<TUser, TRole>(IServiceProvider serviceProvider, IdentitySeeds<TUser, TRole> identitySeeds)
            where TUser : IdentityUser<string>
            where TRole : IdentityRole<string>
        {
            RoleManager<TRole> roleManager = serviceProvider.GetService<RoleManager<TRole>>();
            if (roleManager is not null)
            {
                foreach (var role in identitySeeds.Roles)
                {
                    if (!roleManager.RoleExistsAsync(role.Role.Name).Result)
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
        }

        private static void AddUsers<TUser, TRole>(IServiceProvider serviceProvider, IdentitySeeds<TUser, TRole> identitySeeds)
            where TUser : IdentityUser<string>
            where TRole : IdentityRole<string>
        {
            UserManager<TUser> userManager = serviceProvider.GetService<UserManager<TUser>>();
            RoleManager<TRole> roleManager = serviceProvider.GetService<RoleManager<TRole>>();
            
            if (userManager is not null)
            {
                foreach (var user in identitySeeds.Users)
                {
                    if (userManager.FindByEmailAsync(user.User.Email).Result == null)
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
                            if (user.Roles?.Any() == true)
                            {
                                foreach (TRole role in user.Roles)
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

        internal Collection<UserDescriptor> Users { get; } = new();
        internal Collection<RoleDescriptor> Roles { get; } = new();

        public IdentitySeeds<TUser, TRole> AddUser(TUser user, string password, IEnumerable<Claim> claims = null, IEnumerable<TRole> roles = null)
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

        internal record UserDescriptor(TUser User, string Password, IEnumerable<Claim> Claims, IEnumerable<TRole> Roles);
        internal record RoleDescriptor(TRole Role, IEnumerable<Claim> Claims);
    }
}
