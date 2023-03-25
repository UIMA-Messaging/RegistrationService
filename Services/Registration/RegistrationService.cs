using ChannelService.Repository;
using RegistrationApi.Contracts;
using RegistrationApi.Errors;
using RegistrationApi.EventBus;

namespace RegistrationApi.Services.Register
{
    public class RegistrationService : IRegistrationService
    {
        private readonly UserRepository repository;
        private readonly RabbitMQHelper<RegisteredUser> bus;

        public RegistrationService(UserRepository repository, RabbitMQHelper<RegisteredUser> bus)
        {
            this.repository = repository;
            this.bus = bus;
        }

        public async Task<RegisteredUser> RegisterUser(BasicUser user)
        {
            var (exists, placement) = await repository.CheckDisplayNameAvailability(user.DisplayName);

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

            bus.Send(registeredUser, "users.new");

            return registeredUser;
        }

        public async Task UnregisterUser(string userId)
        {
            await repository.DeleteUser(userId);
        }
    }
}
