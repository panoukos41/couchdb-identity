#pragma warning disable CA1034 // Nested types should not be visible

using AspNetCore.Identity.CouchDB.Models;

namespace AspNetCore.Identity.CouchDB
{
    /// <summary>
    /// Views regarding AspNetCore.Identity.CouchDb integration.
    /// </summary>
    /// <remarks>
    /// These can be overriden but make sure your database has the new views
    /// and that they perform the correct actions.
    /// </remarks>
    public static class CouchDbIdentityViews
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
            public static (string design, string view) NormalizedUserName { get; set; } = ("user", "normalized_username");

            /// <summary>
            /// A view for the 'NormalizedEmail' property.
            /// </summary>
            /// <value>Default is user/normalized_email.</value>
            public static (string design, string view) NormalizedEmail { get; set; } = ("user", "normalized_email");

            /// <summary>
            /// A view for the 'Roles' property.
            /// </summary>
            /// <value>Default is user/roles.</value>
            public static (string design, string view) Roles { get; set; } = ("user", "roles");
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
            public static (string design, string view) NormalizedName { get; set; } = ("role", "normalized_name");
        }
    }
}