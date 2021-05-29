using System;
using System.Collections.Generic;
using ProcessData;
using Terminal.Gui;

namespace TerminalGUIApp
{
    public class MainWindow : Window
    {
        private User currentUser;
        private MenuBar mainMenu;
        private MenuBar helpMenu;
        private MenuBar userAccountMenu;
        private CourseRepository courseRepository;
        private LectureRepository LectureRepository;
        private UserRepository userRepository;
        private UsersAndCoursesRepository usersAndCoursesRepository;
        private ListView allCoursesListView;
        private FrameView frameView;
        private int pageLength = 10;
        private int page = 1;
        private string searchValue = "";
        private bool selecting = false;

        public MainWindow()
        {
            mainMenu = new MenuBar(
               new MenuBarItem[]
               {
                    new MenuBarItem ("_File", new MenuItem[]
                    {
                        new MenuItem("_Export...", "", OnExportOpen)
                        {
                            CanExecute = CanBeExecuted,
                        },
                        new MenuItem("_Import...", "", OnImportOpen)
                        {
                            CanExecute = CanBeExecuted,
                        },
                        new MenuItem("_Quit", "", OnQuit),
                    })
               })
            {
                Width = Dim.Percent(5),
            };
            mainMenu.MenuOpening += OnAllMenusClose;
            helpMenu = new MenuBar(
                new MenuBarItem[]
                {
                    new MenuBarItem("_Help", new MenuItem[]
                    {
                        new MenuItem("_About", "", OnAbout)
                    })
                }
            )
            {
                X = Pos.Left(mainMenu),
                Y = Pos.Bottom(mainMenu) + Pos.Percent(1),
                Width = Dim.Percent(5)
            };
            this.Add(mainMenu, helpMenu);


            userAccountMenu = new MenuBar(
                new MenuBarItem[]{new MenuBarItem("Account",new MenuItem[]{
                    new MenuBarItem ("Information", "",OnInformationOpen),
                    new MenuBarItem("Teaching", "", OnTeachingOpen),
                    new MenuBarItem("Styding", "", OnStydingOpen),
                }
                )})
            {
                X = Pos.Left(helpMenu),
                Y = Pos.Bottom(helpMenu) + Pos.Percent(1),
                Width = Dim.Percent(5)
            };


            this.Add(userAccountMenu);




            //////////////////////////////////////

            currentUser = new User();

            this.Title = "Online styding";

            //   Rect frame = new Rect(2,8,100,50);

            allCoursesListView = new ListView(new List<Course>())
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
            allCoursesListView.OpenSelectedItem += OnOpenCourse;

            frameView = new FrameView("Courses")
            {
                X = Pos.Percent(15),
                Y = Pos.Percent(20),
                Width = Dim.Fill() - Dim.Percent(15),
                Height = pageLength + 2,
            };
            frameView.Add(allCoursesListView);
            this.Add(frameView);


            Button subscribe = new Button("Subscribe")
            {
                X = Pos.Center(),
                Y = allCoursesListView.Y + 21,
            };
            subscribe.Clicked += OnSubscribeClicked;
            this.Add(subscribe);

            /*    Button createNewCourseBtn = new Button(2, 2, "Create new course");
               createNewCourseBtn.Clicked += OnCreateButtonClicked;
               this.Add(createNewCourseBtn); */

            /*  Button editButton = new Button("Edit course");
             editButton.Clicked += OnEditButtonClicked;
             win.Add(editButton); */
        }

        private void OnInformationOpen()
        {
            InformationOpenDialog dialog = new InformationOpenDialog();

            User test = new User();
            test = new User();
            test.id = 181;
            test.username = "hello";
            test.fullname = "Max Litva";
            test.password = "1231231";
            test.createdAt = DateTime.Now;
            test.isAuthor = true;
            test.imported = false;

            dialog.SetUser(test);
            dialog.SetRepositories(courseRepository, userRepository, usersAndCoursesRepository);

            Application.Run(dialog);
        }

        private void OnTeachingOpen()
        {
            OnTeachingOpen dialog = new OnTeachingOpen();

            dialog.SetRepositories(courseRepository, userRepository, usersAndCoursesRepository);

            Application.Run(dialog);

        }

        private void OnStydingOpen()
        {

        }

        private void OnQuit()
        {
            Application.RequestStop();
        }
        private void OnAbout()
        {
            MessageBox.Query("About program", "Course work project. Made by a student of KP-01 Yuliya Levitskaya, according to the lectures of the teacher Hadyniak Ruslan Anatoliiovych.", "Ok");
        }

        private void OnAllMenusClose()
        {
            MenuBar[] menus = new MenuBar[] { mainMenu, helpMenu };

            for (int i = 0; i < menus.Length; i++)
            {
                menus[i].CloseMenu();
            }
        }
        private bool CanBeExecuted()
        {
            if (currentUser == null)
            {
                return false;
            }

            return true;
        }
        private void OnImportOpen()
        {
            /* Toplevel top = Application.Top;

            ImportWindow win = new ImportWindow();
            win.SetRepositories(usersRepository, postsRepository, commentsRepository);

            RunWindow(win); */
        }
        private void OnExportOpen()
        {
            /*  Toplevel top = Application.Top;

             ExportWindow win = new ExportWindow();
             win.SetRepositories(postsRepository);

             RunWindow(win); */
        }


        private void OnCreateButtonClicked()
        {
            CreateCourseDialog dialog = new CreateCourseDialog();
            Application.Run(dialog);

            if (!dialog.canceled)
            {
                Course newCourse = dialog.GetCourse();
                if (newCourse != null)
                {
                    newCourse.id = courseRepository.Insert(newCourse);

                    allCoursesListView.SetSource(this.courseRepository.GetPage(page, pageLength));
                }
            }
        }

        private void OnSubscribeClicked()
        {


        }

        private void OnOpenCourse(ListViewItemEventArgs args)
        {
            Course course = (Course)args.Value;

            if (course.isPrivate)
            {
                MessageBox.Query("Open course", "This course is private.You do not have rights to view it", "Ok");
                return;
            }

            OpenCourseDialog dialog = new OpenCourseDialog();


            dialog.SetCourse(course);

            Application.Run(dialog);
        }

        public void SetRepositories(UserRepository userRepository, CourseRepository courseRepository, LectureRepository lectureRepository, UsersAndCoursesRepository usersAndCoursesRepository)
        {
            this.courseRepository = courseRepository;
            this.userRepository = userRepository;
            this.LectureRepository = lectureRepository;
            this.usersAndCoursesRepository = usersAndCoursesRepository;

            allCoursesListView.SetSource(this.courseRepository.GetPage(page, pageLength));
        }
    }
}