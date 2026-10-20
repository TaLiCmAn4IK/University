using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using WatchShop.Models;

namespace WatchShop.Services
{
    /// <summary>
    /// Репозиторий пользователей. Аутентификация и регистрация через ADO.NET.
    /// </summary>
    public class UserRepository
    {
        private readonly string connectionString;

        public UserRepository()
        {
            connectionString = ConfigurationManager.ConnectionStrings["WatchShopConnection"].ConnectionString;
        }

        /// <summary>Поиск пользователя по логину/паролю; null если не найден.</summary>
        public User? Authenticate(string username, string password)
        {
            using var conn = new SqlConnection(connectionString);
            conn.Open();

            using var cmd = new SqlCommand(
                "SELECT Id, Username, Password, Role, Email FROM Users " +
                "WHERE Username=@u AND Password=@p AND IsActive=1", conn);
            cmd.Parameters.Add("@u", SqlDbType.NVarChar, 50).Value = username;
            cmd.Parameters.Add("@p", SqlDbType.NVarChar, 100).Value = password;

            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            return new User
            {
                Id = reader.GetInt32(0),
                Username = reader.GetString(1),
                Password = reader.GetString(2),
                Role = reader.GetString(3),
                Email = reader.IsDBNull(4) ? "" : reader.GetString(4)
            };
        }

        /// <summary>
        /// Регистрация нового пользователя. Транзакция страхует от частичной
        /// записи. UNIQUE-индекс на Username не даст создать дубликат.
        /// </summary>
        public int Register(string username, string password, string email)
        {
            using var conn = new SqlConnection(connectionString);
            conn.Open();
            using var tx = conn.BeginTransaction();
            try
            {
                using var cmd = new SqlCommand(
                    "INSERT INTO Users (Username, Password, Email, Role) " +
                    "OUTPUT INSERTED.Id " +
                    "VALUES (@u, @p, @e, 'Client')", conn, tx);
                cmd.Parameters.Add("@u", SqlDbType.NVarChar, 50).Value = username;
                cmd.Parameters.Add("@p", SqlDbType.NVarChar, 100).Value = password;
                cmd.Parameters.Add("@e", SqlDbType.NVarChar, 100).Value = email ?? "";

                int newId = (int)cmd.ExecuteScalar();
                tx.Commit();
                return newId;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public bool UsernameExists(string username)
        {
            using var conn = new SqlConnection(connectionString);
            conn.Open();
            using var cmd = new SqlCommand(
                "SELECT COUNT(*) FROM Users WHERE Username=@u", conn);
            cmd.Parameters.Add("@u", SqlDbType.NVarChar, 50).Value = username;
            return (int)cmd.ExecuteScalar() > 0;
        }

        public void UpdateProfile(int userId, string email, string newPassword)
        {
            using var conn = new SqlConnection(connectionString);
            conn.Open();
            using var cmd = new SqlCommand(
                "UPDATE Users SET Email=@e, Password=@p WHERE Id=@id", conn);
            cmd.Parameters.Add("@e",  SqlDbType.NVarChar, 100).Value = email ?? "";
            cmd.Parameters.Add("@p",  SqlDbType.NVarChar, 100).Value = newPassword ?? "";
            cmd.Parameters.Add("@id", SqlDbType.Int).Value           = userId;
            cmd.ExecuteNonQuery();
        }
    }
}
