using MySqlConnector;
using System.Data;

namespace complaintApp_API.Data
{
    public class DataContext
    {
        private readonly IConfiguration _config;
        private readonly string? _connectingString;

        public DataContext(IConfiguration configuration)
        {
            _config = configuration;
            _connectingString = configuration.GetConnectionString("DefaultConnection");
        }

        public IDbConnection CreateConnection() => new MySqlConnection(_connectingString);
    }
}
