using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System;
namespace ProcessData
{
    public class TemporaryLectureRepository
    {
        private SqliteConnection connection;
        public TemporaryLectureRepository(string databasePath)
        {
            string connectionPath = $"Data Source={databasePath}";

            this.connection = new SqliteConnection(connectionPath);
        }

        public int Insert(Lecture lecture)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();

            command.CommandText =
            @"
                INSERT INTO tempLectures (topic,description,duration,createdAt)
                VALUES ($topic,$description,$duration,$createdAt);
                SELECT last_insert_rowid();
            ";

            command.Parameters.AddWithValue("$topic", lecture.topic);
            command.Parameters.AddWithValue("$description", lecture.description);
            command.Parameters.AddWithValue("$duration", lecture.duration);
            command.Parameters.AddWithValue("$createdAt", lecture.createdAt);

            int insertedId = (int)(long)command.ExecuteScalar();

            connection.Close();

            return insertedId;
        }

        public bool Update(int lectureId, Lecture lecture)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText =
            @"
                UPDATE tempLectures SET topic = $topic, description = $description,
                duration = $duration, createdAt = $createdAt WHERE id = $id
            ";
            command.Parameters.AddWithValue("$id", lectureId);
            command.Parameters.AddWithValue("$topic", lecture.topic);
            command.Parameters.AddWithValue("$description", lecture.description);
            command.Parameters.AddWithValue("$duration", lecture.duration);
            command.Parameters.AddWithValue("$createdAt", lecture.createdAt);

            int nChanged = command.ExecuteNonQuery();

            bool isUpdated = false;

            if (nChanged != 0)
            {
                isUpdated = true;
            }

            connection.Close();

            return isUpdated;
        }

        public Lecture[] GetAll()
        {
            List<Lecture> list = new List<Lecture>();
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM tempLectures";
            SqliteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Lecture lecture = ReadLecture(reader);
                list.Add(lecture);
            }

            Lecture[] allLectures = new Lecture[list.Count];
            list.CopyTo(allLectures);

            return allLectures;
        }

        public bool DeleteById(int id)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();

            command.CommandText = @"DELETE FROM tempLectures WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);

            int deletedCount = command.ExecuteNonQuery();

            bool isDeleted = false;

            if (deletedCount != 0)
            {
                isDeleted = true;
            }

            connection.Close();

            return isDeleted;
        }

        /*  public bool DeleteAll()
         {
             connection.Open();

             SqliteCommand command = connection.CreateCommand();

             command.CommandText = @"DELETE * FROM tempLectures";

             int deletedCount = command.ExecuteNonQuery();

             bool isDeleted = false;

             if (deletedCount != 0)
             {
                 isDeleted = true;
             }

             connection.Close();

             return isDeleted;

         } */

        public int GetSearchPagesCount(int pageSize, string searchValue)
        {
            if (pageSize < 1)
            {
                throw new ArgumentOutOfRangeException($"Page size can not be '{pageSize}'");
            }

            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM tempLectures 
                                    WHERE topic LIKE '%' || $searchValue || '%'";
            command.Parameters.AddWithValue("$searchValue", searchValue);

            int totalFound = (int)(long)command.ExecuteScalar();

            connection.Close();

            int totalSearchPages = (int)Math.Ceiling((float)totalFound / (float)pageSize);

            return totalSearchPages;
        }


        public List<Lecture> GetSearchPage(string searchValue, int pageNum, int pageSize)
        {
            if (pageNum < 1)
            {
                throw new ArgumentOutOfRangeException($"Page '{pageNum}' out of range");
            }

            if (pageSize < 1)
            {
                throw new ArgumentOutOfRangeException($"Page size can not be '{pageSize}'");
            }

            connection.Open();

            SqliteCommand command = connection.CreateCommand();

            command.CommandText = @"SELECT * FROM tempLectures 
                                    WHERE topic LIKE '%' || $searchValue || '%'
                                    LIMIT $skip,$countOfOut";
            command.Parameters.AddWithValue("$searchValue", searchValue);
            command.Parameters.AddWithValue("$skip", (pageNum - 1) * pageSize);
            command.Parameters.AddWithValue("$countOfOut", pageSize);

            SqliteDataReader reader = command.ExecuteReader();

            List<Lecture> searchPage = ReadLectures(reader);

            reader.Close();

            connection.Close();

            return searchPage;
        }


        private static List<Lecture> ReadLectures(SqliteDataReader reader)
        {
            List<Lecture> lecturesList = new List<Lecture>();

            while (reader.Read())
            {
                Lecture lecture = ReadLecture(reader);

                lecturesList.Add(lecture);
            }

            return lecturesList;
        }

        private static Lecture ReadLecture(SqliteDataReader reader)
        {
            Lecture lecture = new Lecture();

            lecture.id = reader.GetInt32(0);
            lecture.topic = reader.GetString(1);
            lecture.description = reader.GetString(2);
            lecture.duration = reader.GetString(3);
            lecture.createdAt = reader.GetDateTime(4);

            return lecture;
        }
    }
}