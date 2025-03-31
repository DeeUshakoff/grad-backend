using AuthService.Models.Organization;
using System.ComponentModel.DataAnnotations;

namespace AuthService.DTO.Organization
{
    public class PatchOrganizationGitRelationDTO : IPatchableDTO<OrganizationGitRelation>
    {
        [Required] public required string GitName { get; set; }
        public string? GitToken { get; set; }
        public int? OrganizationId { get; set; }

        public void ApplyEntity(OrganizationGitRelation entity)
        {
            entity.GitName = GitName ?? entity.GitName;
        }
    }
}
