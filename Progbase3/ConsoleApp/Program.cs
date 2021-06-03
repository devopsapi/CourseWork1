using System;
using System.Collections.Generic;
using ProcessData;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using GenerateData;

namespace ConsoleApp
{
    class Program
    {
        static CourseRepository repoC;
        static LectureRepository repoL;
        static UserRepository repoU;
        static UsersAndCoursesRepository repoUC;
        static void Main(string[] args)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");



            /*  string databasePath = "C:/Users/Yuli/Desktop/CourseWork/progbase3/data/database.db";
             repoC = new CourseRepository(databasePath);
             repoL = new LectureRepository(databasePath);
             repoU = new UserRepository(databasePath);
             repoUC = new UsersAndCoursesRepository(databasePath); */

            //            Console.WriteLine(repoC.CheckIfUserIsCourseAuthor(182, 73));



            //   Console.WriteLine(repoC.GetAllUserCourses(repoUC.GetAllUserCoursesId(181)).Length);



            /* 
                        Course course = new Course();
                        course.title = "new";
                        course.description = "new";
                        course.author = "new";
                        course.amountOfSubscribers = 10;
                        course.price = 1;
                        course.rating = 4.0;
                        course.isPrivate = false;
                        course.publishedAt = DateTime.Now;

                        repoC.Update(21, course);
             */

            //   Console.WriteLine(repoUC.isExists(181,21));





            /*  Lecture[] alll = repoL.GetAllCourseLectures(21);

             for (int i = 0; i < alll.Length; i++)
             {
                 Console.WriteLine(alll[i].topic);
             }
  */
            /*  List<Course> allcourse = repoC.GetAll();
             foreach (Course c in allcourse)
             {
                 Console.WriteLine(c.publishedAt);
             } */

            /*  Course course = new Course();
             course.title = "new";
             course.description = "new";
             course.professor = "one";
             course.lectures = "two";
             course.amountOfSubscribers = 3;
             course.rating = 4.0;
             course.isPrivate = true;

             repo = new CourseRepository(databasePath);
             course.id = repo.Insert(course);
             repo.DeleteById(3);
             Course course1 = new Course();
             course1.title = "old";
             course1.description = "old";
             course1.professor = "kek";
             course1.lectures = "lol";
             course1.amountOfSubscribers = 0;
             course1.rating = 10.0;
             course1.isPrivate = true;

             repo.Update(5, course1);
             Console.WriteLine(repo.GetTotalPages(10)); */

            // GenerateCourses(10);
            //   GenerateLectures(10);
            //  GenerateUsers(10);


        }

        static void GenerateCourses(int dataAmount)
        {
            string coursesCsvFile = "C:/Users/Yuli/Desktop/CourseWork/progbase3/data/Courses.csv";
            List<Course> generatedCourses = ReadCoursesFromCsv(coursesCsvFile, dataAmount);
            foreach (Course c in generatedCourses)
            {
                c.id = repoC.Insert(c);
            }
        }

        static List<Course> ReadCoursesFromCsv(string coursesCsvFile, int dataAmount)
        {
            int count = 0;
            List<Course> courses = new List<Course>();

            int[] ids = repoU.GetAllUsersIds();

            StreamReader reader = new StreamReader(coursesCsvFile);
            string line = "";
            while ((line = reader.ReadLine()) != null)
            {
                if (count == dataAmount)
                {
                    break;
                }

                string[] values = line.Split(",");

                if (values[0] != "Title")
                {
                    Random random = new Random();
                    Course course = new Course();

                    course.title = values[0];
                    course.description = $"Learn '{course.title}' with us!";
                    course.amountOfSubscribers = int.Parse(values[1]);
                    course.rating = double.Parse(values[2]);
                    course.author = values[3];
                    course.price = double.Parse(values[4]);
                    course.publishedAt = DateTime.Parse(values[5]);
                    course.isPrivate = bool.Parse(values[6]);

                    if (ids == null)
                    {
                        course.userId = random.Next(1, dataAmount + 1);
                    }
                    else
                    {
                        course.userId = ids[random.Next(0, ids.Length)];
                    }

                    courses.Add(course);
                    count++;
                }
            }
            return courses;
        }


        static void GenerateLectures(int dataAmount)
        {
            string lecturesCsvFile = "C:/Users/Yuli/Desktop/CourseWork/progbase3/data/Lectures.csv";
            List<Lecture> generatedLectures = ReadLecturesFromCsv(lecturesCsvFile, dataAmount);
            foreach (Lecture c in generatedLectures)
            {
                c.id = repoL.Insert(c);
            }
        }

        static List<Lecture> ReadLecturesFromCsv(string lecturesCsvFile, int dataAmount)
        {
            int count = 0;
            List<Lecture> lectures = new List<Lecture>();
            StreamReader reader = new StreamReader(lecturesCsvFile);
            string line = "";
            while ((line = reader.ReadLine()) != null)
            {
                if (count == dataAmount)
                {
                    break;
                }
                string[] values = line.Split(",");

                Random random = new Random();

                if (values[0] != "topic")
                {
                    Lecture lecture = new Lecture();
                    lecture.topic = values[0];
                    lecture.description = values[1];
                    lecture.duration = TimeSpan.FromSeconds(double.Parse(values[2])).ToString();
                    int[] ids = repoC.GetAllCoursesIds();
                    if (ids.Length == 0)
                    {
                        lecture.courseId = random.Next(0, dataAmount + 1);
                    }
                    else
                    {
                        lecture.courseId = ids[random.Next(0, ids.Length)];
                    }


                    lectures.Add(lecture);
                    count++;
                }
            }
            return lectures;
        }


        static void GenerateUsers(int dataAmount)
        {
            string fullnamesFile = "C:/Users/Yuli/Desktop/CourseWork/progbase3/data/fullnames.txt";
            List<User> generatedUsers = ReadUsers(fullnamesFile, dataAmount);
            foreach (User u in generatedUsers)
            {
                u.id = repoU.Insert(u);
            }
        }
        static List<User> ReadUsers(string fullnamesFile, int dataAmount)
        {
            List<User> generatedUsers = new List<User>();
            StreamReader reader = new StreamReader(fullnamesFile);
           
            string[] usernames = ReadAllUsersFromTxt("C:/Users/Yuli/Desktop/CourseWork/progbase3/data/users.txt", dataAmount);
            int count = 0;
            string line = "";
            while ((line = reader.ReadLine()) != null)
            {
                if (count == dataAmount)
                {
                    break;
                }

                Random random = new Random();
                User user = new User();
                user.username = usernames[count];
                user.password = GeneratePassword(32);
                user.fullname = line;
                user.createdAt = RandomDay();
                user.imported = false;
              

                generatedUsers.Add(user);
                count++;
            }

            return generatedUsers;
        }

        static string[] ReadAllUsersFromTxt(string filename, int dataAmount)
        {
            StreamReader reader = new StreamReader(filename);
            List<string> list = new List<string>();
            int count = 0;
            string line = "";

            while ((line = reader.ReadLine()) != null)
            {
                if (count == dataAmount)
                {
                    break;
                }

                list.Add(line);
            }

            string[] usernames = new string[list.Count];
            list.CopyTo(usernames);
            return usernames;
        }


        public static string GeneratePassword(int length)
        {
            try
            {
                byte[] result = new byte[length];
                for (int index = 0; index < length; index++)
                {
                    result[index] = (byte)new Random().Next(48, 127);
                }
                return System.Text.Encoding.ASCII.GetString(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }


        private static DateTime RandomDay()
        {
            Random gen = new Random();
            DateTime start = new DateTime(2000, 1, 1);
            int range = (DateTime.Today - start).Days;
            return start.AddDays(gen.Next(range));
        }
    }
}