using ProcessData;
using Terminal.Gui;
using System;

namespace TerminalGUIApp
{
    public class CreateLectureDialog : Dialog
    {
        public bool canceled;
        protected TextField topicInput;
        protected TextField descriptionInput;
        protected TextField duration;
        protected TextField courseIdInput;

        public CreateLectureDialog()
        {
            this.Title = "Create lecture";
            Button canceledBtn = new Button("Canceled");
            canceledBtn.Clicked += OnCreateDialogCanceled;
            this.AddButton(canceledBtn);

            Button okBtn = new Button("OK");
            okBtn.Clicked += OnCreateDialogSubmit;
            this.AddButton(okBtn);

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
            };
            this.Add(descriptionLbl, descriptionInput);

            Label durationLbl = new Label("Duration")
            {
                X = Pos.Left(topicLbl),
                Y = Pos.Top(topicLbl) + Pos.Percent(20),
            };
            duration = new TextField("")
            {
                X = Pos.Left(topicInput),
                Y = Pos.Top(durationLbl),
                Width = Dim.Percent(25),
            };
            this.Add(durationLbl, duration);


            Label courseUserIdLbl = new Label("User id: ")
            {
                X = Pos.Left(topicLbl),
                Y = Pos.Top(topicLbl) + Pos.Percent(30),
            };
            courseIdInput = new TextField("")
            {
                X = Pos.Left(topicInput),
                Y = Pos.Top(courseUserIdLbl),
                Width = Dim.Percent(25),
            };
            this.Add(courseUserIdLbl, courseIdInput);
        }

        public Lecture GetLecture()
        {
            Lecture lecture = new Lecture();
            lecture.topic = topicInput.Text.ToString();
            lecture.description = descriptionInput.Text.ToString();
            lecture.duration = duration.Text.ToString();
          //  lecture.course_id = 
            return lecture;
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