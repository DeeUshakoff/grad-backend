using System.ComponentModel.DataAnnotations;

namespace AuthService.DTO.Organization
{
    public class CreateOrganizationDTO
    {
        [Required] public required string Name { get; set; }
        public string? Description { get; set; }
    }
}
