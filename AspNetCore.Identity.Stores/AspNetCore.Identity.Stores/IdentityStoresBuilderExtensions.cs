using AspNetCore.Identity.Stores.Properties;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Identity.Stores
{
    public static class IdentityStoresBuilderExtensions
    {
        public static void AddStores<TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim>(this IdentityBuilder builder)
            where TUserClaim : IdentityUserClaim<string>, new()
            where TUserRole : IdentityUserRole<string>, new()
            where TUserLogin : IdentityUserLogin<string>, new()
            where TUserToken : IdentityUserToken<string>, new()
            where TRoleClaim : IdentityRoleClaim<string>, new()
        {
            var identityUserType = FindGenericBaseType(builder.UserType, typeof(IdentityUser<>));
            if (identityUserType == null)
            {
                throw new InvalidOperationException(Resources.NotIdentityUser);
            }

            var keyType = identityUserType.GenericTypeArguments[0];

            if (builder.RoleType != null)
            {
                var identityRoleType = FindGenericBaseType(builder.RoleType, typeof(IdentityRole<>));
                if (identityRoleType == null)
                {
                    throw new InvalidOperationException(Resources.NotIdentityRole);
                }

                Type userStoreType = typeof(UserStore<,,,,,>).MakeGenericType(builder.UserType, builder.RoleType, typeof(TUserClaim), typeof(TUserRole), typeof(TUserLogin), typeof(TUserToken));
                
                Type roleStoreType = typeof(RoleStore<,>).MakeGenericType(builder.RoleType, typeof(TRoleClaim));

                builder.Services.TryAddScoped(typeof(IUserStore<>).MakeGenericType(builder.UserType), userStoreType);
                builder.Services.TryAddScoped(typeof(IRoleStore<>).MakeGenericType(builder.RoleType), roleStoreType);
            }
            else
            {   // No Roles
                Type userStoreType = typeof(UserOnlyStore<,,,>).MakeGenericType(builder.UserType, typeof(TUserClaim), typeof(TUserLogin), typeof(TUserToken));
                builder.Services.TryAddScoped(typeof(IUserStore<>).MakeGenericType(builder.UserType), userStoreType);
            }

        }

        private static Type FindGenericBaseType(Type currentType, Type genericBaseType)
        {
            var type = currentType;
            while (type != null)
            {
                var genericType = type.IsGenericType ? type.GetGenericTypeDefinition() : null;
                if (genericType != null && genericType == genericBaseType)
                {
                    return type;
                }
                type = type.BaseType;
            }
            return null;
        }
    }
}
