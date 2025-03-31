using AuthService.Models.Organization;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.Models.Users
{
    public class UserOrganizationRelation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required] public required IdentityUser User { get; set; }
        [Required] public required Organization.Organization Organization { get; set; }
    }
}
