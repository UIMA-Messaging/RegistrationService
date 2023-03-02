using Microsoft.AspNetCore.Mvc;
using IdentityService.Contracts;
using IdentityService.Services.Register;

namespace IdentityService.Controllers
{
    [ApiController]
    [Route("api/v1/users")]
    public class RegistrationController : ControllerBase
    {
        private readonly IRegistrationService service;

        public RegistrationController(IRegistrationService service)
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