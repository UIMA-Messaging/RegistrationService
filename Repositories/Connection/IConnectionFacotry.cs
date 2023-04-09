using System.Data.Common;

namespace RegistrationApi.Repository.Connection
{
    public interface IConnectionFactory
    {
        DbConnection GetOpenConnection();
    }
}