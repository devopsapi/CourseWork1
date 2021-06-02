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
        private ListView allCoursesListView;
        private FrameView frameView;
        private int pageLength = 10;
        private int page = 1;
        private string searchValue = "";
        private bool selecting = false;

        public StydingOpenDialog()
        {
            this.Title = "Styding";

            Button backBtn = new Button("Back")
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

            Button unsubscribed = new Button(30, 20, "Unsubscribed");
            unsubscribed.Clicked += OnUnsubscribed;
            this.AddButton(unsubscribed);
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

            List<Course> courses = new List<Course>(this.courseRepository.GetAllUserCourses(this.usersAndCoursesRepository.GetAllUserCoursesId(this.currentUser.id)));

            // allCoursesListView.SetSource(this.courseRepository.GetPage(page, pageLength));
            allCoursesListView.SetSource(courses);
        }

        public void SetUser(User user)
        {
            this.currentUser = user;
        }

        public void SetRepositories(UserRepository userRepository, CourseRepository courseRepository, LectureRepository lectureRepository, UsersAndCoursesRepository usersAndCoursesRepository)
        {
            this.courseRepository = courseRepository;
            this.userRepository = userRepository;
            this.lectureRepository = lectureRepository;
            this.usersAndCoursesRepository = usersAndCoursesRepository;


            List<Course> courses = new List<Course>(this.courseRepository.GetAllUserCourses(this.usersAndCoursesRepository.GetAllUserCoursesId(this.currentUser.id)));

            // allCoursesListView.SetSource(this.courseRepository.GetPage(page, pageLength));
            allCoursesListView.SetSource(courses);
        }

        private void OnOpenCourse(ListViewItemEventArgs args)
        {
            Course course = (Course)args.Value;

            OpenCourseDialog dialog = new OpenCourseDialog();

            dialog.SetRepositories(this.userRepository, this.courseRepository, this.lectureRepository, this.usersAndCoursesRepository);

            dialog.SetUser(this.currentUser);

            dialog.SetCourse(course);

            dialog.CheckIfUserSubscribed();

            Application.Run(dialog);
        }


        private void OnCreateDialogSubmit()
        {
            Application.RequestStop();
        }
    }
}