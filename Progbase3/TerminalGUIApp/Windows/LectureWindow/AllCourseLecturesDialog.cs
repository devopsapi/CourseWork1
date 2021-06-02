using System.Collections.Generic;
using ProcessData;
using Terminal.Gui;

namespace TerminalGUIApp
{
    public class AllCourseLecturesDialog : Dialog
    {
        public bool canBeEditedAndDeleted;
        private User user;
        private static string databasePath = "C:/Users/Yuli/Desktop/CourseWork/progbase3/data/database.db";
        private UserRepository userRepository;
        private CourseRepository courseRepository;
        private LectureRepository lectureRepository;
        private UsersAndCoursesRepository usersAndCoursesRepository;
        private ListView allLecturesListView;
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
        private Course course;
        public AllCourseLecturesDialog()
        {
            lectureRepository = new LectureRepository(databasePath);

            this.Title = "All lectures";


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
            Label chooseSearchColumn = new Label("Topic - ")
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


            allLecturesListView = new ListView(new List<Lecture>())
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
            allLecturesListView.OpenSelectedItem += OnOpenLecture;

            frameView = new FrameView("Lectures")
            {
                X = Pos.Percent(15),
                Y = Pos.Percent(20),
                Width = Dim.Fill() - Dim.Percent(15),
                Height = pageLength + 2,
            };
            frameView.Add(allLecturesListView);
            this.Add(frameView);

            Button backBtn = new Button("Back");
            backBtn.Clicked += OnCreateDialogSubmit;
            this.AddButton(backBtn);
        }

        public void SetRepositories(UserRepository userRepository, CourseRepository courseRepository, LectureRepository lectureRepository, UsersAndCoursesRepository usersAndCoursesRepository)
        {
            this.courseRepository = courseRepository;
            this.userRepository = userRepository;
            this.lectureRepository = lectureRepository;
            this.usersAndCoursesRepository = usersAndCoursesRepository;

            UpdateCurrentPage();
         //  this.allLecturesListView.SetSource(this.lectureRepository.GetPage(page, pageLength, this.user.id));
        }

        private void OnSearchChange(NStack.ustring text)
        {
            searchValue = searchInput.Text.ToString();

            UpdateCurrentPage();
        }

        private void OnNextPage()
        {
            int totalPages = this.lectureRepository.GetSearchPagesCount(pageLength, searchValue, this.course.id);

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
            int totalPages = this.lectureRepository.GetSearchPagesCount(pageLength, searchValue, this.course.id);

            if (page > totalPages)
            {
                page = 1;
            }

            this.pageLbl.Text = page.ToString();
            this.totalPagesLbl.Text = totalPages.ToString();

            if (!selecting)
            {
                this.allLecturesListView.SetSource(this.lectureRepository.GetSearchPage(searchValue, page, pageLength, this.course.id));

                if (allLecturesListView.Source.ToList().Count == 0)
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
                    frameView.Add(allLecturesListView);
                }
            }
            else
            {
                selecting = false;
            }


            prevPageBtn.Visible = (page != 1);
            nextPageBtn.Visible = (page! < totalPages);
        }
        public void SetUser(User user)
        {
            this.user = user;
        }

        public void SetLectureList(List<Lecture> lecturesList)
        {
            this.allLecturesListView.SetSource(lecturesList);
        }

        public void SetCourse(Course course)
        {
            this.course = course;
        }

        private void OnOpenLecture(ListViewItemEventArgs args)
        {
            Lecture lecture = (Lecture)args.Value;

            OpenLectureDialog dialog = new OpenLectureDialog();

            dialog.SetLecture(lecture);

            if (this.canBeEditedAndDeleted)
            {
                dialog.CheckIfLectureCanBeEditedAndDeleted(true);
            }
            else
            {
                dialog.CheckIfLectureCanBeEditedAndDeleted(courseRepository.CheckIfUserIsCourseAuthor(this.user.id, this.course.id));
            }

            Application.Run(dialog);

            if (dialog.edited)
            {
                Lecture editedLecture = dialog.GetLecture();
                editedLecture.courseId = this.course.id;

                this.lectureRepository.Update(editedLecture.id, editedLecture);

                UpdateCurrentPage();
                /* 
                                List<Lecture> lectures = new List<Lecture>(this.lectureRepository.GetAllCourseLectures(this.course.id));

                                this.allLecturesListView.SetSource(lectures); */
            }

            else if (dialog.deleted)
            {
                this.lectureRepository.DeleteById(lecture.id);

                UpdateCurrentPage();
                /* 
                                List<Lecture> lectures = new List<Lecture>(this.lectureRepository.GetAllCourseLectures(this.course.id));

                                this.allLecturesListView.SetSource(lectures); */
            }

        }

        private void OnCreateDialogSubmit()
        {
            Application.RequestStop();
        }
    }
}