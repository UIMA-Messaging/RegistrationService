using System.Data.Common;

namespace RegistrationService.Repository.Connection
{
    public interface IConnectionFactory
    {
        DbConnection GetOpenConnection();
    }
}