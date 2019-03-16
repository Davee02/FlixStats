using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NetflixStatizier.Models
{
    public class NetflixAccountModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [BindRequired]
        [EmailAddress]
        public string NetflixEmail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string NetflixPassword { get; set; }

        [Required]
        public string NetflixProfileName { get; set; }


        public IdentityUser IdentityUser { get; set; }

        public string IdentityUserId { get; set; }
    }
}
