using System.ComponentModel.DataAnnotations;

namespace NetflixStatizier.Models
{
    public class NetflixAccountViewModel
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
