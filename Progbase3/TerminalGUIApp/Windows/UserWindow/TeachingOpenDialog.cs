using System.Collections.Generic;
using ProcessData;
using Terminal.Gui;
using TerminalGUIApp.Windows.CourseWindow;

namespace TerminalGUIApp
{
    public class TeachingOpenDialog : Dialog
    {
        protected User currentUser;
        private UserRepository userRepository;
        protected TemporaryLectureRepository temporaryLectureRepository;
        protected UsersAndCoursesRepository usersAndCoursesRepository;
        protected CourseRepository courseRepository;
        protected LectureRepository lectureRepository;
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
        private ListView allCoursesListView;
        private FrameView frameView;
        private Button addCourseBtn;
        private Button editCourseBtn;
        private Button deleteCourseBtn;

        public TeachingOpenDialog()
        {
            /* currentUser = new User();
            currentUser.id = 182; */
            this.Title = "Teaching";

            Button backBtn = new Button(46, 30, "Back")
            {
                X = Pos.Center(),
            };
            backBtn.Clicked += OnCreateDialogSubmit;
            this.AddButton(backBtn);

            allCoursesListView = new ListView(new List<Course>())
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
            allCoursesListView.OpenSelectedItem += OpenUserCourseDialog;

            frameView = new FrameView("Courses")
            {
                X = Pos.Percent(15),
                Y = Pos.Percent(20),
                Width = Dim.Fill() - Dim.Percent(15),
                Height = pageLength + 2,
            };
            frameView.Add(allCoursesListView);
            this.Add(frameView);

            addCourseBtn = new Button(20, 20, "Create course");
            addCourseBtn.Clicked += OnCreateButtonClicked;
            this.AddButton(addCourseBtn);

            editCourseBtn = new Button(40, 20, "Edit course");
            editCourseBtn.Clicked += OnEditClicked;
            this.AddButton(editCourseBtn);

            deleteCourseBtn = new Button(60, 20, "Delete course");
            deleteCourseBtn.Clicked += OnDeleteButtonClicked;
            this.AddButton(deleteCourseBtn);


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
            int totalPages = this.courseRepository.GetSearchPagesCountByUserId(pageLength, searchValue, this.currentUser.id);

            if (page >= totalPages)
            {
                return;
            }

            this.page += 1;

            UpdateCurrentPage();
        }

        private void OnPrevPage()
        {
            int totalPages = this.courseRepository.GetSearchPagesCountByUserId(pageLength, searchValue, this.currentUser.id);

            if (page == 1)
            {
                return;
            }

            this.page -= 1;

            UpdateCurrentPage();
        }

        private void UpdateCurrentPage()
        {
            int totalPages = this.courseRepository.GetSearchPagesCountByUserId(pageLength, searchValue, this.currentUser.id);

            if (page > totalPages)
            {
                page = 1;
            }

            this.pageLbl.Text = page.ToString();
            this.totalPagesLbl.Text = totalPages.ToString();

            if (!selecting)
            {
                this.allCoursesListView.SetSource(this.courseRepository.GetSearchPageByUserId(searchValue, page, pageLength, this.currentUser.id));

                if (allCoursesListView.Source.ToList().Count == 0)
                {
                    nullReferenceLbl = new Label("No records found")
                    {
                        X = Pos.Percent(45),
                        Y = Pos.Percent(50),
                    };
                    frameView.RemoveAll();
                    frameView.Add(nullReferenceLbl);

                    /* addCourseBtn.Visible = false;
                    editCourseBtn.Visible = false;
                    deleteCourseBtn.Visible = false; */
                }
                else
                {
                    frameView.RemoveAll();
                    frameView.Add(allCoursesListView);
                    /* 
                                        addCourseBtn.Visible = true;
                                        editCourseBtn.Visible = true;
                                        deleteCourseBtn.Visible = true; */
                }
            }
            else
            {
                selecting = false;
            }


            prevPageBtn.Visible = (page != 1);
            nextPageBtn.Visible = (page! < totalPages);
        }

        private void OnDeleteButtonClicked()
        {

            int courseIndex = this.allCoursesListView.SelectedItem;

            if (courseIndex == -1 || courseIndex >= this.allCoursesListView.Source.ToList().Count)
            {
                return;
            }

            int index = MessageBox.Query("Delete", "Are you sure?", "NO", "YES");

            if (index == 0)
            {
                return;
            }
            else
            {
                Course selectedCourse = (Course)this.allCoursesListView.Source.ToList()[courseIndex];

                bool isDeleted = this.courseRepository.DeleteById(selectedCourse.id);

                if (isDeleted)
                {
                    /* List<Course> userCourses = new List<Course>(this.courseRepository.GetAllAuthorCourses(currentUser.id));
                    allCoursesListView.SetSource(userCourses); */

                    UpdateCurrentPage();
                }

                else
                {
                    MessageBox.ErrorQuery("Delete course", "Could not delete course", "OK");
                }
            }
        }

        private void OnEditClicked()
        {
            EditCourseDialog dialog = new EditCourseDialog();

            int courseIndex = this.allCoursesListView.SelectedItem;

            if (courseIndex == -1 || courseIndex >= this.allCoursesListView.Source.ToList().Count)
            {
                return;
            }

            Course selectedCourse = (Course)this.allCoursesListView.Source.ToList()[courseIndex];

            List<Lecture> lectures = new List<Lecture>(this.lectureRepository.GetAllCourseLectures(selectedCourse.id));

            dialog.justCreated = false;
            dialog.SetCourse(selectedCourse);
            dialog.SetUser(this.currentUser);
            dialog.SetRepositories(this.userRepository, this.courseRepository, this.lectureRepository, this.usersAndCoursesRepository, this.temporaryLectureRepository);

            Application.Run(dialog);

            if (!dialog.canceled)
            {
                Course editedCourse = dialog.GetCourse();
                editedCourse.id = selectedCourse.id;
                if (editedCourse != null)
                {
                    bool isUpdated = this.courseRepository.Update(editedCourse.id, editedCourse);

                    if (isUpdated)
                    {
                        /*  List<Course> userCourses = new List<Course>(this.courseRepository.GetAllAuthorCourses(currentUser.id));
                         allCoursesListView.SetSource(userCourses); */

                        UpdateCurrentPage();
                    }
                    else
                    {
                        MessageBox.ErrorQuery("Edite course", "Could not edit course", "OK");
                    }
                }
            }
        }


        private void OnCreateButtonClicked()
        {
            CreateCourseDialog dialog = new CreateCourseDialog();

            dialog.justCreated = true;

            dialog.SetUser(this.currentUser);
            dialog.SetRepositories(this.userRepository, this.courseRepository, this.lectureRepository, this.usersAndCoursesRepository, this.temporaryLectureRepository);

            Application.Run(dialog);

            if (!dialog.canceled)
            {
                Course newCourse = dialog.GetCourse();
                if (newCourse != null)
                {
                    newCourse.id = this.courseRepository.Insert(newCourse);

                    Lecture[] allLectures = dialog.GetAllLectures();

                    if (allLectures.Length != 0)
                    {
                        foreach (Lecture l in allLectures)
                        {
                            l.courseId = newCourse.id;
                            this.lectureRepository.Insert(l);
                            this.temporaryLectureRepository.DeleteById(l.id);
                        }


                        /*   List<Course> userCourses = new List<Course>(this.courseRepository.GetAllAuthorCourses(currentUser.id));
                      allCoursesListView.SetSource(userCourses); */

                        UpdateCurrentPage();
                    }
                }
            }
        }

        public void SetUser(User user)
        {
            this.currentUser = user;
        }


        private void OpenUserCourseDialog(ListViewItemEventArgs args)
        {
            Course course = (Course)args.Value;

            OpenUserCourseDialog dialog = new OpenUserCourseDialog();

            dialog.SetUser(this.currentUser);

            dialog.SetCourse(course);

            dialog.SetRepositories(this.userRepository, this.courseRepository, this.lectureRepository, this.usersAndCoursesRepository, this.temporaryLectureRepository);

            Application.Run(dialog);

            if (dialog.edited)
            {
                Course editedCourse = dialog.GetCourse();

                /*  List<Course> userCourses = new List<Course>(this.courseRepository.GetAllAuthorCourses(currentUser.id));

                 allCoursesListView.SetSource(userCourses); */

                UpdateCurrentPage();
            }

            if (dialog.deleted)
            {
                this.courseRepository.DeleteById(course.id);

                UpdateCurrentPage();

                /*   List<Course> userCourses = new List<Course>(this.courseRepository.GetAllAuthorCourses(currentUser.id));

                  allCoursesListView.SetSource(userCourses); */
            }
        }


        public void SetRepositories(CourseRepository courseRepository, UserRepository userRepository, UsersAndCoursesRepository usersAndCoursesRepository, LectureRepository lectureRepository, TemporaryLectureRepository temporaryLectureRepository)
        {
            this.usersAndCoursesRepository = usersAndCoursesRepository;
            this.courseRepository = courseRepository;
            this.userRepository = userRepository;
            this.lectureRepository = lectureRepository;
            this.temporaryLectureRepository = temporaryLectureRepository;

            UpdateCurrentPage();
        }

        private void OnCreateDialogSubmit()
        {
            Application.RequestStop();
        }
    }
}