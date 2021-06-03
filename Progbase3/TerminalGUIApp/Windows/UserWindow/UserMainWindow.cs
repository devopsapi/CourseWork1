using System;
using System.Collections.Generic;
using ProcessData;
using Terminal.Gui;
using TerminalGUIApp.Windows.UserWindow;

namespace TerminalGUIApp
{
    public class UserMainWindow : Window
    {
        private User currentUser;
        private MenuBar mainMenu;
        private MenuBar helpMenu;
        private MenuBar userAccountMenu;
        private CourseRepository courseRepository;
        private LectureRepository lectureRepository;
        private UserRepository userRepository;
        private UsersAndCoursesRepository usersAndCoursesRepository;
        private TemporaryLectureRepository temporaryLectureRepository;
        private ListView allCoursesListView;
        private FrameView frameView;
        private int pageLength = 10;
        private int page = 1;
        private string searchValue = "";
        private bool selecting = false;
        private Button prevPageBtn;
        private Button nextPageBtn;
        private Label pageLbl;
        private Label totalPagesLbl;
        private TextField searchInput;
        private Label nullReferenceLbl = new Label();

        public UserMainWindow()
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


            this.Title = "Online styding";

            allCoursesListView = new ListView(new List<Course>())
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
            allCoursesListView.OpenSelectedItem += OnOpenCourse;

            prevPageBtn = new Button("Previous page")
            {
                X = Pos.Percent(35),
                Y = Pos.Percent(10),
            };
            pageLbl = new Label("?")
            {
                X = Pos.Right(prevPageBtn) + Pos.Percent(3),
                Y = Pos.Top(prevPageBtn),
                Width = 3,
            };

            Label separateLbl = new Label("of")
            {
                X = Pos.Right(pageLbl) + Pos.Percent(2),
                Y = Pos.Top(pageLbl),
            };
            totalPagesLbl = new Label("?")
            {
                X = Pos.Right(separateLbl) + Pos.Percent(3),
                Y = Pos.Top(pageLbl),
                Width = 3,
            };
            nextPageBtn = new Button("Next page")
            {
                X = Pos.Right(totalPagesLbl) + Pos.Percent(3),
                Y = Pos.Top(prevPageBtn),
            };
            nextPageBtn.Clicked += OnNextPage;
            prevPageBtn.Clicked += OnPrevPage;
            this.Add(prevPageBtn, pageLbl, separateLbl, totalPagesLbl, nextPageBtn);



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


            ///////////////////

            Label searchLbl = new Label("Seeking categories - ")
            {
                X = Pos.Percent(33),
                Y = Pos.Percent(15),
            };
            Label chooseSearchColumn = new Label("Title / author - ")
            {
                X = Pos.Right(searchLbl),
                Y = Pos.Top(searchLbl),
            };
            searchInput = new TextField()
            {
                X = Pos.Right(chooseSearchColumn) + Pos.Percent(1),
                Y = Pos.Top(searchLbl),
                Width = Dim.Percent(20),
            };
            searchInput.TextChanged += OnSearchChange;
            this.Add(searchLbl, chooseSearchColumn, searchInput);
        }

        private void OnSearchChange(NStack.ustring text)
        {
            searchValue = searchInput.Text.ToString();

            UpdateCurrentPage();
        }

        private void OnNextPage()
        {
            int totalPages = this.courseRepository.GetSearchPagesCount(pageLength, searchValue);

            if (page >= totalPages)
            {
                return;
            }

            this.page += 1;

            UpdateCurrentPage();
        }

        private void OnPrevPage()
        {
            int totalPages = this.courseRepository.GetSearchPagesCount(pageLength, searchValue);

            if (page == 1)
            {
                return;
            }

            this.page -= 1;

            UpdateCurrentPage();
        }


        private void UpdateCurrentPage()
        {
            int totalPages = this.courseRepository.GetSearchPagesCount(pageLength, searchValue);

            if (page > totalPages)
            {
                page = 1;
            }

            this.pageLbl.Text = page.ToString();
            this.totalPagesLbl.Text = totalPages.ToString();

            if (!selecting)
            {
                this.allCoursesListView.SetSource(this.courseRepository.GetSearchPage(searchValue, page, pageLength));

                if (allCoursesListView.Source.ToList().Count == 0)
                {
                    nullReferenceLbl = new Label("No records found")
                    {
                        X = Pos.Percent(45),
                        Y = Pos.Percent(50),
                    };
                    frameView.RemoveAll();
                    frameView.Add(nullReferenceLbl);
                }
                else
                {
                    frameView.RemoveAll();
                    frameView.Add(allCoursesListView);
                }
            }
            else
            {
                selecting = false;
            }


            prevPageBtn.Visible = (page != 1);
            nextPageBtn.Visible = (page! < totalPages);
        }

        private void OnInformationOpen()
        {
            InformationOpenDialog dialog = new InformationOpenDialog();

            dialog.SetUser(currentUser);

            dialog.SetRepositories(courseRepository, userRepository, usersAndCoursesRepository, temporaryLectureRepository);

            Application.Run(dialog);

            if (dialog.deleted)
            {
                userRepository.DeleteByUsername(this.currentUser.username);

                Application.RequestStop();
            }
        }

        private void OnTeachingOpen()
        {
            TeachingOpenDialog dialog = new TeachingOpenDialog();

            dialog.SetUser(this.currentUser);

            dialog.SetRepositories(courseRepository, userRepository, usersAndCoursesRepository, lectureRepository, temporaryLectureRepository);

            Application.Run(dialog);

            UpdateCurrentPage();
        }

        private void OnStydingOpen()
        {
            StydingOpenDialog dialog = new StydingOpenDialog();

            dialog.SetUser(currentUser);

            dialog.SetRepositories(userRepository, courseRepository, lectureRepository, usersAndCoursesRepository, temporaryLectureRepository);

            Application.Run(dialog);
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
            int courseIndex = this.allCoursesListView.SelectedItem;

            if (courseIndex == -1)
            {
                return;
            }

            Course selectedCourse = (Course)this.allCoursesListView.Source.ToList()[courseIndex];

            if (selectedCourse.isPrivate)
            {
                MessageBox.Query("Subscription", "You have no right to subscribe on this course", "OK");
                return;
            }

            bool isExist = this.usersAndCoursesRepository.isExists(this.currentUser.id, selectedCourse.id);

            if (isExist)
            {
                MessageBox.Query("Subscription", "You have already subscribed on this course", "OK");
                return;
            }

            else
            {
                UsersAndCourses usersAndCourses = new UsersAndCourses();
                usersAndCourses.userId = this.currentUser.id;
                usersAndCourses.courseId = selectedCourse.id;
                usersAndCourses.id = this.usersAndCoursesRepository.Insert(usersAndCourses);

                MessageBox.Query("Subscription", "You have successfully subscribed on this course", "OK");
            }
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

            dialog.SetRepositories(this.userRepository, this.courseRepository, this.lectureRepository, this.usersAndCoursesRepository, this.temporaryLectureRepository);
            dialog.SetCourse(course);
            dialog.SetUser(this.currentUser);
            dialog.CheckIfUserSubscribed();

            Application.Run(dialog);
        }

        public void SetUser(User user)
        {
            this.currentUser = user;
        }

        public void SetRepositories(UserRepository userRepository, CourseRepository courseRepository, LectureRepository lectureRepository, UsersAndCoursesRepository usersAndCoursesRepository, TemporaryLectureRepository temporaryLectureRepository)
        {
            this.courseRepository = courseRepository;
            this.userRepository = userRepository;
            this.lectureRepository = lectureRepository;
            this.usersAndCoursesRepository = usersAndCoursesRepository;
            this.temporaryLectureRepository = temporaryLectureRepository;

            UpdateCurrentPage();
            //   allCoursesListView.SetSource(this.courseRepository.GetPage(page, pageLength));
        }
    }
}