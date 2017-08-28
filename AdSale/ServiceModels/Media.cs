namespace AdSale.ServiceModels
{
    /// <summary>
    /// The media for advertisement sale
    /// </summary>
    public class Media
    {
        /// <summary>
        /// The id of the media
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The name of the media
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// True if the media is active, otherwise false
        /// </summary>
        public bool IsActive { get; set; }
    }
}