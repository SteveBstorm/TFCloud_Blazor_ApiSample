using Dapper;
using Microsoft.Data.SqlClient;
using TFCloud_Blazor_ApiSample.Models;

namespace TFCloud_Blazor_ApiSample.Repos
{
    public class UserRepo
    {
        private readonly SqlConnection _connection;

        public UserRepo(SqlConnection connection)
        {
            _connection = connection;
        }

        public bool Register(string email, string password, string nickname)
        {
            string sql = "INSERT INTO Users (Email, Password, Nickname) " +
                "VALUES (@email, @password, @nickname)";

            return _connection.Execute(sql, new { email, password, nickname }) > 0;
        }

        public User Login(string email, string password)
        {
            string sql = "SELECT * FROM Users WHERE Email = @email AND Password = @password";
            return _connection.QuerySingle<User>(sql, new { email, password });

        }

        public List<User> GetAll() 
        {
            string sql = "SELECT * FROM Users";

            return _connection.Query<User>(sql).ToList();

        }

        public string GetPassword(string email)
        {
            return _connection.QueryFirst<string>("SELECT Password FROM Users WHERE Email  = @email", new { email });
        }
    }
}
