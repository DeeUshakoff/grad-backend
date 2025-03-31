using AuthService.DTO.Task;
using AuthService.Models.Repository;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.Models.Task
{
    public class TaskGitRelation : IPatchableModel<PatchTaskGitRelationDTO>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required] public int Id { get; set; }
        [Required] public required Task Task { get; set; }
        public int? GitRepositoryId { get; set; }
        public GitRepository? GitRepository { get; set; }
        public string? GitBranchName { get; set; }

        public void Patch(PatchTaskGitRelationDTO patchDTO)
        {
            // todo: add GitRepository switch
            GitBranchName = patchDTO.GitBranchName ?? GitBranchName;
        }
    }
}
