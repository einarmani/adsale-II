using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AdSale.Models
{
    public class AdTypeModel
    {
        public string Id { get; set; }

        [DisplayName("Nafn")]
        [Required(ErrorMessage = "Vinsamlegast skr��u nafn")]
        public string Name { get; set; }

        [DisplayName("Grunnstafafj�ldi")]
        [Required(ErrorMessage = "Vinsamlegast skr��u grunnstafafj�lda")]
        public int BaseCharacterCount { get; set; }

        [DisplayName("Grunnver�")]
        [Required(ErrorMessage = "Vinsamlegast skr��u grunnver�")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal BaseCharacterPrice { get; set; }

        [DisplayName("Aukastafafj�ldi")]
        [Required(ErrorMessage = "Vinsamlegast skr��u aukastafafj�lda")]
        public int AdditionalCharacterCount { get; set; }

        [DisplayName("Aukaver�")]
        [Required(ErrorMessage = "Vinsamlegast skr��u aukaver�")]
        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
        public decimal AdditionalCharacterPrice { get; set; }

        [DisplayName("Ver� fyrirsagnar")]
        [Required(ErrorMessage = "Vinsamlegast skr��u ver� fyrirsagnar")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal HeaderPrice { get; set; }
        
        public ICollection<MediaModel> Media { get; set; }

        public string MediaName { get; set; }

        [DisplayName("Mi�ill")]
        [Required(ErrorMessage = "Vinsamlegast veldu mi�il")]
        public string MediaId { get; set; }

        /// <summary>
        /// True if the ad type is active, otherwise false
        /// </summary>
        [DisplayName("Virkur")]
        public bool IsActive { get; set; }
    }
}