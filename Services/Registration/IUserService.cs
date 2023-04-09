using UserService.Contracts;

namespace UserService.Services.Register
{
    public interface IUserService
    {
        public Task<RegisteredUser> RegisterUser(BasicUser user);

        public Task UnregisterUser(string userId);
    }
}
