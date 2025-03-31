using System.ComponentModel.DataAnnotations;

namespace AuthService.Models.Project
{
    public class ProjectGitRelation
    {
        [Key]
        [Required] public required string GitName { get; set; }
        public int ProjectId { get; set; }
        [Required] public required Project Project { get; set; }
    }
}
