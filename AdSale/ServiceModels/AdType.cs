namespace AdSale.ServiceModels
{
    /// <summary>
    /// The type of advertisement sale
    /// </summary>
    public class AdType
    {
        /// <summary>
        /// The id of the type
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The name of the type
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The number of base character that can be used for the base character price
        /// </summary>
        public int BaseCharacterCount { get; set; }

        /// <summary>
        /// The price for the base characters
        /// </summary>
        public decimal BaseCharacterPrice { get; set; }

        /// <summary>
        /// The number of additional characters for additional price to be added
        /// </summary>
        public int AdditionalCharacterCount { get; set; }

        /// <summary>
        /// The price of additional characters
        /// </summary>
        public decimal AdditionalCharacterPrice { get; set; }

        /// <summary>
        /// The price of the header
        /// </summary>
        public decimal HeaderPrice { get; set; }

        /// <summary>
        /// The id of the media
        /// </summary>
        public string MediaId { get; set; }

        /// <summary>
        /// True if the ad type is active, otherwise false
        /// </summary>
        public bool IsActive { get; set; }
    }
}