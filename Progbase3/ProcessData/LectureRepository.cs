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
                INSERT INTO lectures (topic,description,duration,createdAt,course_id)
                VALUES ($topic,$description,$duration,$createdAt,$course_id);
                SELECT last_insert_rowid();
            ";

            command.Parameters.AddWithValue("$topic", lecture.topic);
            command.Parameters.AddWithValue("$description", lecture.description);
            command.Parameters.AddWithValue("$duration", lecture.duration);
            command.Parameters.AddWithValue("$createdAt", lecture.createdAt);
            command.Parameters.AddWithValue("$course_id", lecture.courseId);


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
                UPDATE lectures SET topic = $topic, description = $description,
                duration = $duration, course_id = $course_id WHERE id = $id
            ";
            command.Parameters.AddWithValue("$id", lectureId);
            command.Parameters.AddWithValue("$topic", lecture.topic);
            command.Parameters.AddWithValue("$description", lecture.description);
            command.Parameters.AddWithValue("$duration", lecture.duration);
            command.Parameters.AddWithValue("$course_id", lecture.courseId);

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


        public int InsertImport(Lecture lecture)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();

            command.CommandText =
            @"
                INSERT INTO lectures (id,topic,description,duration,createdAt,course_id,) VALUES ($id, $topic,$description,$duration,$createdAt,$course_id);
                SELECT last_insert_rowid();
            ";

            command.Parameters.AddWithValue("$id", lecture.id);
            command.Parameters.AddWithValue("$topic", lecture.topic);
            command.Parameters.AddWithValue("$description", lecture.description);
            command.Parameters.AddWithValue("$duration", lecture.duration);
            command.Parameters.AddWithValue("$createdAt", lecture.createdAt);
            command.Parameters.AddWithValue("$course_id", lecture.courseId);

            int insertedId = (int)(long)command.ExecuteScalar();

            connection.Close();

            return insertedId;
        }


        public bool LectureExists(int lectureId)
        {
            Lecture lecture = new Lecture();

            connection.Open();

            SqliteCommand command = connection.CreateCommand();

            command.CommandText = @"SELECT COUNT(*) FROM lectures WHERE id = $lectureId";
            command.Parameters.AddWithValue("$lectureId", lectureId);

            int countOfFound = (int)(long)command.ExecuteScalar();

            bool isExists = false;

            if (countOfFound != 0)
            {
                isExists = true;
            }

            connection.Close();

            return isExists;
        }


        public Lecture[] GetAll()
        {
            List<Lecture> list = new List<Lecture>();
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM lectures";
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


        public List<Lecture> GetPage(int pageNum, int pageSize, int courseId)
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

            command.CommandText = @"SELECT * FROM lectures WHERE course_id = $course_id LIMIT $skip,$countOfOut";
            command.Parameters.AddWithValue("$course_id", courseId);
            command.Parameters.AddWithValue("$skip", (pageNum - 1) * pageSize);
            command.Parameters.AddWithValue("$countOfOut", pageSize);

            SqliteDataReader reader = command.ExecuteReader();

            List<Lecture> page = ReadLectures(reader);

            reader.Close();

            connection.Close();

            return page;
        }


        public int GetSearchPagesCount(int pageSize, string searchValue, int courseId)
        {
            if (pageSize < 1)
            {
                throw new ArgumentOutOfRangeException($"Page size can not be '{pageSize}'");
            }

            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM lectures 
                                    WHERE course_id = $courseId AND topic LIKE '%' || $searchValue || '%'";
            command.Parameters.AddWithValue("$courseId", courseId);
            command.Parameters.AddWithValue("$searchValue", searchValue);

            int totalFound = (int)(long)command.ExecuteScalar();

            connection.Close();

            int totalSearchPages = (int)Math.Ceiling((float)totalFound / (float)pageSize);

            return totalSearchPages;
        }


        public List<Lecture> GetSearchPage(string searchValue, int pageNum, int pageSize, int courseId)
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
                                    WHERE course_id = $courseId AND topic LIKE '%' || $searchValue || '%'
                                    LIMIT $skip,$countOfOut";
            command.Parameters.AddWithValue("$courseId", courseId);
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
            lecture.courseId = reader.GetInt32(5);

            return lecture;
        }


        public Lecture[] GetAllCourseLectures(int course_id)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();

            command.CommandText = @"SELECT * FROM lectures WHERE course_id = $course_id";
            command.Parameters.AddWithValue("$course_id", course_id);

            SqliteDataReader reader = command.ExecuteReader();

            List<Lecture> lecturesList = ReadLectures(reader);

            Lecture[] allUserLectures = new Lecture[lecturesList.Count];

            lecturesList.CopyTo(allUserLectures);

            reader.Close();

            connection.Close();

            return allUserLectures;
        }


      /*   public int[] GetAllLecturesIds()
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
        } */
    }
}