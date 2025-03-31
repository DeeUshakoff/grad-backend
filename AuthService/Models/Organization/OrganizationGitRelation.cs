using AuthService.Helpers;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace AuthService.Models.Organization
{
    public class OrganizationGitRelation : IEncryptableEntity
    {
        [Key]
        [Required] public required string GitName { get; set; }
        [JsonIgnore]
        [Required] public required string GitToken { get; set; }
        public int OrganizationId { get; set; }
        [JsonIgnore]
        [Required] public required Organization Organization { get; set; }
        public void EncryptSelf(IEncryptionService encryptionService)
        {
            GitToken = encryptionService.Encrypt(GitToken);
        }
    }
}
