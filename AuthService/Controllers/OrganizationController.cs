using AuthService.DTO.Organization;
using AuthService.Helpers;
using AuthService.Models.Organization;
using AuthService.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace AuthService.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class OrganizationController(
        AuthDbContext dbContext,
        UserManager<IdentityUser> userManager,
        EncryptionService encryptionService
        ) : ControllerBase
    {
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var org = await dbContext.Organizations.FindAsync(id);
            var git = await dbContext.OrganizationGitRelations
                .Where(x => x.Organization == org)
                .FirstOrDefaultAsync();
            
            return Ok(new { org, git });
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateOrganizationDTO model)
        {
            if (await dbContext.Organizations.Where(x => x.Name == model.Name).AnyAsync())
            {
                return BadRequest("Org already exist");
            }

            var currentUser = await userManager.GetUserAsync(User);
            var organization = new Organization
            {
                ActivityStatus = Models.Enums.ActivityStatus.Active,
                Name = model.Name,
                Description = model.Description,
                Owner = currentUser,
            };

            await dbContext.Organizations.AddAsync(organization);
            await dbContext.SaveChangesAsync();


            var creatorOrgRelation = new UserOrganizationRelation 
            { 
                Organization = organization,
                User = currentUser,
            };

            await dbContext.AddAsync(creatorOrgRelation);
            await dbContext.SaveChangesAsync();
            return Ok(organization);
        }

        [HttpPost("{id}/git")]
        public async Task<IActionResult> LinkGit(int id, [FromBody] CreateOrganizationGitRelation model)
        {
            var organization = await dbContext.Organizations.FindAsync(id);
            if (organization == null)
            {
                return NotFound("Organization not found");
            }

            var gitRelation = await dbContext.OrganizationGitRelations
                .FirstOrDefaultAsync(x => x.Organization == organization);

            if (gitRelation != null)
            {
                return BadRequest("Already exists");
            }

            gitRelation = new OrganizationGitRelation
            {
                GitName = model.GitName,
                GitToken = model.GitToken,
                Organization = organization,
            };

            gitRelation.EncryptSelf(encryptionService);

            dbContext.OrganizationGitRelations.Add(gitRelation);
            await dbContext.SaveChangesAsync();

            return Ok(gitRelation);
        }

        [HttpPatch("{id}/git")]
        public async Task<IActionResult> PatchGit(int id, [FromBody] PatchOrganizationGitRelationDTO model)
        {
            var organization = await dbContext.Organizations.FindAsync(id);
            if (organization == null)
            {
                return NotFound("Organization not found");
            }

            var gitRelation = await dbContext.OrganizationGitRelations
                .FirstOrDefaultAsync(x => x.Organization == organization);

            if (gitRelation == null)
            {
                return NotFound("Git relation not found");
            }

            gitRelation.GitName = model.GitName;
            await dbContext.SaveChangesAsync();

            return Ok(gitRelation);
        }

        [HttpGet("{id}/projects")]
        public async Task<IActionResult> GetProjects(int id)
        {
            var org = await dbContext.Organizations.FindAsync(id);

            if(org == null)
            {
                return NotFound();
            }

            var projects = dbContext.Projects.Where(x => x.Organization == org);

            return Ok(projects);
        }

        [HttpGet("{id}/repos")]
        public async Task<IActionResult> GetRepos(int id)
        {
            var org = await dbContext.Organizations.FindAsync(id);

            if (org == null)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpGet("{id}/members")]
        public async Task<IActionResult> GetMembers(int id)
        {
            var org = await dbContext.Organizations.FindAsync(id);

            if (org == null)
            {
                return NotFound();
            }

            var members = dbContext.UserOrganizationRelations
                .Where(x => x.Organization == org)
                .Select(x => x.User);

            return Ok(members);
        }
        [HttpGet("{id}/members/search/{name}")]
        public async Task<IActionResult> SearchMembers(
            int id,
            [FromQuery] string userName,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var org = await dbContext.Organizations.FindAsync(id);

            if (org == null)
            {
                return NotFound();
            }

            var members = dbContext.UserOrganizationRelations
                .Where(x => x.Organization == org)
                .Select(x => x.User);
            
            return Ok(members);
        }

        [HttpGet("{id}/members/search")]
        public async Task<IActionResult> SearchUsersByUserName(
            int id,
        [FromQuery] string userName,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return BadRequest("UserName parameter is required.");
            }

            var org = await dbContext.Organizations.FindAsync(id);

            if (org == null)
            {
                return NotFound();
            }

            var query = dbContext.UserOrganizationRelations
                .Where(x => x.Organization == org)
                .Select(x => x.User)
                .Where(u => u.UserName.Contains(userName, StringComparison.OrdinalIgnoreCase));

            var totalCount = await query.CountAsync();
            var users = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                Users = users
            };

            return Ok(result);
        }


        [HttpPost("{id}/members")]
        public async Task<IActionResult> AddMember(int id)
        {
            var org = await dbContext.Organizations.FindAsync(id);

            if (org == null)
            {
                return NotFound();
            }

            var members = dbContext.UserOrganizationRelations
                .Where(x => x.Organization == org)
                .Select(x => x.User);

            return Ok(members);
        }

        //// PUT api/<OrganizationController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/<OrganizationController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
