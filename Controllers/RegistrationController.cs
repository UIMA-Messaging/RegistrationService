using Microsoft.AspNetCore.Mvc;
using RegistrationApi.Contracts;
using RegistrationApi.Services.Register;

namespace RegistrationApi.Controllers
{
    [ApiController]
    [Route("users")]
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