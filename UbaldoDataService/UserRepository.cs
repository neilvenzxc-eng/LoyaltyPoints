using MySql.Data.MySqlClient;
using LoyaltyPoints.Models;
using System.Text.Json;
using System.IO;
using System.Collections.Generic;

namespace LoyaltyPoints.Data
{
    public class UserRepository
    {
        //database connection
        private string connStr = "Server=localhost;Database=ubaldo_db;Uid=root;Pwd=;";
        private string jsonPath = "users_snapshot.json";

        //check if user exists
        public User GetUser(string u, string p)
        {
            using var c = new MySqlConnection(connStr);
            c.Open();
            var cmd = new MySqlCommand("SELECT * FROM users WHERE username=@u AND password=@p", c);
            cmd.Parameters.AddWithValue("@u", u);
            cmd.Parameters.AddWithValue("@p", p);

            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;

            return new User
            {
                Id = r.GetInt32("id"),
                Username = r.GetString("username"),

                Password = r.IsDBNull(r.GetOrdinal("password")) ? string.Empty : r.GetString("password"),

                Points = r.GetInt32("points")
            };
        }

        //update user points
        public void UpdatePoints(int id, int pts)
        {
            using var c = new MySqlConnection(connStr);
            c.Open();
            var cmd = new MySqlCommand("UPDATE users SET points=@p WHERE id=@id", c);
            cmd.Parameters.AddWithValue("@p", pts);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();

            SyncJsonFile();
        }

        //create new user
        public void CreateUser(string u, string p)
        {
            using var c = new MySqlConnection(connStr);
            c.Open();
            var cmd = new MySqlCommand("INSERT INTO users (username, password, points) VALUES (@u, @p, 0)", c);
            cmd.Parameters.AddWithValue("@u", u);
            cmd.Parameters.AddWithValue("@p", p);
            cmd.ExecuteNonQuery();

            SyncJsonFile();
        }

        //json database
        private void SyncJsonFile()
        {
            try
            {
                List<User> allUsers = new List<User>();

                using (var connection = new MySqlConnection(connStr))
                {
                    connection.Open();
                    string query = "SELECT id, username, password, points FROM users";

                    using (var command = new MySqlCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            allUsers.Add(new User
                            {
                                Id = reader.GetInt32("id"),
                                Username = reader.GetString("username"),
                                Password = reader.IsDBNull(reader.GetOrdinal("password")) ? string.Empty : reader.GetString("password"),
                                Points = reader.GetInt32("points")
                            });
                        }
                    }
                }

                string sharedFolder = @"C:\Users\Aiyan\Documents\GitHub\Ubaldo_Act2Continue";
                string filePath = Path.Combine(sharedFolder, "users_snapshot.json");

                if (!Directory.Exists(sharedFolder))
                {
                    Directory.CreateDirectory(sharedFolder);
                }

                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(allUsers, options);

                File.WriteAllText(filePath, jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"JSON Sync Warning: {ex.Message}");
            }

        }
        public bool DeleteUser(int id)
        {
            using var c = new MySqlConnection(connStr);
            c.Open();

            var cmd = new MySqlCommand("DELETE FROM users WHERE id = @id", c);
            cmd.Parameters.AddWithValue("@id", id);
            int rowsAffected = cmd.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                SyncJsonFile();
                return true;
            }

            return false;
        }
    }
}