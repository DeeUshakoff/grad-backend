using AuthService.Models.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.Models.Organization
{
    public class Organization
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required] public required string Name { get; set; }
        [Required] public required IdentityUser Owner { get; set; }
        [Required] public required ActivityStatus ActivityStatus { get; set; }
        public string? Description { get; set; }
        public ICollection<OrganizationGitRelation> GitRelations { get; set; }
    }
}
