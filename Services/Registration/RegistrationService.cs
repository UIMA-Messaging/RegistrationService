using ChannelService.Repository;
using IdentityService.Contracts;
using IdentityService.Errors;

namespace IdentityService.Services.Register
{
    public class RegistrationService : IRegistrationService
    {
        private readonly UserRepository repository;

        public RegistrationService(UserRepository repository)
        {
            this.repository = repository;
        }

        public async Task<RegisteredUser> RegisterUser(BasicUser user)
        {
            var (exists, placement) = await repository.CheckUserExists(user.DisplayName);

            if (exists) 
            {
                throw new UserAlreadyExists();
            }

            string username = $"{user.DisplayName}#{placement:0000}";

            var registeredUser = new RegisteredUser
            {
                Id = Guid.NewGuid().ToString(),
                Username = username,
                DisplayName = user.DisplayName,
                Image = user.Image,
                JoinedAt = DateTime.UtcNow
            };

            await repository.CreateUser(registeredUser);

            return registeredUser;
        }

        public async Task UnregisterUser(string userId)
        {
            await repository.DeleteUser(userId);
        }
    }
}
