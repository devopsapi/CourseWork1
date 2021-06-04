using System;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace ProcessData
{
    public class UserRepository
    {
        private SqliteConnection connection;

        public UserRepository(string databasePath)
        {
            SqliteConnection connection = new SqliteConnection($"Data Source={databasePath}");

            this.connection = connection;
        }

        public bool UserExists(string username)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM users WHERE username = $username";
            command.Parameters.AddWithValue("$username", username);

            int countOfFound = (int)(long)command.ExecuteScalar();

            bool isExists = false;

            if (countOfFound != 0)
            {
                isExists = true;
            }

            connection.Close();

            return isExists;
        }

        public User[] GetAllUsers()
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users";

            SqliteDataReader reader = command.ExecuteReader();

            List<User> list = ReadUsers(reader);

            reader.Close();

            connection.Close();

            User[] users = new User[list.Count];
            list.CopyTo(users);

            return users;
        }

        public int[] GetAllUsersIds()
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT id FROM users";

            SqliteDataReader reader = command.ExecuteReader();

            List<int> idsList = GetListOfIds(reader);

            reader.Close();

            connection.Close();

            int[] ids = new int[idsList.Count];
            idsList.CopyTo(ids);

            return ids;
        }

        public int Insert(User user)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();

            command.CommandText =
            @"
                INSERT INTO users (username, password, fullname, createdAt) 
                VALUES ($username, $password, $fullname, $createdAt);

                SELECT last_insert_rowid();
            ";

            command.Parameters.AddWithValue("$username", user.username);
            command.Parameters.AddWithValue("$password", user.password);
            command.Parameters.AddWithValue("$fullname", user.fullname);
            command.Parameters.AddWithValue("$createdAt", user.createdAt.ToString("o"));

            int insertedId = (int)(long)command.ExecuteScalar();

            connection.Close();

            return insertedId;
        }

        public User GetByUsername(string username)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users WHERE username = $username";
            command.Parameters.AddWithValue("$username", username);

            SqliteDataReader reader = command.ExecuteReader();

            User user = null;

            if (reader.Read())
            {
                user = ReadUser(reader);
            }

            reader.Close();

            connection.Close();

            return user;
        }

        public bool Update(int userId, User user)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"UPDATE users
                                    SET username = $username,
                                        password = $password,
                                        fullname = $fullname WHERE id = $userId";
            command.Parameters.AddWithValue("$userId", userId);
            command.Parameters.AddWithValue("$username", user.username);
            command.Parameters.AddWithValue("$password", user.password);
            command.Parameters.AddWithValue("$fullname", user.fullname);

            int nChanged = command.ExecuteNonQuery();

            bool isUpdated = false;

            if (nChanged != 0)
            {
                isUpdated = true;
            }

            connection.Close();

            return isUpdated;
        }

        public bool DeleteByUsername(string username)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM users WHERE username = $username";
            command.Parameters.AddWithValue("$username", username);

            int deletedCount = command.ExecuteNonQuery();

            bool isDeleted = false;

            if (deletedCount != 0)
            {
                isDeleted = true;
            }

            connection.Close();

            return isDeleted;
        }

        private static List<User> ReadUsers(SqliteDataReader reader)
        {
            List<User> usersList = new List<User>();

            while (reader.Read())
            {
                User user = ReadUser(reader);

                usersList.Add(user);
            }

            return usersList;
        }

        private static User ReadUser(SqliteDataReader reader)
        {
            int id = reader.GetInt32(0);
            string username = reader.GetString(1);
            string password = reader.GetString(2);
            string fullname = reader.GetString(3);
            DateTime createdAt = reader.GetDateTime(4);
            int imported = reader.GetInt32(5);

            User user = new User(id, username, password, fullname, createdAt);
            user.imported = (imported == 1) ? true : false;

            return user;
        }

        private static List<int> GetListOfIds(SqliteDataReader reader)
        {
            List<int> list = new List<int>();

            while (reader.Read())
            {
                list.Add(reader.GetInt32(0));
            }

            return list;
        }
    }
}