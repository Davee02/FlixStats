using System.ComponentModel.DataAnnotations;

namespace FlixStats.Models.InputModels
{
    public class NetflixAccountInputModel
    {
        [Required]
        [EmailAddress]
        public string NetflixEmail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string NetflixPassword { get; set; }

        [Required]
        public string NetflixProfileName { get; set; }
    }
}
