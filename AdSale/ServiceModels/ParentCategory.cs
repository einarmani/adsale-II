using System.Collections.Generic;

namespace AdSale.ServiceModels
{
    /// <summary>
    /// A model for an advertisement sale parent category
    /// </summary>
    public class ParentCategory : Category
    {
        /// <summary>
        /// A collection of subcategories
        /// </summary>
        public ICollection<Category> Subcategories { get; set; }
    }
}