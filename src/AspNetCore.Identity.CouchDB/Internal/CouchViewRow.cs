using Newtonsoft.Json;

namespace CouchDB.Driver.Types
{
    /// <summary>
    /// The object returned from a view execution.
    /// </summary>
    /// <typeparam name="TValue">The type the value will be deserialized to.</typeparam>
    internal class CouchViewRow<TValue>
    {
        /// <summary>
        /// The id of the document.
        /// </summary>
        [JsonProperty("id")]
        internal string Id { get; private set; } = null!;

        /// <summary>
        /// The view key that was emmited.
        /// </summary>
        [JsonProperty("key")]
        internal string Key { get; private set; } = null!;

        /// <summary>
        /// The value that the view emmited.
        /// </summary>
        [JsonProperty("value")]
        internal TValue Value { get; private set; } = default!;
    }

    /// <inheritdoc/>
    /// <typeparam name="TValue">The type the value will be deserialized to.</typeparam>
    /// <typeparam name="TDoc">The type the doc will be deserialized to.</typeparam>
    internal class CouchViewRow<TValue, TDoc> : CouchViewRow<TValue>
        where TDoc : CouchDocument
    {
        /// <summary>
        /// The deserialized json document.
        /// </summary>
        [JsonProperty("doc")]
        internal TDoc Doc { get; private set; } = default!;
    }
}