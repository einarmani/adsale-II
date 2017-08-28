namespace AdSale.ServiceModels
{
    /// <summary>
    /// A model for an advertisement sale category
    /// </summary>
    public class Category
    {
        /// <summary>
        /// Unique identifier for the category. If it is a parent the id doesn't contain a dash. If it's a child the id contains a dash where the string before the dash is the id of the parent
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The name of the category
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// True if the category is visible, otherwise false
        /// </summary>
        public bool IsActive { get; set; }
    }
}
