using MySql.Data.MySqlClient;
using System.Data;
using System.Reflection.PortableExecutable;

namespace MyCalculator.DAL
{
    public class DataAccessService : IDataAccess
    {
        private readonly string _connectionString;
        private MySqlConnection _connection;

        public DataAccessService()
        {
            _connectionString = "server=localhost;user=root;database=calculator;port=3306;password=juanjo";
        }

        public IList<string> GetComputationsHistory()
        {
            var result = new List<string>();

            using (MySqlConnection conn = new MySqlConnection())
            {
                conn.ConnectionString = _connectionString;

                conn.Open();

                MySqlCommand cmd = new MySqlCommand();

                cmd.Connection = conn;

                cmd.CommandText = "SELECT * FROM computations ORDER BY id DESC limit 10";

                MySqlDataReader dataReader = cmd.ExecuteReader();

                DataTable dt = new DataTable();
                dt.Load(dataReader);

                foreach (DataRow row in dt.Rows)
                {
                    result.Add(row["computation"].ToString() ?? string.Empty);
                }
            }

            return result;
        }

        public void SaveComputation(string computation)
        {
            using (MySqlConnection conn = new MySqlConnection())
            {
                conn.ConnectionString = _connectionString;

                conn.Open();

                var query = $"INSERT INTO computations(id, computation) VALUES(NULL, '{computation}')";

                MySqlCommand command = new MySqlCommand(query, conn);
                
                command.ExecuteNonQuery();
            }
        }
    }
}
