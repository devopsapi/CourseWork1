using System;
using System.Collections.Generic;
using ProcessData;
using Terminal.Gui;

namespace TerminalGUIApp
{
    public class OpenCourseDialog : Dialog
    {
        private static string databasePath = "C:/Users/Yuli/Desktop/CourseWork/progbase3/data/database.db";
        protected User currentUser;
        protected UsersAndCoursesRepository usersAndCoursesRepository;
        protected CourseRepository courseRepository;
        protected LectureRepository lectureRepository;
        protected TextField titleInput;
        protected TextField courseId;
        protected TextField descriptionInput;
        protected TextField courseCreatedAtDateField;
        protected TextField authorInput;
        protected TextField subscribers;
        protected TextField rating;
        protected TextField priceInput;
        protected Label isPrivateLabel;
        protected TextField courseUserIdInput;
        protected ListView lectures;
        public bool subscribed;

        //  public Button seeLecturesBtn;

        public Button subscribe;


        private Course course = new Course();


        public OpenCourseDialog()
        {
            currentUser = new User();
            usersAndCoursesRepository = new UsersAndCoursesRepository(databasePath);
            courseRepository = new CourseRepository(databasePath);
            currentUser.id = 181;

            this.Title = "Open course";

            Button backBtn = new Button("Back")
            {
                X = Pos.Center(),
            };
            backBtn.Clicked += OnCreateDialogSubmit;
            this.AddButton(backBtn);

            Button seeLecturesBtn = new Button(60, 1, "See all course's lectures");
            seeLecturesBtn.Clicked += OnSeeAllLecturesClicked;
            this.AddButton(seeLecturesBtn);

            subscribe = new Button(60, 4, "Subscribe");
            subscribe.Clicked += OnSubscribeClicked;
            this.AddButton(subscribe);


            Label titleLbl = new Label("Title: ")
            {
                X = Pos.Percent(5),
                Y = Pos.Percent(5),
            };
            titleInput = new TextField("")
            {
                X = Pos.Percent(20),
                Y = Pos.Top(titleLbl),
                Width = Dim.Percent(25),
                ReadOnly = true,
            };
            this.Add(titleLbl, titleInput);


            Label descriptionLbl = new Label("Description: ")
            {
                X = Pos.Left(titleLbl),
                Y = Pos.Top(titleLbl) + Pos.Percent(10),
            };
            descriptionInput = new TextField("")
            {
                X = Pos.Left(titleInput),
                Y = Pos.Top(descriptionLbl),
                Width = Dim.Percent(25),
                ReadOnly = true,

            };
            this.Add(descriptionLbl, descriptionInput);


            Label authorLbl = new Label("Author")
            {
                X = Pos.Left(titleLbl),
                Y = Pos.Top(titleLbl) + Pos.Percent(20),
            };
            authorInput = new TextField("")
            {
                X = Pos.Left(titleInput),
                Y = Pos.Top(authorLbl),
                Width = Dim.Percent(25),
                ReadOnly = true,
            };
            this.Add(authorLbl, authorInput);


            Label priceLbl = new Label("Price")
            {
                X = Pos.Left(titleLbl),
                Y = Pos.Top(titleLbl) + Pos.Percent(30),
            };
            priceInput = new TextField("")
            {
                X = Pos.Left(titleInput),
                Y = Pos.Top(priceLbl),
                Width = Dim.Percent(25),
                ReadOnly = true,
            };
            this.Add(priceLbl, priceInput);


            Label isPrivateLbl = new Label("Is private")
            {
                X = Pos.Left(titleLbl),
                Y = Pos.Top(titleLbl) + Pos.Percent(40),
            };
            isPrivateLabel = new Label("?")
            {
                X = Pos.Left(titleInput),
                Y = Pos.Top(isPrivateLbl),
                Width = Dim.Percent(25),
            };
            this.Add(isPrivateLbl, isPrivateLabel);


            Label subscribersLbl = new Label("Subscribers")
            {
                X = Pos.Left(titleLbl),
                Y = Pos.Top(titleLbl) + Pos.Percent(50),
            };
            subscribers = new TextField("")
            {
                X = Pos.Left(titleInput),
                Y = Pos.Top(subscribersLbl),
                Width = Dim.Percent(25),
                ReadOnly = true,
            };
            this.Add(subscribersLbl, subscribers);


            Label ratingLbl = new Label("Rating")
            {
                X = Pos.Left(titleLbl),
                Y = Pos.Top(titleLbl) + Pos.Percent(60),
            };
            rating = new TextField("")
            {
                X = Pos.Left(titleInput),
                Y = Pos.Top(ratingLbl),
                Width = Dim.Percent(25),
                ReadOnly = true,
            };
            this.Add(ratingLbl, rating);


            Label courseCreatedAtLbl = new Label("Created at: ")
            {
                X = Pos.Left(titleLbl),
                Y = Pos.Top(titleLbl) + Pos.Percent(70),
            };
            courseCreatedAtDateField = new TextField("")
            {
                X = Pos.Left(titleInput),
                Y = Pos.Top(courseCreatedAtLbl),
                Width = Dim.Percent(25),
                ReadOnly = true,
            };
            this.Add(courseCreatedAtLbl, courseCreatedAtDateField);


            /*  Label courseUserIdLbl = new Label("User id: ")
             {
                 X = Pos.Left(titleLbl),
                 Y = Pos.Top(titleLbl) + Pos.Percent(80),
             };
             courseUserIdInput = new TextField("")
             {
                 X = Pos.Left(titleInput),
                 Y = Pos.Top(courseUserIdLbl),
                 Width = Dim.Percent(25),
                 ReadOnly = true,
             };
             this.Add(courseUserIdLbl, courseUserIdInput); */


            Label courseIdLbl = new Label("Course id: ")
            {
                X = Pos.Left(titleLbl),
                Y = Pos.Top(titleLbl) + Pos.Percent(80),
            };
            courseId = new TextField()
            {
                X = Pos.Left(titleInput),
                Y = Pos.Top(courseIdLbl),
                Width = Dim.Percent(25),
                ReadOnly = true,
            };
            this.Add(courseIdLbl, courseId);


        }

        private void OnSeeAllLecturesClicked()
        {
            if (this.courseRepository.GetAllAuthorCourses(currentUser.id).Length != 0)
            {
                subscribed = true;
            }
            else
            {
                subscribed = usersAndCoursesRepository.isExists(currentUser.id, course.id);
            }

            if (subscribed)
            {
                AllCourseLecturesDialog dialog = new AllCourseLecturesDialog(int.Parse(courseId.Text.ToString()));
                Application.Run(dialog);
            }
            else
            {
                MessageBox.Query("Subscription", "You can not see course lectures. Please subscribe first.", "OK");
                return;
            }
        }

        public void GetCurrentUser(User user)
        {
            this.currentUser = user;
        }

        private void OnSubscribeClicked()
        {
            if (this.courseRepository.GetAllAuthorCourses(currentUser.id).Length != 0)
            {
                subscribed = true;
            }
            else
            {
                subscribed = usersAndCoursesRepository.isExists(currentUser.id, course.id);
            }

            if (this.subscribed)
            {
                MessageBox.Query("Subscription", "You are already subscribed on this course", "OK");
                return;
            }

            UsersAndCourses usersAndCourses = new UsersAndCourses();
            usersAndCourses.userId = currentUser.id;
            usersAndCourses.courseId = course.id;

            usersAndCourses.id = usersAndCoursesRepository.Insert(usersAndCourses);

            course.amountOfSubscribers++;
            courseRepository.Update(course.id, course);
            this.subscribed = true;
            MessageBox.Query("Subscription", "Subscribed successfully", "Ok");

        }

        public void SetCourse(Course course)
        {
            this.titleInput.Text = course.title;
            this.descriptionInput.Text = course.description;
            this.authorInput.Text = course.author;
            this.subscribers.Text = course.amountOfSubscribers.ToString();
            this.rating.Text = course.rating.ToString();
            this.priceInput.Text = course.price.ToString();
            this.isPrivateLabel.Text = course.isPrivate.ToString();
            this.courseId.Text = course.id.ToString();
            //    this.courseUserIdInput.Text = course.userId.ToString();
            this.courseCreatedAtDateField.Text = course.publishedAt.ToShortDateString();

            this.course = course;
        }



        private void OnCreateDialogSubmit()
        {
            Application.RequestStop();
        }
    }
}