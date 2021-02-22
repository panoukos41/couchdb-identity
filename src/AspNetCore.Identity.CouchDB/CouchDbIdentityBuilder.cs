using AspNetCore.Identity.CouchDB;
using CouchDB.Driver;
using System;
using System.ComponentModel;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Exposes the necessary methods required to configure the AspNetCore.Identity.CouchDB services.
    /// </summary>
    public class CouchDbIdentityBuilder
    {
        /// <summary>
        /// Initializes a new instance of <see cref="CouchDbIdentityBuilder"/>.
        /// </summary>
        /// <param name="services">The services collection.</param>
        public CouchDbIdentityBuilder(IServiceCollection services)
            => Services = services ?? throw new ArgumentNullException(nameof(services));

        /// <summary>
        /// Gets the services collection.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IServiceCollection Services { get; }

        /// <summary>
        /// Amends the default AspNetCore.Identity.CouchDB configuration.
        /// </summary>
        /// <param name="configuration">The delegate used to configure the AspNetCore.Identity.CouchDB options.</param>
        /// <remarks>This extension can be safely called multiple times.</remarks>
        /// <returns>The <see cref="CouchDbIdentityBuilder"/>.</returns>
        public CouchDbIdentityBuilder Configure(Action<CouchDbIdentityOptions> configuration)
        {
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            Services.Configure(configuration);

            return this;
        }

        /// <summary>
        /// Replaces the default user discriminator (by default, aspnetcore.user).
        /// </summary>
        /// <param name="discriminator">The discriminator name.</param>
        /// <returns>The <see cref="CouchDbIdentityBuilder"/>.</returns>
        public CouchDbIdentityBuilder SetUserDiscriminator(string discriminator)
        {
            Check.NotNull(discriminator, nameof(discriminator));

            return Configure(options => options.UserDiscriminator = discriminator);
        }

        /// <summary>
        /// Replaces the default role discriminator (by default, aspnetcore.role).
        /// </summary>
        /// <param name="discriminator">The discriminator name</param>
        /// <returns>The <see cref="CouchDbIdentityBuilder"/>.</returns>
        public CouchDbIdentityBuilder SetRoleDiscriminator(string discriminator)
        {
            Check.NotNull(discriminator, nameof(discriminator));

            return Configure(options => options.RoleDiscriminator = discriminator);
        }

        /// <summary>
        /// Change the database name that will be used on the provided couch client
        /// to retrieve a database.
        /// </summary>
        /// <param name="name">The name of the database.</param>
        /// <returns>The <see cref="CouchDbIdentityBuilder"/>.</returns>
        public CouchDbIdentityBuilder SetDatabaseName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Database name must not be null or whitespace.", nameof(name));
            }

            return Configure(options => options.DatabaseName = name);
        }

        /// <summary>
        /// Configures the CouchDB stores to use the specified client
        /// instead of retrieving it from the dependency injection container.
        /// </summary>
        /// <param name="client">The <see cref="ICouchClient"/>.</param>
        /// <returns>The <see cref="CouchDbIdentityBuilder"/>.</returns>
        public CouchDbIdentityBuilder UseClient(ICouchClient client)
        {
            Check.NotNull(client, nameof(client));

            return Configure(options => options.CouchClient = client);
        }

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object? obj) => base.Equals(obj);

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => base.GetHashCode();

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string? ToString() => base.ToString();
    }
}