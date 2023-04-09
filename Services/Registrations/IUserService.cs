using RegistrationService.Contracts;

namespace RegistrationService.Services.Registerations
{
    public interface IUserService
    {
        public Task<RegisteredUser> RegisterUser(BasicUser user);

        public Task UnregisterUser(string userId);
    }
}
