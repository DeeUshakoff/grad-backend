using AuthService.DTO;
using AuthService.DTO.Task;
using AuthService.Models.Task;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Controllers
{
    [ApiController]
    [Authorize]
    [Route("/api/tasks")]
    public class TaskController(AuthDbContext dbContext) : ControllerBase
    {

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(int id)
        {
            var task = await dbContext.Tasks
                .Include(t => t.AssignedTo)
                .Include(t => t.AssignedFrom)
                .Include(t => t.Project)
                .ThenInclude(p => p.Organization).Where(x => x.Id == id).FirstOrDefaultAsync();

            //var task = await dbContext.Tasks.FindAsync(id);

            if (task == null)
            {
                return NotFound();
            }

            var git = await dbContext.TaskGitRelations
                .Where(x => x.Task == task)
                .FirstOrDefaultAsync();

            return Ok( new { task, git });
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] TaskDTO model)
        {
            var assignedFrom = await dbContext.Users.FindAsync(model.AssignedFromId);
            if(assignedFrom == null)
            {
                return NotFound("Assigner not found");
            }

            var assignedTo = await dbContext.Users.FindAsync(model.AssignedToId);
            if (assignedTo == null)
            {
                return NotFound("Assigned not found");
            }

            var project = await dbContext.Projects.FindAsync(model.ProjectId);
            if (project == null)
            {
                return NotFound("Project not found");
            }

            var task = new Models.Task.Task
            {
                Title = model.Title,
                Description = model.Description,
                AssignedTo = assignedTo,
                AssignedFrom = assignedFrom,
                Project = project,
                TaskStatus = (Models.Task.TaskStatus)model.TaskStatus,
                CreatedAt = DateTime.UtcNow
            };

            await dbContext.Tasks.AddAsync(task);
            await dbContext.SaveChangesAsync();


            
            if (!model.NeedGit)
            {
                return Ok(task);
            }

            var gitRepository = await dbContext.GitRepositories.FindAsync(model.ProjectRepoId);
            if(gitRepository == null)
            {
                return NotFound("Repo not found");
            }

            var taskGitRelation = new TaskGitRelation
            {
                Task = task,
                GitRepository = gitRepository,
            };

            await dbContext.TaskGitRelations.AddAsync(taskGitRelation);
            await dbContext.SaveChangesAsync();

            return Ok(task);
        }


        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, [FromBody] TaskPatchDTO model)
        {
            if(model == null)
            {
                return BadRequest();
            }

            var entity = await dbContext.Tasks.FindAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            entity.Patch(model);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await dbContext.SaveChangesAsync();

            return Ok(entity);
        }

        [HttpGet("{id}/git")]
        public async Task<IActionResult> GetGit(int id)
        {
            var task = await dbContext.Tasks.FindAsync(id);
            if(task == null)
            {
                return NotFound();
            }

            var result = await dbContext.TaskGitRelations
                .Where(x => x.Task == task)
                .FirstOrDefaultAsync();
            if(result == null)
            {
                return Ok(result);
            }

            var git = await dbContext.GitRepositories.FindAsync(result.GitRepositoryId);
            return Ok(git);
        }

        [HttpPatch("{id}/git")]
        public async Task<IActionResult> PatchGit(int id, [FromBody] PatchTaskGitRelationDTO model)
        {
            var task = await dbContext.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            var result = await dbContext.TaskGitRelations
                .Where(x => x.Task == task)
                .FirstOrDefaultAsync();

            if (result == null)
            {
                return NotFound();
            }

            result.Patch(model);
            await dbContext.SaveChangesAsync();

            //var git = await dbContext.GitRepositories.FindAsync(result.GitRepositoryId);


            return Ok(result);
        }
    }
}
