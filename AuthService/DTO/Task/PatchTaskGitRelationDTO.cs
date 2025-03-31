using AuthService.Models.Task;

namespace AuthService.DTO.Task
{
    public class PatchTaskGitRelationDTO : IPatchableDTO<TaskGitRelation>
    {
        public int? GitRepositoryId { get; set; }
        public string? GitBranchName { get; set; }
        public void ApplyEntity(TaskGitRelation entity)
        {
            GitRepositoryId = entity.GitRepositoryId;
            GitBranchName = entity.GitBranchName;
        }
    }
}
