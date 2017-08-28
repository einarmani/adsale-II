using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AdSale.Models
{
    /// <summary>
    /// A model for advertisement sale category
    /// </summary>
    public class CategoryModel
    {
        /// <summary>
        /// The id of the category. If it is a parent the id doesn't contain a dash. If it's a child the id contains a dash where the string before the dash is the id of the parent
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The id of the parent category
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// The name of the category
        /// </summary>
        [DisplayName("Nafn")]
        [Required(ErrorMessage = "Vinsamlegast skráðu nafn")]
        public string Name { get; set; }

        /// <summary>
        /// True if the category is active, otherwise false
        /// </summary>
        [DisplayName("Virkur")]
        public bool IsActive { get; set; }
    }
}
