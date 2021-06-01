using System.Collections.Generic;
using ProcessData;
using Terminal.Gui;

namespace TerminalGUIApp
{
    public class AllCourseLecturesDialog : Dialog
    {
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
        private Course course;
        public AllCourseLecturesDialog()
        {
            lectureRepository = new LectureRepository(databasePath);

            this.Title = "All lectures";


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

            List<Lecture> lectures = new List<Lecture>(this.lectureRepository.GetAllCourseLectures(this.course.id));
            // allCoursesListView.SetSource(this.courseRepository.GetPage(page, pageLength));
            this.allLecturesListView.SetSource(lectures);
        }
        public void SetUser(User user)
        {
            this.user = user;
        }

        public void SetCourse(Course course)
        {
            this.course = course;
        }

        /*  public void SetRepository(LectureRepository lectureRepository)
         {
             this.lectureRepository = lectureRepository;

             List<Lecture> lectures = new List<Lecture>(this.lectureRepository.GetAllCourseLectures(this.course.id));
             // allCoursesListView.SetSource(this.courseRepository.GetPage(page, pageLength));
             this.allLecturesListView.SetSource(lectures);
         } */
        private void OnOpenLecture(ListViewItemEventArgs args)
        {
            Lecture lecture = (Lecture)args.Value;

            OpenLectureDialog dialog = new OpenLectureDialog();

            dialog.SetLecture(lecture);

            dialog.CheckIfLectureCanBeEditedAndDeleted(courseRepository.CheckIfUserIsCourseAuthor(this.user.id, this.course.id));

            Application.Run(dialog);

            if (dialog.edited)
            {
                Lecture editedLecture = dialog.GetLecture();

                this.lectureRepository.Update(editedLecture.id, editedLecture);

                List<Lecture> lectures = new List<Lecture>(this.lectureRepository.GetAllCourseLectures(this.course.id));

                this.allLecturesListView.SetSource(lectures);
            }

            else if (dialog.deleted)
            {
                this.lectureRepository.DeleteById(lecture.id);
            }

        }

        private void OnCreateDialogSubmit()
        {
            Application.RequestStop();
        }
    }
}