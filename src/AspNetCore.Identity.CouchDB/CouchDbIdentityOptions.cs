﻿using CouchDB.Driver;

namespace AspNetCore.Identity.CouchDB
{
    /// <summary>
    /// Provides various settings needed to configure the AspNetCore Identity CouchDB integration.
    /// </summary>
    public class CouchDbIdentityOptions
    {
        /// <summary>
        /// Gets or sets the name of the applications collection (by default, aspnetcore.user).
        /// </summary>
        public string UserDiscriminator { get; set; } = "aspnetcore.user";

        /// <summary>
        /// Gets or sets the name of the authorizations collection (by default, aspnetcore.role).
        /// </summary>
        public string RoleDiscriminator { get; set; } = "aspnetcore.role";

        /// <summary>
        /// Gets or sets the <see cref="ICouchClient"/> used by the AspNetCore.Identity stores.
        /// If no value is explicitly set, the database is resolved from the DI container.
        /// </summary>
        public ICouchClient? CouchClient { get; set; }

        /// <summary>
        /// The name of the database to use.
        /// </summary>
        public string DatabaseName { get; set; } = "identity";

        /// <summary>
        /// The limit a query can run is at 268_435_456. This is set to 500_000
        /// by default. You can change it however you want.
        /// </summary>
        public int QueryLimit { get; set; } = 500_000;
    }
}