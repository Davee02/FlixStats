using System.ComponentModel.DataAnnotations;

namespace NetflixStatizier.Models
{
    public class NetflixAccountModel
    {
        [Required(AllowEmptyStrings = false)]
        [EmailAddress]
        public string NetflixEmail { get; set; }

        [Required(AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        public string NetflixPassword { get; set; }
    }
}
