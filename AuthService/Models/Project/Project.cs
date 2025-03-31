using AuthService.DTO.Project;
using AuthService.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.Models.Project
{
    public class Project : IPatchableModel<PatchProjectDTO>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required] public required string Name { get; set; }
        [Required] public required Organization.Organization Organization { get; set; }
        [Required] public ActivityStatus ActivityStatus { get; set; }
        public string? Description { get; set; }
        public ICollection<ProjectGitRelation> GitRelations { get; set; }
        public int OrganizationId { get; set; }
        public void Patch(PatchProjectDTO patchDTO)
        {
            Name = patchDTO.Name ?? Name;
            ActivityStatus = patchDTO.ActivityStatus != null ? (ActivityStatus)patchDTO.ActivityStatus : ActivityStatus;
            Description = patchDTO.Description ?? Description;
        }
    }
}
