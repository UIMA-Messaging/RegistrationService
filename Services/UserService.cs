using RegistrationService.Contracts;
using RegistrationService.EventBus.RabbitMQ;
using RegistrationService.Exceptions;
using RegistrationService.Repository;

namespace RegistrationService.Services
{
    public class UserService
    {
        private readonly UserRepository repository;
        private readonly IRabbitMQPublisher<RegisteredUser> rabbitMQPublisher;

        public UserService(UserRepository repository, IRabbitMQPublisher<RegisteredUser> rabbitMQPublisher)
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

            try
            {
                rabbitMQPublisher.Publish(registeredUser, "users.new");
            }
            catch { }

            return registeredUser;
        }

        public async Task UnregisterUser(string userId)
        {
            await repository.DeleteUser(userId);
        }
    }
}
