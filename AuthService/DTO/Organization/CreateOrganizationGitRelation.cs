using System.ComponentModel.DataAnnotations;

namespace AuthService.DTO.Organization
{
    public class CreateOrganizationGitRelation
    {
        [Required] public required string GitName { get; set; }
        [Required] public required string GitToken { get; set; }
        public int? OrganizationId { get; set; }
    }
}
