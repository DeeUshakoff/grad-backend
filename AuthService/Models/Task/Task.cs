using AuthService.DTO;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.Models.Task
{
    public class Task
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required] public required string Title { get; set; }
        [Required] public required virtual IdentityUser AssignedTo { get; set; }
        [Required] public required virtual Project.Project Project { get; set; }
        [Required] public required virtual IdentityUser? AssignedFrom { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public TaskStatus TaskStatus { get; set; }

        public Task Patch(TaskPatchDTO patchDTO)
        {
            Title = patchDTO.Title ?? Title;
            //AssignedTo = patchDTO.AssignedTo ?? AssignedTo;
            //AssignedFrom = patchDTO.AssignedFrom ?? AssignedFrom;
            //Project = patchDTO.Project ?? Project;
            Description = patchDTO.Description ?? Description;
            TaskStatus = patchDTO.TaskStatus != null ? (TaskStatus) patchDTO.TaskStatus: TaskStatus;

            return this;
        }
    }

    public enum TaskStatus
    {
        Canceled,
        Created,
        Done,
        Assigned,
        InProgress,
        OnReview
    }
}
