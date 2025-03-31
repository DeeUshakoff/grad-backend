using AuthService.DTO.Project;
using AuthService.Models.Project;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectsController(AuthDbContext dbContext) : ControllerBase
    {
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var project = await dbContext.Projects.FindAsync(id);
            if(project == null)
            {
                return NotFound();
            }
            var git = await dbContext.ProjectGitRelations
                .Where(x => x.Project.Id == project.Id)
                .FirstOrDefaultAsync();

            return Ok(new { project, git } );
        }

        [HttpGet("{id}/tasks")]
        public async Task<IActionResult> GetProjectTasks(int id)
        {
            var project = await dbContext.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            var tasks = dbContext.Tasks.Include(t => t.AssignedTo)
                .Include(t => t.AssignedFrom)
                .Include(t => t.Project)
                .ThenInclude(p => p.Organization).Where(x => x.Project == project);
            return Ok(tasks);
        }

        [HttpGet("{id}/repos")]
        public async Task<IActionResult> GetProjectRepos(int id)
        {
            var project = await dbContext.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            var repos = dbContext.GitRepositories.Where(x => x.Project == project);
            return Ok(repos);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateProjectDTO model)
        {
            var org = await dbContext.Organizations.FindAsync(model.OrganizationID);
            if(org == null)
            {
                return NotFound("Organization not found");
            }

            var project = new Project
            {
                Name = model.Name,
                Organization = org,
                ActivityStatus = Models.Enums.ActivityStatus.Active,
                Description = model.Description
            };

            await dbContext.Projects.AddAsync(project);
            await dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch([FromBody] PatchProjectDTO model, int id)
        {
            var project = await dbContext.Projects.FindAsync(id);

            if(project == null)
            {
                return NotFound();
            }

            project.Patch(model);

            await dbContext.SaveChangesAsync();
            return Ok(project);
        }
    }
}
