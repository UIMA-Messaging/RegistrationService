using RegistrationService.Contracts;

namespace RegistrationService.Repository
{
    public interface IUserRepository
    {
        Task<(bool, int)> CheckDisplayNameAvailability(string displayName);

        Task<RegisteredUser> GetUserByUsername(string username);

        Task<RegisteredUser> GetUserById(string id);

        Task CreateUser(RegisteredUser user);

        Task DeleteUser(string userId);

    }
}