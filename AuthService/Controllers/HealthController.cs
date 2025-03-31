using AuthService.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController: ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok();
        }


        [HttpGet("/encrypt/{text}")]
        public async Task<IActionResult> Encrypt(string text, EncryptionService encryptionService)
        {
            var encr = encryptionService.Encrypt(text);
            var decr = encryptionService.Decrypt(encr);

            return Ok(new {encr, decr});
        }
        [HttpGet("/decrypt/{text}")]
        public async Task<IActionResult> Decrypt(string text, EncryptionService encryptionService)
        {
            
            var decr = encryptionService.Decrypt("aGa9EsAluK37vj78JHB5osyjQdlC4OFt0tUVbEV2iItJv5IV3yY2gdwrXszZdI6fB5FcxH7ds29ewobK5afRkJpKdcZRPZe3uku3bk+JoNeH/f5MQaQQ9qR1As+l12/F");

            return Ok(decr);
        }
    }
}
