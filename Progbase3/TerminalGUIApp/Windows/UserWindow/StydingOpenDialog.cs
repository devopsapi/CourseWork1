using System.Collections.Generic;
using ProcessData;
using Terminal.Gui;

namespace TerminalGUIApp.Windows.UserWindow
{
    public class StydingOpenDialog : Dialog
    {
        private User currentUser;
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

        public StydingOpenDialog()
        {
            this.Title = "Styding";

            Button backBtn = new Button(45, 30, "Back")
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

            Button unsubscribed = new Button(42, 20, "Unsubscribed");
            unsubscribed.Clicked += OnUnsubscribed;
            this.AddButton(unsubscribed);

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
            List<int> ids = this.usersAndCoursesRepository.GetAllUserCoursesId(this.currentUser.id);
            int totalPages = this.courseRepository.GetSearchPagesCountOfSubscibedCourses(ids, pageLength, searchValue);

            if (page >= totalPages)
            {
                return;
            }

            this.page += 1;

            UpdateCurrentPage();
        }

        private void OnPrevPage()
        {
            List<int> ids = this.usersAndCoursesRepository.GetAllUserCoursesId(this.currentUser.id);
            int totalPages = this.courseRepository.GetSearchPagesCountOfSubscibedCourses(ids, pageLength, searchValue);

            if (page == 1)
            {
                return;
            }

            this.page -= 1;

            UpdateCurrentPage();
        }


        private void UpdateCurrentPage()
        {
            List<int> ids = this.usersAndCoursesRepository.GetAllUserCoursesId(this.currentUser.id);
            int totalPages = this.courseRepository.GetSearchPagesCountOfSubscibedCourses(ids, pageLength, searchValue);

            if (page > totalPages)
            {
                page = 1;
            }

            this.pageLbl.Text = page.ToString();
            this.totalPagesLbl.Text = totalPages.ToString();

            if (!selecting)
            {
                this.allCoursesListView.SetSource(this.courseRepository.GetSearchPageOfSubscribedCourses(ids, searchValue, page, pageLength));

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

        private void OnUnsubscribed()
        {
            int index = this.allCoursesListView.SelectedItem;

            if (index == -1)
            {
                return;
            }

            Course selectedCourse = (Course)this.allCoursesListView.Source.ToList()[index];

            this.usersAndCoursesRepository.Delete(this.currentUser.id, selectedCourse.id);

            UpdateCurrentPage();
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
        }

        private void OnOpenCourse(ListViewItemEventArgs args)
        {
            Course course = (Course)args.Value;

            OpenCourseDialog dialog = new OpenCourseDialog();

            dialog.SetRepositories(this.userRepository, this.courseRepository, this.lectureRepository, this.usersAndCoursesRepository, this.temporaryLectureRepository);

            dialog.SetUser(this.currentUser);

            dialog.SetCourse(course);

            dialog.CheckIfUserSubscribed();

            Application.Run(dialog);

            UpdateCurrentPage();
        }


        private void OnCreateDialogSubmit()
        {
            Application.RequestStop();
        }
    }
}