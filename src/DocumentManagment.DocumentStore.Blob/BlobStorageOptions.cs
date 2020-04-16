namespace DocumentManagement.DocumentStore.Blob
{
    /// <summary>
    /// Blob storage options.
    /// </summary>
    public class BlobStorageOptions
    {
        /// <summary>
        /// Connection string.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Container name.
        /// </summary>
        public string ContainerName { get; set; } = "pdfstore";

        /// <summary>
        /// Container prefix.
        /// </summary>
        public string ContainerPrefix { get; set; } = string.Empty;
    }
}
