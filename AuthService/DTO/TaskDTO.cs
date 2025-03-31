using System.ComponentModel.DataAnnotations;

namespace AuthService.DTO
{
    public class TaskDTO
    {
        [StringLength(256, MinimumLength = 1)]
        [Required] public required string Title { get; set; }
        [Required] public required string AssignedToId { get; set; }
        [Required] public required string AssignedFromId { get; set; }
        [Required] public int ProjectId { get; set; }
        [Required] public int TaskStatus { get; set; }
        public string? Description { get; set; }
        public int? ProjectRepoId { get; set; }
        [Required] public bool NeedGit { get; set; }
    }

    public class TaskPatchDTO : IPatchableDTO<Models.Task.Task>
    {
        public string? Title { get; set; }

        public int? TaskStatus { get; set; }
        public string? Description { get; set; }

        public void ApplyEntity(Models.Task.Task task)
        {
            Title = task.Title;
            TaskStatus = (int)task.TaskStatus;
            Description = task.Description;
        }
    }
}
