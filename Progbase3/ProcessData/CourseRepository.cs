using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System;
namespace ProcessData
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
                INSERT INTO courses (title, description, author, subscribers, rating, isPrivate,publishedAt, user_id) 
                VALUES ($title, $description, $author,$subscribers,$rating,$isPrivate,$publishedAt,$user_id);
                SELECT last_insert_rowid();
            ";

            command.Parameters.AddWithValue("$title", course.title);
            command.Parameters.AddWithValue("$description", course.description);
            command.Parameters.AddWithValue("$author", course.author);
            command.Parameters.AddWithValue("$subscribers", course.amountOfSubscribers);
            command.Parameters.AddWithValue("$rating", course.rating);
            command.Parameters.AddWithValue("$isPrivate", course.isPrivate ? 1 : 0);
            command.Parameters.AddWithValue("$publishedAt", course.publishedAt);
            command.Parameters.AddWithValue("$user_id", course.user_id);


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
                author = $author,subscribers = $subscribers,
                rating = $rating,isPrivate = $isPrivate, 
                user_id = $user_id WHERE id = $id
            ";
            command.Parameters.AddWithValue("$id", courseId);
            command.Parameters.AddWithValue("$title", course.title);
            command.Parameters.AddWithValue("$description", course.description);
            command.Parameters.AddWithValue("$author", course.author);
            command.Parameters.AddWithValue("$subscribers", course.amountOfSubscribers);
            command.Parameters.AddWithValue("$rating", course.rating);
            command.Parameters.AddWithValue("$isPrivate ", course.isPrivate ? 1 : 0);
            command.Parameters.AddWithValue("$user_id", course.user_id);


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
                                    OR author LIKE '%' || $searchValue || '%'";
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
                                    OR author LIKE '%' || $searchValue || '%'
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


        public int[] GetAllCoursesIds()
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();

            command.CommandText = @"SELECT id FROM courses";
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


        private static Course ReadCourse(SqliteDataReader reader)
        {
            Course course = new Course();
            if (reader.Read())
            {
                course.id = reader.GetInt32(0);
                course.title = reader.GetString(1);
                course.description = reader.GetString(2);
                course.author = reader.GetString(3);
                course.amountOfSubscribers = reader.GetInt32(4);
                course.rating = double.Parse(reader.GetString(5));
                course.isPrivate = reader.GetBoolean(6);
                course.publishedAt = DateTime.Parse(reader.GetString(7));
                course.user_id = reader.GetInt32(8);
            }

            return course;
        }


        public Course[] GetAllUserCourses(int user_id)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();

            command.CommandText = @"SELECT * FROM courses WHERE id = $user_id";
            command.Parameters.AddWithValue("$user_id", user_id);

            SqliteDataReader reader = command.ExecuteReader();

            List<Course> coursesList = ReadCourses(reader);

            Course[] allUserCourses = new Course[coursesList.Count];

            coursesList.CopyTo(allUserCourses);

            reader.Close();

            connection.Close();

            return allUserCourses;
        }

        public Course[] GetAllAuthorCourses(string author)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();

            command.CommandText = @"SELECT * FROM courses WHERE author = $author";
            command.Parameters.AddWithValue("$author", author);

            SqliteDataReader reader = command.ExecuteReader();

            List<Course> coursesList = ReadCourses(reader);

            Course[] allAuthorCourses = new Course[coursesList.Count];

            coursesList.CopyTo(allAuthorCourses);

            reader.Close();

            connection.Close();

            return allAuthorCourses;
        }


        public Course GetAllCourseLectures(int course_id)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM courses, lectures WHERE courses.id = $course_id AND lectures.course_id = $course_id";
            command.Parameters.AddWithValue("$course_id", course_id);

            SqliteDataReader reader = command.ExecuteReader();

            Course course = new Course();

            Lecture[] lectures = null;

            ReadFromCrossJoin(reader, course, ref lectures);

            course.lectures = lectures;
        //    Console.WriteLine("Amount of lectures " + lectures.Length);
/* 
            Console.WriteLine("Course information");

            Console.WriteLine(course.id);
            Console.WriteLine(course.title);
            Console.WriteLine(course.description);
 */
            reader.Close();
            connection.Close();

            return course;
        }


        private void ReadFromCrossJoin(SqliteDataReader reader, Course course, ref Lecture[] lectures)
        {
            List<Lecture> list = new List<Lecture>();

            while (reader.Read())
            {
                course.id = reader.GetInt32(0);
                course.title = reader.GetString(1);
                course.description = reader.GetString(2);
                course.author = reader.GetString(3);
                course.amountOfSubscribers = reader.GetInt32(4);
                course.rating = double.Parse(reader.GetString(5));
                course.isPrivate = reader.GetBoolean(6);
                course.publishedAt = DateTime.Parse(reader.GetString(7));
                course.user_id = reader.GetInt32(8);

                Lecture lecture = new Lecture();

                lecture.id = reader.GetInt32(9);
                lecture.topic = reader.GetString(10);
                lecture.course_id = reader.GetInt32(11);

                list.Add(lecture);
            }

            lectures = new Lecture[list.Count];
            list.CopyTo(lectures);
        }
        private Lecture[] ReadLecturesFromCrossJoin(SqliteDataReader reader)
        {
            List<Lecture> list = new List<Lecture>();
            Lecture[] lectures;

            while (reader.Read())
            {
                Lecture lecture = new Lecture();

                lecture.id = reader.GetInt32(9);
                lecture.topic = reader.GetString(10);
                lecture.course_id = reader.GetInt32(11);

                list.Add(lecture);
                //    Console.WriteLine(lecture.id);
            }

            //      Console.WriteLine("Inside list lectures " + list.Count);

            lectures = new Lecture[list.Count];
            list.CopyTo(lectures);

            return lectures;
        }
    }
}