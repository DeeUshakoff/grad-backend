using AuthService.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("/api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] SignInDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return NotFound();
            if(!await _userManager.CheckPasswordAsync(user, model.Password))
                return Unauthorized();

            await _signInManager.SignInAsync(user, isPersistent: false);

            return Ok(new { message = "Signed in successfully" });
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpDTO model)
        {
            var user = new IdentityUser { UserName = model.Username, Email = model.Email };

            if(model.Password != model.RepeatPassword)
            {
                return BadRequest();
            }

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

            await _signInManager.SignInAsync(user, isPersistent: false);
            return Ok();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if(user == null)
            {
                return NotFound();
            }
            await _userManager.DeleteAsync(user);
            return Ok();
        }

        [HttpPost("signout")]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return SignOut("Cookies");
        }
    }
}
