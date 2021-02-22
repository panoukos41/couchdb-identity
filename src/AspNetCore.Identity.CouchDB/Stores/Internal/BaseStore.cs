using AspNetCore.Identity.CouchDB.Models.Internal;
using CouchDB.Driver;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace AspNetCore.Identity.CouchDB.Stores.Internal
{
    /// <summary>
    /// Base store all stores derive from. Provides basic methods and properties.
    /// </summary>
    /// <typeparam name="TStore">The type of the main store.</typeparam>
    /// <remarks>This is meant for internal stores.</remarks>
    public abstract class BaseStore<TStore> where TStore : IdentityCouchDocument
    {
        /// <summary>
        /// The client used to get databases etc.
        /// </summary>
        protected ICouchClient Client { get; }

        /// <summary>
        /// Gets the options associated with the current store.
        /// </summary>
        protected IOptionsMonitor<CouchDbIdentityOptions> Options { get; }

        /// <summary>
        /// Get the discriminator value used create and to query
        /// different documents.
        /// </summary>
        protected abstract string Discriminator { get; }

        protected BaseStore(IServiceProvider provider, IOptionsMonitor<CouchDbIdentityOptions> options)
        {
            Options = options;
            Client = options.CurrentValue.CouchClient
                ?? provider.GetRequiredService<ICouchClient>();
        }

        /// <summary>
        /// Get a 'database' class that will serialize/deserialize to <typeparamref name="TStore"/>
        /// </summary>
        /// <returns>A 'database' class that serializes/deserializes classes to <typeparamref name="TStore"/>.</returns>
        protected ICouchDatabase<TStore> GetDatabase() =>
            GetDatabase<TStore>();

        /// <summary>
        /// Get a 'database' class that will serialize/deserialize
        /// executed classes to <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to serialize/deserialize to/from.</typeparam>
        /// <returns>A 'database' class that serializes/deserializes classes to <typeparamref name="T"/>.</returns>
        protected ICouchDatabase<T> GetDatabase<T>()
            where T : IdentityCouchDocument
        {
            return Client.GetDatabase<T>(Options.CurrentValue.DatabaseName);
        }

        /// <summary>
        /// Get an IQueryable that is configured to search only for
        /// the supplied <see cref="Discriminator"/> and limits results
        /// to <see cref="CouchDbIdentityOptions.QueryLimit"/>.
        /// </summary>
        protected IQueryable<TStore> QueryDb() =>
            QueryDb(GetDatabase());

        /// <summary>
        /// Get an IQueryable that is configured to search only for
        /// the supplied <see cref="Discriminator"/> will deserialize
        /// to <typeparamref name="T"/> and limits results
        /// to <see cref="CouchDbIdentityOptions.QueryLimit"/>.
        /// </summary>
        /// <typeparam name="T">The type to deserialize results to.</typeparam>
        protected IQueryable<T> QueryDb<T>(ICouchDatabase<T> database)
            where T : IdentityCouchDocument
        {
            return database
                .Take(Options.CurrentValue.QueryLimit)
                .Where(x => x.Discriminator == Discriminator);
        }
    }
}