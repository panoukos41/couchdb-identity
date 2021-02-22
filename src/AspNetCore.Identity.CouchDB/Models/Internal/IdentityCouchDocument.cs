using CouchDB.Driver.Types;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace AspNetCore.Identity.CouchDB.Models.Internal
{
    /// <summary>
    /// Base model from which all couch models derive from.
    /// Models should also derive from <see cref="CouchDocument"/>
    /// </summary>
    /// <remarks>This is meant for internal documents.</remarks>
    [JsonObject("identity")]
    public abstract class IdentityCouchDocument : CouchDocument
    {
        /// <summary>
        /// Gets the unique value that seperates this document from others.
        /// This value should be unique for one class to the whole application/applications.
        /// </summary>
        [DataMember]
        [JsonProperty("discriminator")]
        public abstract string Discriminator { get; set; }
    }
}