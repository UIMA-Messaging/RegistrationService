using System.Data.Common;

namespace UserService.Repository.Connection
{
    public interface IConnectionFactory
    {
        DbConnection GetOpenConnection();
    }
}