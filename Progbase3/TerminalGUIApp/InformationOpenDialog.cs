using System;
using ProcessData;
using Terminal.Gui;
namespace TerminalGUIApp
{
    public class InformationOpenDialog : Dialog
    {

        private User user;
        private TextField userId;
        private TextField createdAt;
        private TextField fullnameInput;
        private TextField usernameInput;
        private TextField subscribed;
        private UserRepository userRepository;
        private CourseRepository courseRepository;
        private UsersAndCoursesRepository usersAndCoursesRepository;

        public InformationOpenDialog()
        {
            //  user = new User();
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

            // NO DATA EXSIST PROBLEM, BETTER DELETE THIS
            /*   Label subscibedOnLbl = new Label("Subscribed on courses:")
              {
                  X = Pos.Left(usernameLbl),
                  Y = Pos.Top(usernameLbl) + Pos.Percent(40),
              };
              subscribed = new TextField("")
              {
                  X = Pos.Left(usernameInput),
                  Y = Pos.Top(subscibedOnLbl),
                  Width = Dim.Percent(25),
                  ReadOnly = true,
              };
              this.Add(subscibedOnLbl, subscribed); */

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
            int index = MessageBox.Query("Delete","Are you sure?", "NO", "YES");

            if (index == 1)
            {
                userRepository.DeleteByUsername(this.user.username);
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

        public void SetRepositories(CourseRepository courseRepository, UserRepository userRepository, UsersAndCoursesRepository usersAndCoursesRepository)
        {
            this.usersAndCoursesRepository = usersAndCoursesRepository;
            this.courseRepository = courseRepository;
            this.userRepository = userRepository;
        }
    }
}