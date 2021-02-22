using AspNetCore.Identity.CouchDB;
using AspNetCore.Identity.CouchDB.Stores;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Microsoft.AspNetCore.Identity
{
    /// <summary>
    /// Contains extension methods to <see cref="IdentityBuilder"/> for adding entity framework stores.
    /// </summary>
    public static class CouchDbIdentityBuilderExtensions
    {
        /// <summary>
        /// Registers the CouchDB stores services in the DI container and
        /// configures AspNetCore Identity to use the CouchDB entities by default.
        /// </summary>
        /// <param name="builder">The services builder used to register new services.</param>
        /// <remarks>This extension can be safely called multiple times.</remarks>
        /// <returns>The <see cref="CouchDbIdentityBuilder"/>.</returns>
        public static CouchDbIdentityBuilder AddCouchDbStores(this IdentityBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            builder.Services.AddSingleton(
                typeof(IRoleStore<>).MakeGenericType(builder.RoleType),
                typeof(RoleStore<>).MakeGenericType(builder.RoleType));

            builder.Services.AddSingleton(
                typeof(IUserStore<>).MakeGenericType(builder.UserType),
                typeof(UserStore<,>).MakeGenericType(builder.UserType, builder.RoleType));

            return new CouchDbIdentityBuilder(builder.Services);
        }

        /// <summary>
        /// Registers the CouchDB stores services in the DI container and
        /// configures AspNetCore Identity to use the CouchDB entities by default.
        /// </summary>
        /// <param name="builder">The services builder used to register new services.</param>
        /// <param name="configuration">The configuration delegate used to configure the CouchDB services.</param>
        /// <remarks>This extension can be safely called multiple times.</remarks>
        /// <returns>The <see cref="IdentityBuilder"/>.</returns>
        public static IdentityBuilder AddCouchDbStores(
            this IdentityBuilder builder, Action<CouchDbIdentityBuilder> configuration)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(configuration, nameof(configuration));

            configuration(builder.AddCouchDbStores());

            return builder;
        }
    }
}