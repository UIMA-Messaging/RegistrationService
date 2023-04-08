using Npgsql;
using System.Data.Common;

namespace ChannelService.Repository.Connection
{
    public class ConnectionFactory : IConnectionFactory
    {
        private readonly string connectionString;

        public ConnectionFactory(string connectionString)
        {
            this.connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public DbConnection GetOpenConnection()
        {
            var connection = new NpgsqlConnection(connectionString);
            const int retryCount = 5;
            var retries = 0;
            while (true)
            {
                try
                {
                    connection.Open();
                    break;
                }
                catch (Exception)
                {
                    retries++;
                    if (retries == retryCount)
                    {
                        throw;
                    }
                    Thread.Sleep((int)Math.Pow(retries, 2) * (500 + new Random().Next(500)));
                }
            }
            return connection;
        }
    }
}
