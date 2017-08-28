using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AdSale.Models
{
    public class AdTypeModel
    {
        public string Id { get; set; }

        [DisplayName("Nafn")]
        [Required(ErrorMessage = "Vinsamlegast skráðu nafn")]
        public string Name { get; set; }

        [DisplayName("Grunnstafafjöldi")]
        [Required(ErrorMessage = "Vinsamlegast skráðu grunnstafafjölda")]
        public int BaseCharacterCount { get; set; }

        [DisplayName("Grunnverð")]
        [Required(ErrorMessage = "Vinsamlegast skráðu grunnverð")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal BaseCharacterPrice { get; set; }

        [DisplayName("Aukastafafjöldi")]
        [Required(ErrorMessage = "Vinsamlegast skráðu aukastafafjölda")]
        public int AdditionalCharacterCount { get; set; }

        [DisplayName("Aukaverð")]
        [Required(ErrorMessage = "Vinsamlegast skráðu aukaverð")]
        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
        public decimal AdditionalCharacterPrice { get; set; }

        [DisplayName("Verð fyrirsagnar")]
        [Required(ErrorMessage = "Vinsamlegast skráðu verð fyrirsagnar")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal HeaderPrice { get; set; }
        
        public ICollection<MediaModel> Media { get; set; }

        public string MediaName { get; set; }

        [DisplayName("Miðill")]
        [Required(ErrorMessage = "Vinsamlegast veldu miðil")]
        public string MediaId { get; set; }

        /// <summary>
        /// True if the ad type is active, otherwise false
        /// </summary>
        [DisplayName("Virkur")]
        public bool IsActive { get; set; }
    }
}