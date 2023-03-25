using RegistrationApi.Contracts;

namespace RegistrationApi.Services.Register
{
    public interface IRegistrationService
    {
        public Task<RegisteredUser> RegisterUser(BasicUser user);

        public Task UnregisterUser(string userId);
    }
}
