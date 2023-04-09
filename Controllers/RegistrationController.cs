using Microsoft.AspNetCore.Mvc;
using UserService.Contracts;
using UserService.Services.Register;

namespace UserService.Controllers
{
    [ApiController]
    [Route("users")]
    public class RegistrationController : ControllerBase
    {
        private readonly IUserService service;

        public RegistrationController(IUserService service)
        {
            this.service = service;
        }

        [HttpPost("register")]
        public async Task<RegisteredUser> RegisterUser([FromBody] BasicUser user)
        {
            return await service.RegisterUser(user);
        }

        [HttpDelete("unregister/{userId}")]
        public async Task UnregisterUser(string userId)
        {
            await service.UnregisterUser(userId);
        }
    }
}