﻿using AspNetCore.Identity.Stores.AzureCosmosDB.Repositories;
using AspNetCore.Identity.Stores.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AspNetCore.Identity.Stores.AzureCosmosDB.Extensions;

/// <summary>
/// Contains extension methods to <see cref="IdentityBuilder"/> for adding Azure CosmosDB stores.
/// </summary>
public static class IdentityAzureCosmosDBBuilderExtensions
{
    /// <summary>
    /// Adds an Azure CosmosDB implementation of identity information stores.
    /// </summary>
    /// <param name="builder">The <see cref="IdentityBuilder"/> instance this method extends.</param>
    /// <returns>The <see cref="IdentityBuilder"/> instance this method extends.</returns>
    public static IdentityBuilder AddAzureCosmosDbStores(this IdentityBuilder builder)
    {
        return AddAzureCosmosDbStores<IdentityUserClaim<string>, IdentityUserRole<string>, IdentityUserLogin<string>, IdentityUserToken<string>,IdentityRoleClaim<string>>(builder);
    }
    public static IdentityBuilder AddAzureCosmosDbStores<TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim>(this IdentityBuilder builder)
        where TUserClaim : IdentityUserClaim<string>, new()
        where TUserRole: IdentityUserRole<string>, new()
        where TUserLogin : IdentityUserLogin<string>, new()
        where TUserToken : IdentityUserToken<string>, new()
        where TRoleClaim : IdentityRoleClaim<string>, new()
    {
        builder.Services.AddTransient<IStoreInitializer, CosmosDbStoreInitializer>();
        builder.AddStores<TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim>();
        AddStorageTables<string, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim>(builder);

        return builder;
    }

    private static void AddStorageTables<TKey, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim>(IdentityBuilder builder)
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>, new()
        where TUserRole : IdentityUserRole<TKey>, new()
        where TUserLogin : IdentityUserLogin<TKey>, new()
        where TUserToken : IdentityUserToken<TKey>, new()
        where TRoleClaim : IdentityRoleClaim<TKey>, new()
    {
        builder.Services.TryAddScoped(typeof(IUsersTable<,>).MakeGenericType(builder.UserType, typeof(TKey)), typeof(UsersTable<,>).MakeGenericType(builder.UserType, typeof(TKey)));

        Type userClaimsTableType = typeof(UserClaimsTable<,,>).MakeGenericType(builder.UserType, typeof(TUserClaim), typeof(TKey));
        builder.Services.TryAddScoped(userClaimsTableType);
        builder.Services.TryAddScoped(typeof(IUserClaimsTable<,>).MakeGenericType(builder.UserType, typeof(TKey)), i => i.GetRequiredService(userClaimsTableType));
        builder.Services.TryAddScoped(typeof(IUserClaimsTable<,,>).MakeGenericType(builder.UserType, typeof(TUserClaim), typeof(TKey)), i => i.GetRequiredService(userClaimsTableType));

        Type userRolesTableType = typeof(UserRolesTable<,>).MakeGenericType(typeof(TUserRole), typeof(TKey));
        builder.Services.TryAddScoped(userRolesTableType);
        builder.Services.TryAddScoped(typeof(IUserRolesTable<>).MakeGenericType(typeof(TKey)), i => i.GetRequiredService(userRolesTableType));
        builder.Services.TryAddScoped(typeof(IUserRolesTable<,>).MakeGenericType(typeof(TUserRole), typeof(TKey)), i => i.GetRequiredService(userRolesTableType));

        Type userLoginsTableType = typeof(UserLoginsTable<,,>).MakeGenericType(builder.UserType, typeof(TUserLogin), typeof(TKey));
        builder.Services.TryAddScoped(userLoginsTableType);
        builder.Services.TryAddScoped(typeof(IUserLoginsTable<,>).MakeGenericType(builder.UserType, typeof(TKey)), i => i.GetRequiredService(userLoginsTableType));
        builder.Services.TryAddScoped(typeof(IUserLoginsTable<,,>).MakeGenericType(builder.UserType, typeof(TUserLogin), typeof(TKey)), i => i.GetRequiredService(userLoginsTableType));

        Type userTokensTableType = typeof(UserTokensTable<,>).MakeGenericType(typeof(TUserToken), typeof(TKey));
        builder.Services.TryAddScoped(userTokensTableType);
        builder.Services.TryAddScoped(typeof(IUserTokensTable<>).MakeGenericType(typeof(TKey)), i => i.GetRequiredService(userTokensTableType));
        builder.Services.TryAddScoped(typeof(IUserTokensTable<,>).MakeGenericType(typeof(TUserToken), typeof(TKey)), i => i.GetRequiredService(userTokensTableType));

        if (builder.RoleType is not null)
        {
            builder.Services.TryAddScoped(typeof(IRolesTable<,>).MakeGenericType(builder.RoleType, typeof(TKey)), typeof(RolesTable<,>).MakeGenericType(builder.RoleType, typeof(TKey)));
            Type roleClaimsTableType = typeof(RoleClaimsTable<,,>).MakeGenericType(builder.RoleType, typeof(TRoleClaim), typeof(TKey));
            builder.Services.TryAddScoped(roleClaimsTableType);
            builder.Services.TryAddScoped(typeof(IRoleClaimsTable<,,>).MakeGenericType(builder.RoleType, typeof(TRoleClaim), typeof(TKey)), i => i.GetRequiredService(roleClaimsTableType));
            builder.Services.TryAddScoped(typeof(IRoleClaimsTable<,>).MakeGenericType(builder.RoleType, typeof(TKey)), i => i.GetRequiredService(roleClaimsTableType));
        }
    }
}
