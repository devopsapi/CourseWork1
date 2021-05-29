using System;
using ProcessData;
using Terminal.Gui;

namespace TerminalGUIApp
{
    internal class OpenLectureDialog : Dialog
    {
        protected TextField topicInput;
        protected TextField lectureIdInput;
        protected TextField descriptionInput;
        protected TextField durationInput;
        public OpenLectureDialog()
        {
            this.Title = "Open lecture";

            Button backBtn = new Button("Back");
            backBtn.Clicked += OnCreateDialogSubmit;
            this.AddButton(backBtn);

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

            Label lectureIdLbl = new Label("Lecture id:")
            {
                X = Pos.Left(topicLbl),
                Y = Pos.Top(topicLbl) + Pos.Percent(30),
            };
            lectureIdInput = new TextField("")
            {
                X = Pos.Left(topicInput),
                Y = Pos.Top(durationLbl),
                Width = Dim.Percent(25),
                ReadOnly = true,
            };
            this.Add(lectureIdLbl, lectureIdInput);

        }
        public void SetLecture(Lecture lecture)
        {
            this.topicInput.Text = lecture.topic;
            this.descriptionInput.Text = lecture.description;
            this.durationInput.Text = lecture.duration.ToString();
            this.lectureIdInput.Text = lecture.id.ToString();
        }

        private void OnCreateDialogSubmit()
        {
            Application.RequestStop();
        }
    }
}