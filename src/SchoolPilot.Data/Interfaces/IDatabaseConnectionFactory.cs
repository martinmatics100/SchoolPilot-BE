

using System.Data;

namespace SchoolPilot.Data.Interfaces
{
    public interface IDatabaseConnectionFactory
    {
        IDbConnection GetConnection(string connectionString);
    }
}
