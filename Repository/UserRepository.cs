using ChannelService.Repository.Connection;
using Dapper;
using IdentityService.Contracts;

namespace ChannelService.Repository
{
    public class UserRepository
    {
        private readonly IConnectionFactory factory;

        public UserRepository(IConnectionFactory factory)
        {
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public async Task<(bool, int)> CheckUserExists(string displayName)
        {
            using var connection = factory.GetOpenConnection();
            const string sql = @"
                SELECT CASE WHEN COUNT(*) != 0 THEN 1 ELSE 0 END, COUNT(*) + 1
                FROM Users
                WHERE DisplayName = @DisplayName";
            return await connection.QueryFirstAsync<(bool, int)>(sql, new { DisplayName = displayName });
        }

        public async Task CreateUser(RegisteredUser user)
        {
            using var connection = factory.GetOpenConnection();
            const string sql = @"
                INSERT INTO Users(Id, Username, DisplayName, Image, JoinedAt)
                VALUES (@Id, @Username, @DisplayName, @Image, CURRENT_TIMESTAMP)";
            await connection.ExecuteAsync(sql, user);
        }

        public async Task DeleteUser(string userId)
        {
            using var connection = factory.GetOpenConnection();
            const string sql = @"DELETE FROM Users WHERE Id = @Id";
            await connection.ExecuteAsync(sql, new { Id = userId });
        }
    }
}

