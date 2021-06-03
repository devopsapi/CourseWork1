﻿using System;
using Terminal.Gui;
using System.Collections.Generic;
using ProcessData;
using TerminalGUIApp.Windows;

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
            TemporaryLectureRepository temporaryLectureRepository = new TemporaryLectureRepository(databasePath);

            User user = new User();
            user = new User();
            user.id = 182;
            user.username = "hello";
            user.fullname = "Max Litva";
            user.password = "1231231";
            user.createdAt = DateTime.Now;
            user.imported = false;


            Application.Init();
            Toplevel top = Application.Top;

            MainWindow window = new MainWindow();
            window.SetRepositories(userRepository, courseRepository, lectureRepository, usersAndCoursesRepository, temporaryLectureRepository);

            /* UserMainWindow window = new UserMainWindow();
            window.SetUser(user);

            window.SetRepositories(userRepository, courseRepository, lectureRepository, usersAndCoursesRepository, temporaryLectureRepository);
 */
            top.Add(window);

            Application.Run();

        }
    }
}
