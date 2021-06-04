using System;
using ProcessData;
using Terminal.Gui;
using TerminalGUIApp.Windows.UserWindows;

namespace TerminalGUIApp
{
    public class EditUserAccountDialog : Dialog
    {
        public bool accepted;
        public bool edited;
        private User user;
        private TextField fullnameInput;
        private TextField usernameInput;
        private UserRepository userRepository;
        private CourseRepository courseRepository;
        private UsersAndCoursesRepository usersAndCoursesRepository;
        private Button changePasswordBtn;
        public EditUserAccountDialog()
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
            };
            this.Add(fullnameLbl, fullnameInput);

            changePasswordBtn = new Button("Change password")
            {
                X = Pos.Right(fullnameInput) + Pos.Percent(3),
                Y = Pos.Top(fullnameInput),
                AutoSize = true,
            };
            changePasswordBtn.Clicked += OnOpenChangePassDialog;
            this.Add(changePasswordBtn);


            Button cancelBtn = new Button("Cancel");
            cancelBtn.Clicked += OnDialogCancel;
            this.AddButton(cancelBtn);

            Button oklBtn = new Button("OK");
            oklBtn.Clicked += OnDialogSubmit;
            this.AddButton(oklBtn);

        }

        private void OnOpenChangePassDialog()
        {
            ChangePassDialog dialog = new ChangePassDialog();
            Application.Run(dialog);

            if (dialog.passChanged)
            {
                accepted = true;
            }
        }


        public User GetEditedUser()
        {
            if (userRepository.UserExists(this.usernameInput.Text.ToString()))
            {
                MessageBox.ErrorQuery("Edit", "Username is already taken. Please choose another one.", "OK");
            }
            if (this.usernameInput.Text.ToString() == "" || this.fullnameInput.Text.ToString() == "")
            {
                MessageBox.ErrorQuery("Edit", "All fields must be filled", "OK");
            }
            else
            {
                if (this.usernameInput.Text.ToString() != "" || this.fullnameInput.Text.ToString() != "")
                {
                    this.user.username = this.usernameInput.Text.ToString();
                    this.user.fullname = this.fullnameInput.Text.ToString();

                    this.edited = userRepository.Update(this.user.id, this.user);
                }
            }

            return this.user;
        }

        public void SetUser(User user)
        {
            this.usernameInput.Text = user.username;
            this.fullnameInput.Text = user.fullname;

            this.user = user;
        }

        public void SetRepositories(CourseRepository courseRepository, UserRepository userRepository, UsersAndCoursesRepository usersAndCoursesRepository)
        {
            this.usersAndCoursesRepository = usersAndCoursesRepository;
            this.courseRepository = courseRepository;
            this.userRepository = userRepository;
        }

        private void OnDialogSubmit()
        {
            this.edited = true;
            Application.RequestStop();
        }

        private void OnDialogCancel()
        {
            this.edited = false;
            Application.RequestStop();
        }
    }
}