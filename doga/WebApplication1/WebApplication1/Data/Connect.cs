using MySql.Data.MySqlClient;

namespace GradeApi.Data
{
    public class Connect
    {
        public MySqlConnection Connection;
        private string ConnectionString;

        public Connect()
        {
            string host = "localhost";
            string database = "suli";
            string user = "root"; // a te felhasználóneved
            string password = ""; // a te jelszavad

            ConnectionString = $"SERVER={host};DATABASE={database};UID={user};PASSWORD={password};SslMode=None";
            Connection = new MySqlConnection(ConnectionString);
        }
    }
}
