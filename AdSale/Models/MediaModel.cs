using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AdSale.Models
{
    /// <summary>
    /// A model for advertisement sale media
    /// </summary>
    public class MediaModel
    {
        /// <summary>
        /// The id of the media
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The name of the media
        /// </summary>
        [DisplayName("Nafn")]
        [Required(ErrorMessage = "Vinsamlegast skráðu nafn")]
        public string Name { get; set; }

        /// <summary>
        /// True if the media is active, otherwise false
        /// </summary>
        [DisplayName("Virkur")]
        public bool IsActive { get; set; }
    }
}