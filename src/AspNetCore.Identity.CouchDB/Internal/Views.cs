using AspNetCore.Identity.CouchDB.Models;

#pragma warning disable CA1034 // Nested types should not be visible

namespace AspNetCore.Identity.CouchDB.Internal
{
    /// <summary>
    /// Views regarding AspNetCore.Identity.CouchDb integration.
    /// </summary>
    /// <remarks>
    /// These can be overriden but make sure your database has the new views
    /// and that they perform the correct actions.
    /// </remarks>
    public static class Views
    {
        /// <summary>
        /// Views regarding a <see cref="CouchDbUser"/> document.
        /// </summary>
        public static class User
        {
            /// <summary>
            /// A view for the 'NormalizedUsername' property.
            /// </summary>
            /// <value>Default is user/normalized_username.</value>
            public static View<string, string, CouchDbUser> NormalizedUserName { get; set; } =
                View<string, string, CouchDbUser>.Create("user", "normalized_username");

            /// <summary>
            /// A view for the 'NormalizedEmail' property.
            /// </summary>
            /// <value>Default is user/normalized_email.</value>
            public static View<string, string, CouchDbUser> NormalizedEmail { get; set; } =
                View<string, string, CouchDbUser>.Create("user", "normalized_email");

            /// <summary>
            /// A view for the 'Roles' property.
            /// </summary>
            /// <value>Default is user/roles.</value>
            public static View<string, string, CouchDbUser> Roles { get; set; } =
                View<string, string, CouchDbUser>.Create("user", "roles");
        }

        /// <summary>
        /// Views regarding a <see cref="CouchDbRole"/> document.
        /// </summary>
        public static class Role
        {
            /// <summary>
            /// A view for the 'NormalizedName' property.
            /// </summary>
            /// <value>Default is user/normalized_name.</value>
            public static View<string, int, CouchDbRole> NormalizedName { get; set; } =
                View<string, int, CouchDbRole>.Create("role", "normalized_name");
        }
    }
}