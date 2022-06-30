using System.Data.OracleClient;
using System.Data;

namespace ExternalAPIs.Helper
{
    public interface IOracleServerConnectionProvider
    {
        public IDbConnection GetDbConnection();
    }

    public class OracleServerConnectionProvider: IOracleServerConnectionProvider
    {
        private readonly string _connectionString;

        public OracleServerConnectionProvider(string connectionString)
        {
            _connectionString = connectionString;
        }
 

        public IDbConnection GetDbConnection()
        {
            return new OracleConnection(_connectionString);
        }
    }

}
