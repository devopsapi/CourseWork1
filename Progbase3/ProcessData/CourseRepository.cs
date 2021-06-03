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
                INSERT INTO courses (title, description, author, subscribers,price, rating, isPrivate,publishedAt, user_id) 
                VALUES ($title, $description, $author,$subscribers,$price,$rating,$isPrivate,$publishedAt,$user_id);
                SELECT last_insert_rowid();
            ";

            command.Parameters.AddWithValue("$title", course.title);
            command.Parameters.AddWithValue("$description", course.description);
            command.Parameters.AddWithValue("$author", course.author);
            command.Parameters.AddWithValue("$subscribers", course.amountOfSubscribers);
            command.Parameters.AddWithValue("$price", course.price);
            command.Parameters.AddWithValue("$rating", course.rating);
            command.Parameters.AddWithValue("$isPrivate", course.isPrivate ? 1 : 0);
            command.Parameters.AddWithValue("$publishedAt", course.publishedAt);
            command.Parameters.AddWithValue("$user_id", course.userId);


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
                author = $author,subscribers = $subscribers,price = $price, 
                rating = $rating, isPrivate = $isPrivate, publishedAt = $publishedAt,user_id = $user_id 
                WHERE id = $id
            ";
            command.Parameters.AddWithValue("$id", courseId);
            command.Parameters.AddWithValue("$title", course.title);
            command.Parameters.AddWithValue("$description", course.description);
            command.Parameters.AddWithValue("$author", course.author);
            command.Parameters.AddWithValue("$subscribers", course.amountOfSubscribers);
            command.Parameters.AddWithValue("$price", course.price);
            command.Parameters.AddWithValue("$rating", course.rating);
            command.Parameters.AddWithValue("$isPrivate", course.isPrivate ? 1 : 0);
            command.Parameters.AddWithValue("$publishedAt", course.publishedAt);
            command.Parameters.AddWithValue("$user_id", course.userId);


            int nChanged = command.ExecuteNonQuery();

            bool isUpdated = false;

            if (nChanged != 0)
            {
                isUpdated = true;
            }

            connection.Close();

            return isUpdated;
        }


        public bool DeleteById(int courseId)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();

            command.CommandText = @"DELETE FROM courses WHERE id = $id";
            command.Parameters.AddWithValue("$id", courseId);

            int deletedCount = command.ExecuteNonQuery();

            bool isDeleted = false;

            if (deletedCount != 0)
            {
                isDeleted = true;
            }

            connection.Close();

            return isDeleted;
        }


        public int InsertImport(Course course)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();

            command.CommandText =
            @"
                INSERT INTO courses (id, title, description,author,subscribers,price,rating,isPrivate,publishedAt, user_id) 
                VALUES ($id, $title, $description,$author,$subscribers,$price,$rating,$isPrivate,$publishedAt,$userId);

                SELECT last_insert_rowid();
            ";

            command.Parameters.AddWithValue("$id", course.id);
            command.Parameters.AddWithValue("$title", course.title);
            command.Parameters.AddWithValue("$description", course.description);
            command.Parameters.AddWithValue("$author", course.author);
            command.Parameters.AddWithValue("$subscribers", course.amountOfSubscribers);
            command.Parameters.AddWithValue("$price", course.price);
            command.Parameters.AddWithValue("$rating", course.rating);
            command.Parameters.AddWithValue("$isPrivate", course.isPrivate.ToString());
            command.Parameters.AddWithValue("$publishedAt", course.publishedAt.ToString("o"));
            command.Parameters.AddWithValue("$userId", course.userId);

            int insertedId = (int)(long)command.ExecuteScalar();

            connection.Close();

            return insertedId;
        }


        public Course[] GetAll()
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM courses";
            SqliteDataReader reader = command.ExecuteReader();

            List<Course> list = ReadCourses(reader);

            reader.Close();
            connection.Close();

            Course[] courses = new Course[list.Count];
            list.CopyTo(courses);
            
            return courses;
        }


        public bool CourseExist(int courseId)
        {

            connection.Open();

            SqliteCommand command = connection.CreateCommand();

            command.CommandText = @"SELECT COUNT(*) FROM courses WHERE id = $courseId";
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



        public int GetSearchPagesCountByUserId(int pageSize, string searchValue, int userId)
        {
            if (pageSize < 1)
            {
                throw new ArgumentOutOfRangeException($"Page size can not be '{pageSize}'");
            }

            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM courses 
                                    WHERE user_id = $userId AND title LIKE '%' || $searchValue || '%'";
            command.Parameters.AddWithValue("$userId", userId);
            command.Parameters.AddWithValue("$searchValue", searchValue);

            int totalFound = (int)(long)command.ExecuteScalar();

            connection.Close();

            int totalSearchPages = (int)Math.Ceiling((float)totalFound / (float)pageSize);

            return totalSearchPages;
        }


        public List<Course> GetSearchPageByUserId(string searchValue, int pageNum, int pageSize, int userId)
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
                                    WHERE user_id = $userId AND title LIKE '%' || $searchValue || '%'
                                    LIMIT $skip,$countOfOut";
            command.Parameters.AddWithValue("$userId", userId);
            command.Parameters.AddWithValue("$searchValue", searchValue);
            command.Parameters.AddWithValue("$skip", (pageNum - 1) * pageSize);
            command.Parameters.AddWithValue("$countOfOut", pageSize);

            SqliteDataReader reader = command.ExecuteReader();

            List<Course> searchPage = ReadCourses(reader);

            reader.Close();

            connection.Close();

            return searchPage;
        }


        public int GetSearchPagesCountOfSubscibedCourses(List<int> coursesId, int pageSize, string searchValue)
        {
            if (pageSize < 1)
            {
                throw new ArgumentOutOfRangeException($"Page size can not be '{pageSize}'");
            }

            int totalFound = 0;

            foreach (int courseId in coursesId)
            {
                connection.Open();
                SqliteCommand command = connection.CreateCommand();

                command.CommandText = @"SELECT COUNT(*) FROM courses 
                                    WHERE id = $courseId AND title LIKE '%' || $searchValue || '%'";
                command.Parameters.AddWithValue("$courseId", courseId);
                command.Parameters.AddWithValue("$searchValue", searchValue);

                int count = (int)(long)command.ExecuteScalar();

                totalFound += count;

                connection.Close();
            }

            int totalSearchPages = (int)Math.Ceiling((float)totalFound / (float)pageSize);

            return totalSearchPages;
        }


        public List<Course> GetSearchPageOfSubscribedCourses(List<int> coursesId, string searchValue, int pageNum, int pageSize)
        {
            if (pageNum < 1)
            {
                throw new ArgumentOutOfRangeException($"Page '{pageNum}' out of range");
            }

            if (pageSize < 1)
            {
                throw new ArgumentOutOfRangeException($"Page size can not be '{pageSize}'");
            }

            List<Course> searchPage = new List<Course>();

            foreach (int id in coursesId)
            {
                connection.Open();
                SqliteCommand command = connection.CreateCommand();

                command.CommandText = @"SELECT * FROM courses WHERE id = $id AND title LIKE '%' || $searchValue || '%' LIMIT $skip,$countOfOut";
                command.Parameters.AddWithValue("$id", id);
                command.Parameters.AddWithValue("$searchValue", searchValue);
                command.Parameters.AddWithValue("$skip", (pageNum - 1) * pageSize);
                command.Parameters.AddWithValue("$countOfOut", pageSize);

                SqliteDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    Course course = ReadCourse(reader);
                    searchPage.Add(course);
                }

                reader.Close();

                connection.Close();
            }

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

            /* if (reader.Read())
            { */
            course.id = reader.GetInt32(0);
            course.title = reader.GetString(1);
            course.description = reader.GetString(2);
            course.author = reader.GetString(3);
            course.amountOfSubscribers = reader.GetInt32(4);
            course.price = reader.GetDouble(5);
            course.rating = reader.GetDouble(6);
            course.isPrivate = reader.GetBoolean(7);
            course.publishedAt = reader.GetDateTime(8);
            course.userId = reader.GetInt32(9);
            //}

            return course;
        }

        public bool CheckIfUserIsCourseAuthor(int userId, int courseId)
        {
            Course course = new Course();

            connection.Open();

            SqliteCommand command = connection.CreateCommand();

            command.CommandText = @"SELECT COUNT(*) FROM courses WHERE id = $id AND user_id = $userId";
            command.Parameters.AddWithValue("$id", courseId);
            command.Parameters.AddWithValue("$userId", userId);

            int nCount = (int)(long)command.ExecuteScalar();

            bool isAuthor = false;

            if (nCount != 0)
            {
                isAuthor = true;
            }

            connection.Close();

            return isAuthor;
        }


        public Course[] GetAllUserCourses(List<int> coursesId)
        {
            List<Course> list = new List<Course>();

            foreach (int id in coursesId)
            {
                connection.Open();
                SqliteCommand command = connection.CreateCommand();

                command.CommandText = @"SELECT * FROM courses WHERE id = $id";
                command.Parameters.AddWithValue("$id", id);

                SqliteDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    Course course = ReadCourse(reader);
                    list.Add(course);
                }

                reader.Close();

                connection.Close();
            }

            Course[] courses = new Course[list.Count];
            list.CopyTo(courses);

            return courses;
        }


        public Course[] GetAllAuthorCourses(int userId)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();

            command.CommandText = @"SELECT * FROM courses WHERE user_id = $userId";
            command.Parameters.AddWithValue("$userId", userId);

            SqliteDataReader reader = command.ExecuteReader();

            List<Course> coursesList = ReadCourses(reader);

            Course[] allAuthorCourses = new Course[coursesList.Count];

            coursesList.CopyTo(allAuthorCourses);

            reader.Close();

            connection.Close();

            return allAuthorCourses;
        }


        public Course GetCourseWithAllLectures(int courseId)
        {

            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM courses, lectures WHERE courses.id = $course_id AND lectures.course_id = $course_id";
            command.Parameters.AddWithValue("$course_id", courseId);

            SqliteDataReader reader = command.ExecuteReader();

            Course course = new Course();

            Lecture[] lectures = null;

            ReadFromCrossJoin(reader, course, ref lectures);

            course.lectures = lectures;

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
                course.price = reader.GetDouble(5);
                course.rating = double.Parse(reader.GetString(6));
                course.isPrivate = reader.GetBoolean(7);
                course.publishedAt = DateTime.Parse(reader.GetString(8));
                course.userId = reader.GetInt32(9);

                Lecture lecture = new Lecture();

                lecture.id = reader.GetInt32(10);
                lecture.topic = reader.GetString(11);
                lecture.description = reader.GetString(12);
                lecture.duration = reader.GetString(13);
                lecture.courseId = reader.GetInt32(14);

                list.Add(lecture);
            }

            lectures = new Lecture[list.Count];
            list.CopyTo(lectures);
        }
    }
}