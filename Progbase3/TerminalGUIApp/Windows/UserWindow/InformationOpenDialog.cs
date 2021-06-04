using System;
using ProcessData;
using Terminal.Gui;
namespace TerminalGUIApp
{
    public class InformationOpenDialog : Dialog
    {

        private User user;
        public bool deleted;
        private TextField userId;
        private TextField createdAt;
        private TextField fullnameInput;
        private TextField usernameInput;
        private UserRepository userRepository;
        private CourseRepository courseRepository;
        private UsersAndCoursesRepository usersAndCoursesRepository;
        protected TemporaryLectureRepository temporaryLectureRepository;

        public InformationOpenDialog()
        {
            this.Title = "User information";

            Label usernameLbl = new Label("Username: ")
            {
                X = Pos.Percent(5),
                Y = Pos.Percent(5),
            };
            usernameInput = new TextField("")
            {
                X = Pos.Percent(35),
                Y = Pos.Top(usernameLbl),
                Width = Dim.Percent(25),
                ReadOnly = true,
            };
            this.Add(usernameLbl, usernameInput);


            Label fullnameLbl = new Label("Fullname:")
            {
                X = Pos.Left(usernameLbl),
                Y = Pos.Top(usernameLbl) + Pos.Percent(10),
            };
            fullnameInput = new TextField("")
            {
                X = Pos.Left(usernameInput),
                Y = Pos.Top(fullnameLbl),
                Width = Dim.Percent(25),
                ReadOnly = true,
            };
            this.Add(fullnameLbl, fullnameInput);


            Label createdAtLbl = new Label("Created at:")
            {
                X = Pos.Left(usernameLbl),
                Y = Pos.Top(usernameLbl) + Pos.Percent(20),
            };
            createdAt = new TextField("")
            {
                X = Pos.Left(usernameInput),
                Y = Pos.Top(createdAtLbl),
                Width = Dim.Percent(25),
                ReadOnly = true,
            };
            this.Add(createdAtLbl, createdAt);


            Label userIdLbl = new Label("User id:")
            {
                X = Pos.Left(usernameLbl),
                Y = Pos.Top(usernameLbl) + Pos.Percent(30),
            };
            userId = new TextField("")
            {
                X = Pos.Left(usernameInput),
                Y = Pos.Top(userIdLbl),
                Width = Dim.Percent(25),
                ReadOnly = true,
            };
            this.Add(userIdLbl, userId);

            Button backBtn = new Button("Back");
            backBtn.Clicked += OnCreateDialogSubmit;
            this.AddButton(backBtn);

            Button editBtn = new Button(80, 1, "Edit");
            editBtn.Clicked += OnEditButtonClicked;
            this.AddButton(editBtn);

            Button deleteBtn = new Button(80, 3, "Delete");
            deleteBtn.Clicked += OnDeleteButton;
            this.AddButton(deleteBtn);
        }

        private void OnDeleteButton()
        {
            int index = MessageBox.Query("Delete", "Are you sure?", "NO", "YES");

            if (index == 1)
            {
                this.deleted = true;
                Application.RequestStop();
            }
        }

        private void OnEditButtonClicked()
        {
            EditUserAccountDialog dialog = new EditUserAccountDialog();

            dialog.SetUser(user);
            dialog.SetRepositories(courseRepository, userRepository, usersAndCoursesRepository);

            Application.Run(dialog);

            if (dialog.edited)
            {
                this.user = dialog.GetEditedUser();

                this.usernameInput.Text = this.user.username;
                this.fullnameInput.Text = this.user.fullname;

                this.userRepository.Update(this.user.id, user);
            }

            if (dialog.accepted)
            {
                User changedUser = dialog.GetEditedUser();

                if (changedUser == null)
                {
                    return;
                }

                changedUser.createdAt = user.createdAt;
                changedUser.id = user.id;

                bool isUpdated = userRepository.Update(user.id, changedUser);
            }
        }
        private void OnCreateDialogSubmit()
        {
            Application.RequestStop();
        }

        public void SetUser(User currentUser)
        {
            this.user = currentUser;

            this.usernameInput.Text = this.user.username.ToString();
            this.fullnameInput.Text = this.user.fullname.ToString();
            this.createdAt.Text = this.user.createdAt.ToString();
            this.userId.Text = this.user.id.ToString();
        }

        public void SetRepositories(CourseRepository courseRepository, UserRepository userRepository, UsersAndCoursesRepository usersAndCoursesRepository, TemporaryLectureRepository temporaryLectureRepository)
        {
            this.usersAndCoursesRepository = usersAndCoursesRepository;
            this.courseRepository = courseRepository;
            this.userRepository = userRepository;
            this.temporaryLectureRepository = temporaryLectureRepository;
        }
    }
}