using Microsoft.AspNetCore.Identity;
using System.Collections.ObjectModel;
using System.Security.Claims;

namespace AspNetCore.Identity.Stores;

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
