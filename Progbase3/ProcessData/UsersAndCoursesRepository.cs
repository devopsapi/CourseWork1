using System;
using Microsoft.Data.Sqlite;
using System.Configuration;
using System.Collections.Generic;
namespace ProcessData
{
    public class UsersAndCoursesRepository
    {
        private SqliteConnection connection;

        public UsersAndCoursesRepository(string databasePath)
        {
            SqliteConnection connection = new SqliteConnection($"Data Source={databasePath}");

            this.connection = connection;
        }

        public int Insert(UsersAndCourses usersAndCourses)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();

            command.CommandText =
            @"
                INSERT INTO users_courses (user_id,course_id) 
                VALUES ($user_id, $course_id);

                SELECT last_insert_rowid();
            ";

            command.Parameters.AddWithValue("$user_id", usersAndCourses.userId);
            command.Parameters.AddWithValue("$course_id", usersAndCourses.courseId);

            int insertedId = (int)(long)command.ExecuteScalar();

            connection.Close();

            return insertedId;
        }


        public bool Delete(int userId, int courseId)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM users_courses WHERE user_id = $userId AND course_id = $courseId";
            command.Parameters.AddWithValue("$userId", userId);
            command.Parameters.AddWithValue("$courseId", courseId);

            int deletedCount = command.ExecuteNonQuery();

            bool isDeleted = false;

            if (deletedCount != 0)
            {
                isDeleted = true;
            }

            connection.Close();

            return isDeleted;
        }

        public bool isExists(int userId, int courseId)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();

            command.CommandText = @"SELECT COUNT(*) FROM users_courses WHERE course_id = $courseId AND user_id = $userId";
            command.Parameters.AddWithValue("$userId", userId);
            command.Parameters.AddWithValue("$courseId", courseId);

            int countOfFound = (int)(long)command.ExecuteScalar();

            bool isExists = false;

            if (countOfFound != 0)
            {
                isExists = true;
            }

            connection.Close();

            return isExists;
        }

        public List<int> GetAllUserCoursesId(int userId)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users_courses WHERE user_id = $userId";
            command.Parameters.AddWithValue("$userId", userId);

            SqliteDataReader reader = command.ExecuteReader();

            List<int> list = ReadAllUserCourses(reader);

            reader.Close();
            connection.Close();

            return list;
        }


        private List<int> ReadAllUserCourses(SqliteDataReader reader)
        {
            List<int> list = new List<int>();

            while (reader.Read())
            {
                int temp = reader.GetInt32(2);
                list.Add(temp);
            }
            return list;
        }
    }
}