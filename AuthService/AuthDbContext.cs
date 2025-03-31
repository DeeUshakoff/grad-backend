using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using AuthService.Models.Task;
using AuthService.Models.Organization;
using AuthService.Models.Project;
using AuthService.Models.Users;
using AuthService.Models.Repository;

namespace AuthService
{
    public class AuthDbContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<Models.Task.Task> Tasks { get; set; }
        public DbSet<TaskGitRelation> TaskGitRelations { get; set; }

        public DbSet<Organization> Organizations { get; set; }
        public DbSet<OrganizationGitRelation> OrganizationGitRelations { get; set; }
        public DbSet<UserOrganizationRelation> UserOrganizationRelations { get; set; }

        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectGitRelation> ProjectGitRelations { get; set; }
        public DbSet<GitRepository> GitRepositories{ get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrganizationGitRelation>()
                .HasOne(ogr => ogr.Organization)
                .WithMany(o => o.GitRelations)
                .HasForeignKey(ogr => ogr.OrganizationId);

            //modelBuilder.Entity<TaskGitRelation>()
            //    .HasOne(tgr => tgr.GitRepository)
                

            base.OnModelCreating(modelBuilder);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(
                @"");
        }
    }
}
