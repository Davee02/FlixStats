using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NetflixStatizier.Models
{
    public class NetflixAccountModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

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
