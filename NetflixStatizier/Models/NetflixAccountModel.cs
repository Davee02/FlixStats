using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NetflixStatizier.Models
{
    public class NetflixAccountModel
    {
        [Required(AllowEmptyStrings = false)]
        [BindRequired]
        [EmailAddress]
        public string NetflixEmail { get; set; }

        [Required(AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        public string NetflixPassword { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string NetflixProfileName { get; set; }
    }
}
