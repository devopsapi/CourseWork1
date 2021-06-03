using System;
using ProcessData;
using Terminal.Gui;
using TerminalGUIApp.Windows.LectureWindow;

namespace TerminalGUIApp
{
    public class OpenLectureDialog : Dialog
    {
        public bool edited;
        public bool deleted;
        protected Lecture lecture;
        protected TextField topicInput;
        //      protected TextField lectureIdInput;
        protected TextField descriptionInput;
        protected TextField durationInput;
        protected Button editBtn;
        protected Button deleteBtn;
        protected TextField createdAt;
        public OpenLectureDialog()
        {
            this.Title = "Open lecture";

            Button backBtn = new Button(46, 30, "Back");
            backBtn.Clicked += OnCreateDialogSubmit;
            this.AddButton(backBtn);


            editBtn = new Button(60, 1, "Edit");
            editBtn.Clicked += OnEditButtonClicked;
            editBtn.Visible = false;
            this.AddButton(editBtn);

            deleteBtn = new Button(70, 1, "Delete");
            deleteBtn.Clicked += OnDeleteButtonClicked;
            deleteBtn.Visible = false;
            this.AddButton(deleteBtn);

            Label topicLbl = new Label("Topic: ")
            {
                X = Pos.Percent(5),
                Y = Pos.Percent(5),

            };
            topicInput = new TextField("")
            {
                X = Pos.Percent(20),
                Y = Pos.Top(topicLbl),
                Width = Dim.Percent(25),
                ReadOnly = true,
            };
            this.Add(topicLbl, topicInput);

            Label descriptionLbl = new Label("Description: ")
            {
                X = Pos.Left(topicLbl),
                Y = Pos.Top(topicLbl) + Pos.Percent(10),
            };
            descriptionInput = new TextField("")
            {
                X = Pos.Left(topicInput),
                Y = Pos.Top(descriptionLbl),
                Width = Dim.Percent(25),
                ReadOnly = true,
            };
            this.Add(descriptionLbl, descriptionInput);

            Label durationLbl = new Label("Duration")
            {
                X = Pos.Left(topicLbl),
                Y = Pos.Top(topicLbl) + Pos.Percent(20),
            };
            durationInput = new TextField("")
            {
                X = Pos.Left(topicInput),
                Y = Pos.Top(durationLbl),
                Width = Dim.Percent(25),
                ReadOnly = true,
            };
            this.Add(durationLbl, durationInput);

            Label createdAtLbl = new Label("Created at:")
            {
                X = Pos.Left(topicLbl),
                Y = Pos.Top(topicLbl) + Pos.Percent(30),
            };
            createdAt = new TextField("")
            {
                X = Pos.Left(topicInput),
                Y = Pos.Top(createdAtLbl),
                Width = Dim.Percent(25),
                ReadOnly = true,
            };
            this.Add(createdAtLbl, createdAt);

        }

        public void CheckIfLectureCanBeEditedAndDeleted(bool isEditedAndDeleted)
        {
            if (isEditedAndDeleted)
            {
                this.editBtn.Visible = true;
                this.deleteBtn.Visible = true;
            }
        }

        private void OnDeleteButtonClicked()
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
            EditLectureDialog dialog = new EditLectureDialog();

            dialog.SetLecture(this.lecture);

            Application.Run(dialog);

            if (!dialog.canceled)
            {
                Lecture editedLecture = dialog.GetLecture();
                editedLecture.id = this.lecture.id;
                this.edited = true;
                this.SetLecture(editedLecture);
            }
        }
        public void SetLecture(Lecture lecture)
        {
            this.topicInput.Text = lecture.topic;
            this.descriptionInput.Text = lecture.description;
            this.durationInput.Text = lecture.duration.ToString();
            this.createdAt.Text = lecture.createdAt.ToString();

            this.lecture = lecture;
        }

        public Lecture GetLecture()
        {
            return this.lecture;
        }
        private void OnCreateDialogSubmit()
        {
            Application.RequestStop();
        }
    }
}