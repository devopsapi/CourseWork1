using System;
using System.Collections.Generic;
using ProcessData;
using Terminal.Gui;

namespace TerminalGUIApp
{
    public class CreateCourseDialog : Dialog
    {
        private List<Lecture> list = new List<Lecture>();
        public bool canceled;
        private User user;
        protected TextField titleInput;
        protected TextField descriptionInput;
        protected DateField courseCreatedAtDateField;
        protected TextField authorInput;
        protected TextField subscribers;
        protected TextField rating;
        protected TextField priceInput;
        protected CheckBox isPrivateCheckBox;
        protected TextField courseUserIdInput;

        public CreateCourseDialog()
        {
            this.Title = "Create course";

            Button canceledBtn = new Button("Cancel");
            canceledBtn.Clicked += OnCreateDialogCanceled;
            this.AddButton(canceledBtn);

            Button okBtn = new Button("OK");
            okBtn.Clicked += OnCreateDialogSubmit;
            this.AddButton(okBtn);

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
            };
            this.Add(priceLbl, priceInput);

            Label isPrivateLbl = new Label("Is private")
            {
                X = Pos.Left(titleLbl),
                Y = Pos.Top(titleLbl) + Pos.Percent(40),
            };
            isPrivateCheckBox = new CheckBox("")
            {
                X = Pos.Left(titleInput),
                Y = Pos.Top(isPrivateLbl),
            };
            this.Add(isPrivateLbl, isPrivateCheckBox);

            Button addLectures = new Button("Create lectures");
            addLectures.Clicked += OnCreateLecturesClicked;
            this.AddButton(addLectures);
        }

        private void OnCreateLecturesClicked()
        {
            CreateLectureDialog dialog = new CreateLectureDialog();

            Application.Run(dialog);

            if (!dialog.canceled)
            {
                Lecture newLecture = dialog.GetLecture();
                if (newLecture != null)
                {
                    this.list.Add(newLecture);
                }
            }
        }

        public Lecture[] GetAllLectures()
        {
            Lecture[] allLectures = new Lecture[this.list.Count];
            list.CopyTo(allLectures);
            return allLectures;
        }

        public Course GetCourse()
        {
            double tryParsePrice;

            if (titleInput.Text.ToString() == "" || descriptionInput.Text.ToString() == "" || authorInput.Text.ToString() == "")
            {
                MessageBox.ErrorQuery("Course", "All fields must be filled", "OK");
                return null;
            }

            if (!double.TryParse(priceInput.Text.ToString(), out tryParsePrice) || tryParsePrice < 0)
            {
                MessageBox.ErrorQuery("Creating course", "Incorrect price value. Must be non-negative integer", "Ok");
                return null;
            }

            Course course = new Course();

            course.title = titleInput.Text.ToString();
            course.description = descriptionInput.Text.ToString();
            course.author = authorInput.Text.ToString();
            course.price = int.Parse(priceInput.Text.ToString());
            course.isPrivate = isPrivateCheckBox.Checked;
            course.publishedAt = DateTime.Now;
            course.amountOfSubscribers = 0;
            course.rating = 0;
            course.userId = this.user.id;

            return course;
        }

        public void SetUser(User user)
        {
            this.user = user;
        }
        private void OnCreateDialogCanceled()
        {
            this.canceled = true;

            Application.RequestStop();
        }

        private void OnCreateDialogSubmit()
        {
            if (this.list.Count == 0)
            {
                MessageBox.ErrorQuery("Create course", "Course munst have at least 1 lecture.Add lectures first", "OK");
                return;
            }

            this.canceled = false;

            Application.RequestStop();
        }
    }
}