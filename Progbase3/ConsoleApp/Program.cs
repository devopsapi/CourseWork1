using System;
using System.Collections.Generic;
using ProcessStydingData;
using System.IO;
namespace ConsoleApp
{
    class Program
    {
        static CourseRepository repoC;
        static LectureRepository repoL;
        static void Main(string[] args)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            string databasePath = "C:/Users/Yuli/Desktop/CourseWork/progbase3/data/database.db";
            repoC = new CourseRepository(databasePath);
            repoL = new LectureRepository(databasePath);
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

            //  GenerateCourses(2000);
            GenerateLectures(100);

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
                    Course course = new Course();
                    course.title = values[0];
                    course.description = $"Learn '{course.title}' with us!";
                    Console.WriteLine(values[1]);
                    course.amountOfSubscribers = int.Parse(values[1]);
                    course.rating = double.Parse(values[2]);
                    course.professor = values[3];
                    course.price = double.Parse(values[4]);
                    course.lectures = "empty";
                    course.publishedAt = DateTime.Parse(values[5]);
                    course.isPrivate = bool.Parse(values[6]);

                    courses.Add(course);
                    count++;
                }


            }
            return courses;
        }


        static void GenerateLectures(int dataAmount)
        {
            string lecturesCsvFile = "C:/Users/Yuli/Desktop/CourseWork/progbase3/data/Lectures.txt";
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

                Lecture lecture = new Lecture();
                lecture.topic = values[0];
                lectures.Add(lecture);
                count++;
            }
            return lectures;
        }

        /* static List<Lecture> GenerateRandomLectures(int dataAmount)
        {
            List<Course> randomCourses = new List<Course>();
            List<Lecture> randomLectures = new List<Lecture>();
        } */
    }
}