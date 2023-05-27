using Microsoft.AspNetCore.Mvc;
using RegistrationService.Services;
using RegistrationService.Contracts;

namespace RegistrationService.Controllers
{
    [ApiController]
    [Route("users")]
    public class RegistrationController : ControllerBase
    {
        private readonly UserService service;

        public RegistrationController(UserService service)
        {
            this.service = service;
        }

        [HttpPost("register")]
        public async Task<RegisteredUser> RegisterUser([FromBody] BasicUser user)
        {
            return await service.RegisterUser(user);
        }

        [HttpDelete("unregister/{id}")]
        public async Task UnregisterUser(string id)
        {
            await service.UnregisterUser(id);
        }
    }
}