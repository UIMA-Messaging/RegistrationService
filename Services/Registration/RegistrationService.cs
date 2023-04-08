using ChannelService.Repository;
using RegistrationApi.Contracts;
using RegistrationApi.Exceptions;
using RegistrationApi.EventBus.RabbitMQ;

namespace RegistrationApi.Services.Register
{
    public class RegistrationService : IRegistrationService
    {
        private readonly UserRepository repository;
        private readonly IRabbitMQPublisher<RegisteredUser> rabbitMQPublisher;

        public RegistrationService(UserRepository repository, IRabbitMQPublisher<RegisteredUser> rabbitMQPublisher)
        {
            this.repository = repository;
            this.rabbitMQPublisher = rabbitMQPublisher;
        }

        public async Task<RegisteredUser> RegisterUser(BasicUser user)
        {
            var (exists, placement) = await repository.CheckDisplayNameAvailability(user.DisplayName);

            if (exists) 
            {
                throw new UserAlreadyExists();
            }

            string username = $@"{user.DisplayName}#{placement:0000}";

            var registeredUser = new RegisteredUser
            {
                Id = Guid.NewGuid().ToString(),
                Username = username,
                DisplayName = user.DisplayName,
                Image = user.Image,
                EphemeralPassword = Guid.NewGuid().ToString(),
                JoinedAt = DateTime.UtcNow,
            };

            await repository.CreateUser(registeredUser);

            rabbitMQPublisher.Publish(registeredUser, "users.new");

            return registeredUser;
        }

        public async Task UnregisterUser(string userId)
        {
            await repository.DeleteUser(userId);
        }
    }
}
