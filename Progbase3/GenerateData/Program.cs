using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using ProcessData;

namespace GenerateData
{
    class Program
    {
        static string[] GetUserEnter()
        {
            Console.Write("Enter command ('help' to help): ");

            string enteredText = Console.ReadLine().Trim();

            string[] command = enteredText.Split();

            if (command[0] == "")
            {
                return null;
            }

            return command;
        }
        static void Main(string[] args)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            string databasePath = "C:/Users/Yuli/Desktop/CourseWork/progbase3/data/database.db";

            UserRepository userRepository = new UserRepository(databasePath);

            CourseRepository courseRepository = new CourseRepository(databasePath);

            LectureRepository lectureRepository = new LectureRepository(databasePath);

            UsersAndCoursesRepository usersAndCoursesRepository = new UsersAndCoursesRepository(databasePath);


            while (true)
            {
                Console.WriteLine("".PadRight(40, '='));

                string[] command = GetUserEnter();

                Console.WriteLine("".PadRight(40, '-'));

                if (command == null || (command[0] == "exit" && command.Length == 1))
                {
                    Console.WriteLine("Program ending");

                    Console.WriteLine("".PadRight(40, '='));

                    break;
                }

                if (command[0] == "exit")
                {
                    Console.Error.WriteLine("Wrong command input. Command must consist of at least 2 elements ('exit' or '' to exit)");
                }
                else
                {
                    try
                    {
                        if (command[0] == "generate")
                        {
                            ProcessGenerate(command, userRepository, courseRepository, lectureRepository, usersAndCoursesRepository);
                        }
                        else
                        {
                            Console.Error.WriteLine($"Input error. Unknown command '{command[0]}'");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"[Error] - {ex}");
                    }
                }
            }
        }
        static void ProcessGenerate(string[] command, UserRepository userRepository, CourseRepository courseRepository, LectureRepository lectureRepository, UsersAndCoursesRepository usersAndCoursesRepository)
        {
            if (command.Length < 4)
            {
                throw new ArgumentException("Command 'generate' must have a form: generate {category} {count of generated} " +
                "{timeIntervals (in format (XX(month)/XX(day)/XXXX(year)) with separator 'To': XX/XX/XXXXToXX/XX/XXXX)}");
            }

            string generateCategory = command[1];

            ValidateGenerateCategory(generateCategory);

            ValidateGeneratedCount(command[2]);

            if (generateCategory == "users")
            {
                GenerateUsers(command, userRepository);
            }
            else if (generateCategory == "courses")
            {
                ValidateUsers(userRepository);

                GenerateCourses(command, userRepository, courseRepository);
            }
            else if (generateCategory == "lectures")
            {
                ValidateUsers(userRepository);
                ValidateCourses(courseRepository);

                GenerateLectures(command, courseRepository, lectureRepository);
            }

            Console.WriteLine($"{command[2]} {generateCategory} successfully generated");
        }

        private static void ValidateCourses(CourseRepository courseRepository)
        {
            Course[] courses = courseRepository.GetAll();

            if (courses.Length == 0)
            {
                throw new ArgumentException("Can not generate lectures. Courses do not exist. Generate courses first");
            }
        }
        private static void ValidateGenerateCategory(string category)
        {
            string[] supportedGenerateCategories = new string[] { "users", "courses", "lectures" };

            for (int i = 0; i < supportedGenerateCategories.Length; i++)
            {
                if (supportedGenerateCategories[i] == category)
                {
                    return;
                }
            }

            throw new ArgumentException($"Unsupported generating category '{category}' ('help' - to see supported categories)");
        }

        private static void ValidateGeneratedCount(string enteredCount)
        {
            int generatedCount;
            bool tryCount = int.TryParse(enteredCount, out generatedCount);
            if (!tryCount || generatedCount <= 0)
            {
                throw new ArgumentException("Incorrect count of generating entities ('help' to help)");
            }
        }
        private static void ValidateUsers(UserRepository userRep)
        {
            User[] users = userRep.GetAllUsers();

            if (users.Length == 0)
            {
                throw new ArgumentException("Can not generate courses and lectures. Users do not exist. Generate users first");
            }
        }

        static void GenerateUsers(string[] command, UserRepository userRepository)
        {
            Authentication authentication = new Authentication(userRepository);

            string fullnamesFile = "C:/Users/Yuli/Desktop/CourseWork/progbase3/data/generator/fullnames.txt";
            string usernamesFile = "C:/Users/Yuli/Desktop/CourseWork/progbase3/data/generator/users.txt";

            int dataAmount;

            if (!int.TryParse(command[2], out dataAmount))
            {
                throw new Exception("Wrong count of generated data");
            }

            string[] fullnames = ReadFullnamesFromFile(fullnamesFile);

            string[] usernames = ReadUsernamesFromFile(usernamesFile);

            bool[] trueOrFalse = new bool[] { true, false };

            DateTime[] datesInterval = ParseDateIntervals(command[3]);

            for (int i = 0; i < dataAmount; i++)
            {
                Random random = new Random();
                User newUser = new User();
                newUser.username = usernames[random.Next(0, usernames.Length)];
                newUser.password = GeneratePassword(32);
                newUser.fullname = fullnames[random.Next(0, fullnamesFile.Length)];
                newUser.createdAt = GenerateRandomDate(datesInterval);
                newUser.imported = trueOrFalse[random.Next(0, trueOrFalse.Length)];
                newUser.isAuthor = trueOrFalse[random.Next(0, trueOrFalse.Length)];

                if (!authentication.Register(newUser))
                {
                    i--;
                    continue;
                }
            }
        }

        private static string[] ReadFullnamesFromFile(string filename)
        {
            List<string> list = new List<string>();

            StreamReader reader = new StreamReader(filename);

            string line = "";

            while ((line = reader.ReadLine()) != null)
            {
                list.Add(line);
            }

            string[] fullnames = new string[list.Count];
            list.CopyTo(fullnames);

            return fullnames;
        }

        private static string[] ReadUsernamesFromFile(string filename)
        {
            List<string> list = new List<string>();

            StreamReader reader = new StreamReader(filename);

            string line = "";

            while ((line = reader.ReadLine()) != null)
            {
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

        private static DateTime[] ParseDateIntervals(string dateIntervals)
        {
            string[] splittedDates = dateIntervals.Split("To");

            DateTime leftLimDate;
            DateTime rightLimDate;

            bool leftDateParsed = DateTime.TryParseExact(splittedDates[0], "d", CultureInfo.InvariantCulture, DateTimeStyles.None, out leftLimDate);
            bool rightLimParsed = DateTime.TryParseExact(splittedDates[1], "d", CultureInfo.InvariantCulture, DateTimeStyles.None, out rightLimDate);

            if (!leftDateParsed || !rightLimParsed)
            {
                throw new ArgumentException("Incorrect date format ('help' to help)");
            }

            int daysInterval = (rightLimDate - leftLimDate).Days;

            if (daysInterval <= 0)
            {
                throw new ArgumentException("Incorrect dates interval ('help' to help)");
            }

            DateTime[] parsedDates = new DateTime[] { leftLimDate, rightLimDate };

            return parsedDates;
        }

        private static DateTime GenerateRandomDate(DateTime[] datesInterval)
        {
            DateTime leftLimDate = datesInterval[0];
            DateTime rightLimDate = datesInterval[1];

            Random random = new Random();

            int daysDiff = (rightLimDate - leftLimDate).Days;

            DateTime generatedDateTime = leftLimDate.AddDays(random.Next(0, daysDiff + 1)).AddHours(random.Next(0, 24)).AddMinutes(random.Next(0, 60)).AddSeconds(random.Next(0, 60)).AddMilliseconds(random.Next(0, 1000));

            return generatedDateTime;
        }

        private static void GenerateCourses(string[] command, UserRepository userRepository, CourseRepository courseRepository)
        {
            int dataAmount;

            if (!int.TryParse(command[2], out dataAmount))
            {
                throw new Exception("Wrong count of generated data");
            }

            string coursesCsvFile = "C:/Users/Yuli/Desktop/CourseWork/progbase3/data/generator/Courses.csv";

            int[] ids = userRepository.GetAllUsersIds();

            Course[] courses = ReadCoursesFromCsv(coursesCsvFile, ids);

            for (int i = 0; i < dataAmount; i++)
            {
                courseRepository.Insert(courses[i]);
            }
        }

        private static Course[] ReadCoursesFromCsv(string filename, int[] ids)
        {
            List<Course> list = new List<Course>();

            StreamReader reader = new StreamReader(filename);

            string line = "";

            while ((line = reader.ReadLine()) != null)
            {
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
                    course.userId = ids[random.Next(0, ids.Length)];

                    list.Add(course);
                }
            }

            Course[] newCourses = new Course[list.Count];

            list.CopyTo(newCourses);

            return newCourses;
        }

        private static void GenerateLectures(string[] command, CourseRepository courseRepository, LectureRepository lectureRepository)
        {
            int dataAmount;

            if (!int.TryParse(command[2], out dataAmount))
            {
                throw new Exception("Wrong count of generated data");
            }

            string lecturesFile = "C:/Users/Yuli/Desktop/CourseWork/progbase3/data/generator/Lectures.csv";

            int[] ids = courseRepository.GetAllCoursesIds();

            Lecture[] lectures = ReadLecturesFromFile(lecturesFile, ids);

            for (int i = 0; i < dataAmount; i++)
            {
                lectureRepository.Insert(lectures[i]);
            }

        }

        private static Lecture[] ReadLecturesFromFile(string filename, int[] ids)
        {
            List<Lecture> list = new List<Lecture>();

            StreamReader reader = new StreamReader(filename);

            string line = "";

            while ((line = reader.ReadLine()) != null)
            {
                string[] values = line.Split(",");

                Random random = new Random();

                if (values[0] != "topic")
                {
                    Lecture lecture = new Lecture();

                    lecture.topic = values[0];
                    lecture.description = values[1];
                    lecture.duration = TimeSpan.FromSeconds(double.Parse(values[2])).ToString();
                    lecture.courseId = ids[random.Next(0, ids.Length)];

                    list.Add(lecture);
                }
            }

            Lecture[] lectures = new Lecture[list.Count];
            list.CopyTo(lectures);

            return lectures;
        }
    }
}
