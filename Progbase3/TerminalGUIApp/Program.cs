using System;
using Terminal.Gui;
using System.Collections.Generic;
using ProcessData;
namespace TerminalGUIApp
{
    class Program
    {

        static void Main(string[] args)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            string databasePath = "C:/Users/Yuli/Desktop/CourseWork/progbase3/data/database.db";
            CourseRepository courseRepository = new CourseRepository(databasePath);
            LectureRepository lectureRepository = new LectureRepository(databasePath);
            UserRepository userRepository = new UserRepository(databasePath);
            UsersAndCoursesRepository usersAndCoursesRepository = new UsersAndCoursesRepository(databasePath);

            Application.Init();
            Toplevel top = Application.Top;

            MainWindow win = new MainWindow();
            win.SetRepositories(userRepository, courseRepository, lectureRepository, usersAndCoursesRepository);
            top.Add(win);

            Application.Run();

        }
    }
}
