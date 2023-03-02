using Microsoft.AspNetCore.Mvc;
using IdentityService.Contracts;

namespace IdentityService.Controllers
{
    [ApiController]
    [Route("api/v1/keys")]
    public class KeyController : ControllerBase
    {
        [HttpPost]
        public Task RegisterKeys([FromBody] KeyBundle bundle) 
        {
            return null;
        }
    }
}
