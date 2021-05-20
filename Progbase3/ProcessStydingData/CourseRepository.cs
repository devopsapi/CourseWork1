using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System;
namespace ProcessStydingData
{
    public class CourseRepository
    {
        private SqliteConnection connection;
        public CourseRepository(string databasePath)
        {
            string connectionPath = $"Data Source={databasePath}";

            this.connection = new SqliteConnection(connectionPath);
        }

        public int Insert(Course course)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();

            command.CommandText =
            @"
                INSERT INTO courses (title, description, professor, lectures, subscribers,rating,isPrivate,publishedAt ) 
                VALUES ($title, $description, $professor, $lectures, $subscribers,$rating,$isPrivate,$publishedAt);
                SELECT last_insert_rowid();
            ";

            command.Parameters.AddWithValue("$title", course.title);
            command.Parameters.AddWithValue("$description", course.description);
            command.Parameters.AddWithValue("$professor", course.professor);
            command.Parameters.AddWithValue("$lectures", course.lectures);
            command.Parameters.AddWithValue("$subscribers", course.amountOfSubscribers);
            command.Parameters.AddWithValue("$rating", course.rating);
            command.Parameters.AddWithValue("$isPrivate", course.isPrivate ? 1 : 0);
            command.Parameters.AddWithValue("$publishedAt", course.publishedAt);


            int insertedId = (int)(long)command.ExecuteScalar();

            connection.Close();

            return insertedId;
        }


        public bool Update(int courseId, Course course)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText =
            @"
                UPDATE courses SET title = $title,description = $description,
                professor = $professor,lectures = $lectures,
                subscribers = $subscribers,rating = $rating,
                isPrivate  = $isPrivate  WHERE id = $id
            ";
            command.Parameters.AddWithValue("$id", courseId);
            command.Parameters.AddWithValue("$title", course.title);
            command.Parameters.AddWithValue("$description", course.description);
            command.Parameters.AddWithValue("$professor", course.professor);
            command.Parameters.AddWithValue("$lectures", course.lectures);
            command.Parameters.AddWithValue("$subscribers", course.amountOfSubscribers);
            command.Parameters.AddWithValue("$rating", course.rating);
            command.Parameters.AddWithValue("$isPrivate ", course.isPrivate ? 1 : 0);


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

            command.CommandText = @"DELETE FROM courses WHERE id = $id";
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
            command.CommandText = @"SELECT COUNT(*) FROM courses";

            int countOfRows = (int)(long)command.ExecuteScalar();

            int totalPages = (int)System.Math.Ceiling((float)countOfRows / (float)pageSize);

            connection.Close();

            return totalPages;
        }

        public List<Course> GetPage(int pageNum, int pageSize)
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

            command.CommandText = @"SELECT * FROM courses LIMIT $skip,$countOfOut";
            command.Parameters.AddWithValue("$skip", (pageNum - 1) * pageSize);
            command.Parameters.AddWithValue("$countOfOut", pageSize);

            SqliteDataReader reader = command.ExecuteReader();

            List<Course> page = ReadCourses(reader);

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
            command.CommandText = @"SELECT COUNT(*) FROM courses 
                                    WHERE title LIKE '%' || $searchValue || '%'
                                    OR professor LIKE '%' || $searchValue || '%'";
            command.Parameters.AddWithValue("$searchValue", searchValue);

            int totalFound = (int)(long)command.ExecuteScalar();

            connection.Close();

            int totalSearchPages = (int)Math.Ceiling((float)totalFound / (float)pageSize);

            return totalSearchPages;
        }

        public List<Course> GetSearchPage(string searchValue, int pageNum, int pageSize)
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

            command.CommandText = @"SELECT * FROM courses 
                                    WHERE title LIKE '%' || $searchValue || '%'
                                    OR description LIKE '%' || $searchValue || '%'
                                    LIMIT $skip,$countOfOut";
            command.Parameters.AddWithValue("$searchValue", searchValue);
            command.Parameters.AddWithValue("$skip", (pageNum - 1) * pageSize);
            command.Parameters.AddWithValue("$countOfOut", pageSize);

            SqliteDataReader reader = command.ExecuteReader();

            List<Course> searchPage = ReadCourses(reader);

            reader.Close();

            connection.Close();

            return searchPage;
        }



        private static List<Course> ReadCourses(SqliteDataReader reader)
        {
            List<Course> coursesList = new List<Course>();

            while (reader.Read())
            {
                Course course = ReadCourse(reader);

                coursesList.Add(course);
            }

            return coursesList;
        }

        private static Course ReadCourse(SqliteDataReader reader)
        {
            Course course = new Course();
            course.id = reader.GetInt32(0);
            course.title = reader.GetString(1);
            course.description = reader.GetString(2);
            course.professor = reader.GetString(3);
            course.lectures = reader.GetString(4);
            course.amountOfSubscribers = reader.GetInt32(5);
            course.rating = double.Parse(reader.GetString(6));
            course.isPrivate = (reader.GetInt32(7) == 1) ? true : false;

            return course;
        }
    }
}