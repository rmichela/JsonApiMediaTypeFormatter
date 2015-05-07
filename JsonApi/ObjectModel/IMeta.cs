namespace JsonApi.ObjectModel
{
    /// <summary>
    /// Provides access to JsonApi objects that support Metadata.
    /// </summary>
    public interface IMeta
    {
        /// <summary>
        /// Gets the Metadata for a JsonApi object
        /// </summary>
        dynamic Meta { get; }
    }
}
