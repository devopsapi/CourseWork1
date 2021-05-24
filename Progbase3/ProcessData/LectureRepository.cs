using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System;
namespace ProcessData
{
    public class LectureRepository
    {
        private SqliteConnection connection;

        public LectureRepository(string databasePath)
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
                INSERT INTO lectures (topic,course_id)
                VALUES ($topic,$course_id);
                SELECT last_insert_rowid();
            ";

            command.Parameters.AddWithValue("$topic", lecture.topic);
            command.Parameters.AddWithValue("$course_id", lecture.course_id);


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
                UPDATE lectures SET topic = $topic, course_id = $course_id WHERE id = $id
            ";
            command.Parameters.AddWithValue("$id", lectureId);
            command.Parameters.AddWithValue("$topic", lecture.topic);
            command.Parameters.AddWithValue("$course_id", lecture.course_id);

            int nChanged = command.ExecuteNonQuery();

            bool isUpdated = false;

            if (nChanged != 0)
            {
                isUpdated = true;
            }

            connection.Close();

            return isUpdated;
        }


        public bool DeleteById(int id)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();

            command.CommandText = @"DELETE FROM lectures WHERE id = $id";
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



        public int GetTotalPages(int pageSize)
        {
            if (pageSize < 1)
            {
                throw new ArgumentOutOfRangeException($"Page size can not be '{pageSize}'");
            }

            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM lectures";

            int countOfRows = (int)(long)command.ExecuteScalar();

            int totalPages = (int)Math.Ceiling((float)countOfRows / (float)pageSize);

            connection.Close();

            return totalPages;
        }

        public List<Lecture> GetPage(int pageNum, int pageSize)
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

            command.CommandText = @"SELECT * FROM lectures LIMIT $skip,$countOfOut";
            command.Parameters.AddWithValue("$skip", (pageNum - 1) * pageSize);
            command.Parameters.AddWithValue("$countOfOut", pageSize);

            SqliteDataReader reader = command.ExecuteReader();

            List<Lecture> page = ReadLectures(reader);

            reader.Close();

            connection.Close();

            return page;
        }

        public int GetSearchPagesCount(int pageSize, string searchValue)
        {
            if (pageSize < 1)
            {
                throw new ArgumentOutOfRangeException($"Page size can not be '{pageSize}'");
            }

            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM lectures 
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

            command.CommandText = @"SELECT * FROM lectures 
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
            lecture.course_id = reader.GetInt32(2);

            return lecture;
        }


        public Lecture[] GetAllCourseLectures(int course_id)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();

            command.CommandText = @"SELECT * FROM lectures WHERE id = $course_id";
            command.Parameters.AddWithValue("$course_id", course_id);

            SqliteDataReader reader = command.ExecuteReader();

            List<Lecture> lecturesList = ReadLectures(reader);

            Lecture[] allUserLectures = new Lecture[lecturesList.Count];

            lecturesList.CopyTo(allUserLectures);

            reader.Close();

            connection.Close();

            return allUserLectures;
        }


        public int[] GetAllLecturesIds()
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();

            command.CommandText = @"SELECT id FROM lectures";
            SqliteDataReader reader = command.ExecuteReader();

            List<int> idsList = GetListOfIds(reader);

            reader.Close();

            connection.Close();

            int[] ids = new int[idsList.Count];
            idsList.CopyTo(ids);

            return ids;
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