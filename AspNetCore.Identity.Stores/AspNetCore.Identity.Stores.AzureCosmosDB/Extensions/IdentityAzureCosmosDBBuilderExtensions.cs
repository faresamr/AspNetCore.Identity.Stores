using AspNetCore.Identity.Stores.AzureCosmosDB.Repositories;
using AspNetCore.Identity.Stores.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Identity.Stores.AzureCosmosDB.Extensions
{
    /// <summary>
    /// Contains extension methods to <see cref="IdentityBuilder"/> for adding Azure Table Storage stores.
    /// </summary>
    public static class IdentityAzureCosmosDBBuilderExtensions
    {
        /// <summary>
        /// Adds an Azure Table Storage implementation of identity information stores.
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
            builder.Services.TryAddScoped(typeof(IUserClaimsTable<,,>).MakeGenericType(builder.UserType, typeof(TUserClaim), typeof(TKey)), typeof(UserClaimsTable<,,>).MakeGenericType(builder.UserType, typeof(TUserClaim), typeof(TKey)));
            builder.Services.TryAddScoped(typeof(IUserRolesTable<,>).MakeGenericType(typeof(TUserRole), typeof(TKey)), typeof(UserRolesTable<,>).MakeGenericType(typeof(TUserRole), typeof(TKey)));
            builder.Services.TryAddScoped(typeof(IUserLoginsTable<,,>).MakeGenericType(builder.UserType, typeof(TUserLogin), typeof(TKey)), typeof(UserLoginsTable<,,>).MakeGenericType(builder.UserType, typeof(TUserLogin), typeof(TKey)));
            builder.Services.TryAddScoped(typeof(IUserTokensTable<,>).MakeGenericType(typeof(TUserToken), typeof(TKey)), typeof(UserTokensTable<,>).MakeGenericType(typeof(TUserToken), typeof(TKey)));

            if (builder.RoleType is not null)
            {
                builder.Services.TryAddScoped(typeof(IRolesTable<,>).MakeGenericType(builder.RoleType, typeof(TKey)), typeof(RolesTable<,>).MakeGenericType(builder.RoleType, typeof(TKey)));
                builder.Services.TryAddScoped(typeof(IRoleClaimsTable<,,>).MakeGenericType(builder.RoleType, typeof(TRoleClaim), typeof(TKey)), typeof(RoleClaimsTable<,,>).MakeGenericType(builder.RoleType, typeof(TRoleClaim), typeof(TKey)));
            }
        }
    }
}
