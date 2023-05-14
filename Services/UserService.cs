using RegistrationService.RabbitMQ;
using RegistrationService.Contracts;
using RegistrationService.Exceptions;
using RegistrationService.Repository;
using System.Diagnostics;

namespace RegistrationService.Services
{
    public class UserService
    {
        private readonly UserRepository repository;
        private readonly string jabberHost;
        private readonly IRabbitMQPublisher<RegisteredUser> registeredUserPublisher;
        private readonly IRabbitMQPublisher<ExchangeKeys> keyExchangePublisher;

        public UserService(UserRepository repository, string jabberHost, IRabbitMQPublisher<RegisteredUser> registeredUserPublisher, IRabbitMQPublisher<ExchangeKeys> keyExchangePublisher)
        {
            this.repository = repository;
            this.jabberHost = jabberHost;
            this.registeredUserPublisher = registeredUserPublisher;
            this.keyExchangePublisher = keyExchangePublisher;
        }

        public async Task<RegisteredUser> RegisterUser(BasicUser user)
        {
            var (exists, placement) = await repository.CheckDisplayNameAvailability(user.DisplayName);

            if (exists)
            {
                throw new UserAlreadyExists();
            }

            var username = $@"{user.DisplayName}#{placement:0000}";

            var registeredUser = new RegisteredUser
            {
                Id = Guid.NewGuid().ToString(),
                Jid = $@"{username}@{jabberHost}",
                Username = username,
                DisplayName = user.DisplayName,
                Image = user.Image,
                EphemeralPassword = Guid.NewGuid().ToString(),
                JoinedAt = DateTime.UtcNow,
            };

            await repository.CreateUser(registeredUser);

            user.ExchangeKeys.UserId = registeredUser.Id;

            try
            {
                registeredUserPublisher.Publish(registeredUser, "users.new");
            }
            catch 
            {
                Debug.WriteLine($" [x] Could not publish registered user `{registeredUser.Id}`");
            }

            try
            {
                keyExchangePublisher.Publish(user.ExchangeKeys, "users.new.keys");
            }
            catch 
            {
                Debug.WriteLine($" [x] Could not publish exchange keys from user `{registeredUser.Id}`");
            }

            return registeredUser;
        }

        public async Task UnregisterUser(string id)
        {
            var user = await repository.GetUserById(id) ?? throw new UserNotFound();
            await repository.DeleteUser(id);

            try
            {
                registeredUserPublisher.Publish(user, "users.remove");
            }
            catch
            {
                Debug.WriteLine($" [x] Could not publish unregistered user `{user.Id}`");
            }
        }
    }
}
