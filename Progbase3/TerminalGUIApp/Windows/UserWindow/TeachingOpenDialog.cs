using System.Collections.Generic;
using ProcessData;
using Terminal.Gui;
using TerminalGUIApp.Windows.CourseWindow;

namespace TerminalGUIApp
{
    public class TeachingOpenDialog : Dialog
    {
        protected User user;
        private UserRepository userRepository;
        protected UsersAndCoursesRepository usersAndCoursesRepository;
        protected CourseRepository courseRepository;
        protected LectureRepository lectureRepository;
        private int pageLength = 10;
        private int page = 1;
        private ListView allCoursesListView;
        private FrameView frameView;
        public TeachingOpenDialog()
        {
            user = new User();
            user.id = 182;
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

            Button addCourse = new Button(40, 20, "Create new course");
            addCourse.Clicked += OnCreateButtonClicked;
            this.AddButton(addCourse);

        }


        private void OnCreateButtonClicked()
        {
            CreateCourseDialog dialog = new CreateCourseDialog();

            dialog.SetUser(this.user);

            Application.Run(dialog);

            if (!dialog.canceled)
            {
                Course newCourse = dialog.GetCourse();
                if (newCourse != null)
                {
                    newCourse.id = this.courseRepository.Insert(newCourse);

                    Lecture[] allLectures = dialog.GetAllLectures();
                    
                    foreach (Lecture l in allLectures)
                    {
                        l.courseId = newCourse.id;
                        this.lectureRepository.Insert(l);
                    }

                    List<Course> userCourses = new List<Course>(this.courseRepository.GetAllAuthorCourses(user.id));
                    allCoursesListView.SetSource(userCourses);
                }
            }
        }



        private void OpenUserCourseDialog(ListViewItemEventArgs args)
        {
            Course course = (Course)args.Value;

            OpenUserCourseDialog dialog = new OpenUserCourseDialog();

            dialog.SetUser(this.user);

            dialog.SetCourse(course);

            dialog.SetRepositories(this.userRepository, this.courseRepository, this.lectureRepository, this.usersAndCoursesRepository);

            Application.Run(dialog);

            if (dialog.edited)
            {
                Course editedCourse = dialog.GetCourse();

                List<Course> userCourses = new List<Course>(this.courseRepository.GetAllAuthorCourses(user.id));

                allCoursesListView.SetSource(userCourses);
            }

            if (dialog.deleted)
            {
                this.courseRepository.DeleteById(course.id);

                List<Course> userCourses = new List<Course>(this.courseRepository.GetAllAuthorCourses(user.id));

                allCoursesListView.SetSource(userCourses);
            }
        }


        public void SetRepositories(CourseRepository courseRepository, UserRepository userRepository, UsersAndCoursesRepository usersAndCoursesRepository, LectureRepository lectureRepository)
        {
            this.usersAndCoursesRepository = usersAndCoursesRepository;
            this.courseRepository = courseRepository;
            this.userRepository = userRepository;
            this.lectureRepository = lectureRepository;

            List<Course> userCourses = new List<Course>(this.courseRepository.GetAllAuthorCourses(user.id));
            allCoursesListView.SetSource(userCourses);
        }

        private void OnCreateDialogSubmit()
        {
            Application.RequestStop();
        }
    }
}