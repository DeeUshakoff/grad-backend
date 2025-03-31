using AuthService.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace AuthService.DTO.Project
{
    public class CreateProjectDTO
    {
        [Required] public required string Name { get; set; }
        [Required] public required int OrganizationID { get; set; }
        public string? Description { get; set; }
    }
}
