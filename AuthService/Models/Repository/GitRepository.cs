using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.Models.Repository
{
    public class GitRepository
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string GitRepoId { get; set; }
        public int ProjectId { get; set; }
        public Project.Project Project { get; set; }
    }
}
