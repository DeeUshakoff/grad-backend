using AuthService.Models.Repository;
using System.ComponentModel.DataAnnotations;

namespace AuthService.DTO.Task
{
    public class CreateTaskGitRelationDTO
    {
        [Required] public required int TaskId { get; set; }
        public int GitRepositoryId { get; set; }
        public string? GitBranchName { get; set; }
    }
}
