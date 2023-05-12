using Dapper;
using RegistrationService.Contracts;
using RegistrationService.Repository.Connection;

namespace RegistrationService.Repository
{
    public class UserRepository
    {
        private readonly IConnectionFactory factory;

        public UserRepository(IConnectionFactory factory)
        {
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public async Task<(bool, int)> CheckDisplayNameAvailability(string displayName)
        {
            await using var connection = factory.GetOpenConnection();
            const string sql = @"
                SELECT CASE WHEN COUNT(*) != 0 THEN 1 ELSE 0 END, (SELECT COUNT(*) + 1 FROM ""Users"")
                FROM public.""Users""
                WHERE DisplayName = @DisplayName";
            return await connection.QueryFirstAsync<(bool, int)>(sql, new { DisplayName = displayName });
        }

        public async Task<RegisteredUser> GetUserByUsername(string username)
        {
            await using var connection = factory.GetOpenConnection();
            const string sql = @"
                SELECT *
                FROM public.""Users""
                WHERE Username = @Username";
            var results = await connection.QueryAsync<RegisteredUser>(sql, new { Username = username });
            return results.FirstOrDefault();
        }

        public async Task CreateUser(RegisteredUser user)
        {
            await using var connection = factory.GetOpenConnection();
            const string sql = @"
                INSERT INTO public.""Users""(Id, Jid, Username, DisplayName, Image, JoinedAt)
                VALUES (@Id, @Jid, @Username, @DisplayName, @Image, CURRENT_TIMESTAMP)";
            await connection.ExecuteAsync(sql, user);
        }

        public async Task DeleteUser(string userId)
        {
            await using var connection = factory.GetOpenConnection();
            const string sql = @"DELETE FROM public.""Users"" WHERE Id = @Id";
            await connection.ExecuteAsync(sql, new { Id = userId });
        }
    }
}

