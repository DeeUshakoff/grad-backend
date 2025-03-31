using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace AuthService.Controllers
{
    [ApiController]
    [Authorize]
    [Route("/api/users")]
    public class UserController(UserManager<IdentityUser> userManager, AuthDbContext dbContext) : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager = userManager;

        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            if(User == null)
            {
                return Unauthorized("Not authenticated");
            }

            if (!User.Identity.IsAuthenticated)
                return Unauthorized("Not authenticated");


            var user = await _userManager.GetUserAsync(User);
            return Ok(user);
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> GetUser(string name)
        {
            var user = await _userManager.FindByNameAsync(name);
            if(user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }


        [HttpGet("{name}/organizations")]
        public async Task<IActionResult> Organizations(string name)
        {
            var user = await _userManager.FindByNameAsync(name);
            var orgs = dbContext.Organizations.Where(x => x.Owner == user);
            return Ok(orgs);
        }

        [HttpGet("{name}/tasks")]
        public async Task<IActionResult> Tasks(string name)
        {
            var user = await _userManager.FindByNameAsync(name);
            if (user == null)
            {
                return NotFound();
            }
            var tasks = dbContext.Tasks.Where(x => x.AssignedTo == user);

            return Ok(tasks);
        }
    }
}
