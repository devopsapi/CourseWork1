using System;
using ProcessData;
using Terminal.Gui;
using TerminalGUIApp.Windows.AuthenticationDialogs;

namespace TerminalGUIApp.Windows
{
    public class MainWindow : Window
    {
        private CourseRepository courseRepository;
        private LectureRepository lectureRepository;
        private UserRepository userRepository;
        private UsersAndCoursesRepository usersAndCoursesRepository;
        private TemporaryLectureRepository temporaryLectureRepository;

        public MainWindow()
        {
            this.Title = "Authentification";

            Button loginBtn = new Button("Login ")
            {
                X = Pos.Center(),
                Y = Pos.Center(),
            };
            loginBtn.Clicked += OnLoginButtonClicked;
            this.Add(loginBtn);

            Button signUpBtn = new Button("Sing up")
            {
                X = Pos.Center(),
                Y = Pos.Center() + 2,
            };
            signUpBtn.Clicked += OnSingUpButtonClicked;
            this.Add(signUpBtn);

            Button quitBtn = new Button("Quit")
            {
                X = 0,
                Y = 0,
            };
            quitBtn.Clicked += OnQuit;
            this.Add(quitBtn);
        }

        private void OnQuit()
        {
            Application.RequestStop();
        }

        private void OnSingUpButtonClicked()
        {
            RegisterDialog dialog = new RegisterDialog();
            dialog.SetRepository(this.userRepository);

            Application.Run(dialog);
        }

        private void OnLoginButtonClicked()
        {
            LoginDialog dialog = new LoginDialog();
            dialog.SetRepository(this.userRepository);

            Application.Run(dialog);

            if (dialog.logged)
            {
                User currentUser = dialog.GetUser;

                UserMainWindow userDialog = new UserMainWindow();

                userDialog.SetUser(currentUser);
                userDialog.SetRepositories(this.userRepository, this.courseRepository, this.lectureRepository, this.usersAndCoursesRepository, this.temporaryLectureRepository);

                Application.Run(userDialog);
            }
        }

        public void SetRepositories(UserRepository userRepository, CourseRepository courseRepository, LectureRepository lectureRepository, UsersAndCoursesRepository usersAndCoursesRepository, TemporaryLectureRepository temporaryLectureRepository)
        {
            this.courseRepository = courseRepository;
            this.userRepository = userRepository;
            this.lectureRepository = lectureRepository;
            this.usersAndCoursesRepository = usersAndCoursesRepository;
            this.temporaryLectureRepository = temporaryLectureRepository;
        }
    }
}