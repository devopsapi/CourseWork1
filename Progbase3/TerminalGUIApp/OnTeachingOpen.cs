using System.Collections.Generic;
using ProcessData;
using Terminal.Gui;

namespace TerminalGUIApp
{
    public class OnTeachingOpen : Dialog
    {
        protected User currentUser;
        private UserRepository userRepository;
        protected UsersAndCoursesRepository usersAndCoursesRepository;
        protected CourseRepository courseRepository;
        protected LectureRepository lectureRepository;
        private int pageLength = 10;
        private int page = 1;
        private ListView allCoursesListView;
        private FrameView frameView;
        public OnTeachingOpen()
        {
            currentUser = new User();
            currentUser.id = 181;
            this.Title = "Teaching";

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

        }



        private void OnOpenCourse(ListViewItemEventArgs args)
        {
            Course course = (Course)args.Value;

            OpenCourseDialog dialog = new OpenCourseDialog();

            dialog.subscribe.Visible = false;
            
            dialog.subscribed = true;

            dialog.SetCourse(course);

            Application.Run(dialog);
        }

        public void SetRepositories(CourseRepository courseRepository, UserRepository userRepository, UsersAndCoursesRepository usersAndCoursesRepository)
        {
            this.usersAndCoursesRepository = usersAndCoursesRepository;
            this.courseRepository = courseRepository;
            this.userRepository = userRepository;

            List<Course> userCourses = new List<Course>(this.courseRepository.GetAllAuthorCourses(currentUser.id));

            allCoursesListView.SetSource(userCourses);
        }

        private void OnCreateDialogSubmit()
        {
            Application.RequestStop();
        }
    }
}