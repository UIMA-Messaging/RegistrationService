using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RegistrationService.Contracts;
using RegistrationService.Exceptions;
using RegistrationService.RabbitMQ;
using RegistrationService.Repository;

namespace RegistrationService.Services.Tests
{
    [TestClass]
    public class UserServiceTests
    {
        public Mock<IRabbitMQPublisher<RegisteredUser>> mockUserRegistrationPublisher;
        public Mock<IRabbitMQPublisher<ExchangeKeys>> mockUserExchangeKeyPublisher;
        public Mock<IUserRepository> mockUserRepository;
        public string jabberHost;

        private UserService userService;

        [TestInitialize]
        public void Initialize()
        {
            mockUserExchangeKeyPublisher = new Mock<IRabbitMQPublisher<ExchangeKeys>>();
            mockUserRegistrationPublisher = new Mock<IRabbitMQPublisher<RegisteredUser>>();
            mockUserRepository = new Mock<IUserRepository>();
            jabberHost = "localhost";

            userService = new UserService(
                mockUserRepository.Object,
                jabberHost,
                mockUserRegistrationPublisher.Object,
                mockUserExchangeKeyPublisher.Object);
        }

        public static string GenerateHexKey() => Guid.NewGuid().ToString("N");

        public static ExchangeKeys GetExchangeKeys() => new()
        {
            IdentityKey = GenerateHexKey(),
            SignedPreKey = GenerateHexKey(),
            OneTimePreKeys = new[]
            {
                GenerateHexKey(),
                GenerateHexKey(),
                GenerateHexKey()
            },
            Signature = "thisIsASignature"
        };

        public static BasicUser GetBasicUser() => new()
        {
            DisplayName = "thisIsADisplayName",
            ExchangeKeys = GetExchangeKeys()
        };

        public static RegisteredUser GetRegisteredUser() => new() 
        {
            Id = Guid.NewGuid().ToString(),
            // Rest not needed
        };

        [TestMethod]
        public async Task RegisterUserTest_ReturnsRegisteredUser()
        {
            var basicUser = GetBasicUser();

            mockUserRepository
                .Setup(mock => mock.CheckDisplayNameAvailability(basicUser.DisplayName))
                .ReturnsAsync((false, 123));

            var registeredUser = await userService.RegisterUser(basicUser);

            Assert.AreEqual(basicUser.DisplayName, registeredUser.DisplayName);
            Assert.IsNotNull(registeredUser.Id);
            Assert.IsNotNull(registeredUser.Jid);
            Assert.IsNotNull(registeredUser.Id);
            Assert.IsNotNull(registeredUser.Id);
            Assert.IsNotNull(registeredUser.Username);
            Assert.IsTrue(registeredUser.Username.Contains(basicUser.DisplayName) 
                && registeredUser.Username.Contains(123.ToString()));

            // Jabber-related
            Assert.IsTrue(registeredUser.Jid.Contains(jabberHost)
                && registeredUser.Jid.Contains(basicUser.DisplayName));
            Assert.IsNotNull(registeredUser.EphemeralPassword);
        }

        [TestMethod]
        public void RegisterUserTest_ThrowsUserAlreadyExists()
        {
            var basicUser = GetBasicUser();

            mockUserRepository
                .Setup(mock => mock.CheckDisplayNameAvailability(basicUser.DisplayName))
                .ReturnsAsync((true, 0));

            var registeredUser = () => userService.RegisterUser(basicUser);

            Assert.ThrowsExceptionAsync<UserAlreadyExists>(registeredUser);
        }

        [TestMethod]
        public async Task RegisterUserTest_InsertsUserToDatabase()
        {
            var basicUser = GetBasicUser();

            mockUserRepository
                .Setup(mock => mock.CheckDisplayNameAvailability(basicUser.DisplayName))
                .ReturnsAsync((false, 0));

            var registeredUser = await userService.RegisterUser(basicUser);

            mockUserRepository.Verify(mock => mock.CreateUser(registeredUser), Times.Once);
        }

        [TestMethod]
        public async Task RegisterUserTest_DispatchesRegisteredUserToRabbitMq()
        {
            var basicUser = GetBasicUser();

            mockUserRepository
                .Setup(mock => mock.CheckDisplayNameAvailability(basicUser.DisplayName))
                .ReturnsAsync((false, 0));

            var registeredUser = await userService.RegisterUser(basicUser);

            mockUserRegistrationPublisher.Verify(mock => mock.Publish(registeredUser, "users.new"), Times.Once);
        }

        [TestMethod]
        public async Task RegisterUserTest_DispatchesExchangeKeysToRabbitMq()
        {
            var basicUser = GetBasicUser();

            mockUserRepository
                .Setup(mock => mock.CheckDisplayNameAvailability(basicUser.DisplayName))
                .ReturnsAsync((false, 0));

            _ = await userService.RegisterUser(basicUser);

            mockUserExchangeKeyPublisher.Verify(mock => mock.Publish(basicUser.ExchangeKeys, "users.new.keys"), Times.Once);
        }

        [TestMethod]
        public async Task UnregisterUserTest_DeletesUserFromDatabase()
        {
            var regiteredUser = GetRegisteredUser();

            mockUserRepository
                .Setup(mock => mock.GetUserById(regiteredUser.Id))
                .ReturnsAsync(regiteredUser);

            await userService.UnregisterUser(regiteredUser.Id);

            mockUserRepository.Verify(mock => mock.DeleteUser(regiteredUser.Id), Times.Once);
        }

        [TestMethod]
        public async Task UnregisterUserTest_DispatchesDeletedUserToRabbitMq()
        {
            var regiteredUser = GetRegisteredUser();

            mockUserRepository
                .Setup(mock => mock.GetUserById(regiteredUser.Id))
                .ReturnsAsync(regiteredUser);

            await userService.UnregisterUser(regiteredUser.Id);

            mockUserRegistrationPublisher.Verify(mock => mock.Publish(regiteredUser, "users.remove"), Times.Once);
        }

        [TestMethod]
        public void UnregisterUserTest_ThrowsUserNotFound()
        {
            var regiteredUser = GetRegisteredUser();

            var registeredUser = () => userService.UnregisterUser(regiteredUser.Id);

            Assert.ThrowsExceptionAsync<UserNotFound>(registeredUser);
        }
    }
}