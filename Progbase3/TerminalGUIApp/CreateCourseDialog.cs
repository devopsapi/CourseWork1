using System;
using ProcessData;
using Terminal.Gui;

namespace TerminalGUIApp
{
    public class CreateCourseDialog : Dialog
    {
        public bool canceled;
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

            Button canceledBtn = new Button("Canceled");
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


            Label courseUserIdLbl = new Label("User id: ")
            {
                X = Pos.Left(titleLbl),
                Y = Pos.Top(titleLbl) + Pos.Percent(50),
            };
            courseUserIdInput = new TextField("")
            {
                X = Pos.Left(titleInput),
                Y = Pos.Top(courseUserIdLbl),
                Width = Dim.Percent(25),
            };
            this.Add(courseUserIdLbl, courseUserIdInput);


            Label courseCreatedAtLbl = new Label("CreatedAt: ")
            {
                X = Pos.Left(titleLbl),
                Y = Pos.Top(titleLbl) + Pos.Percent(60),
            };
            courseCreatedAtDateField = new DateField()
            {
                X = Pos.Left(titleInput),
                Y = Pos.Top(courseCreatedAtLbl),
                Width = Dim.Percent(25),
                IsShortFormat = false,
                ReadOnly = true,
            };
            this.Add(courseCreatedAtLbl, courseCreatedAtDateField);
        }

        public Course GetCourse()
        {
            double tryParsePrice;
            int tryParseId;

            if (titleInput.Text.ToString() == "" || descriptionInput.Text.ToString() == "" || authorInput.Text.ToString() == "" || courseUserIdInput.Text.ToString() == "")
            {
                MessageBox.ErrorQuery("Creating course", "All fields must be filled", "Ok");
                return null;
            }

            if (!double.TryParse(priceInput.Text.ToString(), out tryParsePrice) || tryParsePrice < 0)
            {
                MessageBox.ErrorQuery("Creating course", "Incorrect price value. Must be non-negative integer", "Ok");
                return null;
            }

            if (!int.TryParse(courseUserIdInput.Text.ToString(), out tryParseId) || tryParseId < 0)
            {
                MessageBox.ErrorQuery("Creating course", "Incorrect userId value. Must be non-negative integer", "Ok");
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
            course.userId = int.Parse(courseUserIdInput.Text.ToString());

            return course;
        }
        private void OnCreateDialogCanceled()
        {
            this.canceled = true;

            Application.RequestStop();
        }

        private void OnCreateDialogSubmit()
        {
            this.canceled = false;

            Application.RequestStop();
        }
    }
}