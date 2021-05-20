using System;
using System.Collections.Generic;
using ProcessStydingData;
using System.IO;
namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string databasePath = "C:/Users/Yuli/Desktop/CourseWork/progbase3/data/database.db";
            Course course = new Course();
            course.title = "new";
            course.description = "new";
            course.professor = "one";
            course.lectures = "two";
            course.amountOfSubscribers = 3;
            course.rating = 4.0;
            course.isPrivate = true;

            CourseRepository repo = new CourseRepository(databasePath);
            course.id = repo.Insert(course);
            //repo.DeleteById(3);
            Course course1 = new Course();
            course1.title = "old";
            course1.description = "old";
            course1.professor = "kek";
            course1.lectures = "lol";
            course1.amountOfSubscribers = 0;
            course1.rating = 10.0;
            course1.isPrivate = true;

            repo.Update(5, course1);
            Console.WriteLine(repo.GetTotalPages(10));
        }

        static List<Course> GenerateRandomCourses(int dataAmount)
        {
            string coursesCsvFile = "C:/Users/Yuli/Desktop/CourseWork/progbase3/data";
            List<Course> randomCourses = new List<Course>();



        }

        static void ReadCoursesFromCsv(string coursesCsvFile)
        {
            List<Course> courses = new List<Course>();
            StreamReader reader = new StreamReader(coursesCsvFile);
            string line = "";
            while ((line = reader.ReadLine()) != null)
            {
                string[] values = line.Split(",");
                Course course = new Course();
                course.title = values[1];
                course.description = $"Learn '{course.title}' with us!";
                course.professor = 


            }
        }

        static List<Lecture> GenerateRandomLectures(int dataAmount)
        {
            List<Course> randomCourses = new List<Course>();
            List<Lecture> randomLectures = new List<Lecture>();
        }
    }
}