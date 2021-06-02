using System.Collections.Generic;
using ProcessData;
using Terminal.Gui;

namespace TerminalGUIApp.Windows.LectureWindow
{
    public class OpenLectureAfterCreateDialog : Dialog
    {
        public bool canBeEditedAndDeleted;
        private ListView allLecturesListView;
        private FrameView frameView;
        private int pageLength = 10;
        private int page = 1;
        private Course course;
        private List<Lecture> lectures;

        public OpenLectureAfterCreateDialog()
        {
            this.Title = "All lectures";


            allLecturesListView = new ListView(new List<Lecture>())
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
            allLecturesListView.OpenSelectedItem += OnOpenLecture;

            frameView = new FrameView("Lectures")
            {
                X = Pos.Percent(15),
                Y = Pos.Percent(20),
                Width = Dim.Fill() - Dim.Percent(15),
                Height = pageLength + 2,
            };
            frameView.Add(allLecturesListView);
            this.Add(frameView);

            Button backBtn = new Button("Back");
            backBtn.Clicked += OnCreateDialogSubmit;
            this.AddButton(backBtn);

            Button addLectureBtn = new Button(30, 20, "Add lecture");
            addLectureBtn.Clicked += OnAddLectureClicked;
            this.AddButton(addLectureBtn);
        }


        private void OnAddLectureClicked()
        {
            CreateLectureDialog dialog = new CreateLectureDialog();
            Application.Run(dialog);

            if (!dialog.canceled)
            {
                Lecture newLecture = dialog.GetLecture();
                this.lectures.Add(newLecture);
                this.allLecturesListView.SetSource(this.lectures);
            }
        }
        private void OnCreateDialogSubmit()
        {
            Application.RequestStop();
        }

        public void SetLectureList(List<Lecture> lectures)
        {
            this.lectures = lectures;
            this.allLecturesListView.SetSource(lectures);
        }

        private void OnOpenLecture(ListViewItemEventArgs args)
        {
            Lecture lecture = (Lecture)args.Value;

            OpenLectureDialog dialog = new OpenLectureDialog();

            dialog.SetLecture(lecture);

            dialog.CheckIfLectureCanBeEditedAndDeleted(true);

            Application.Run(dialog);

            if (dialog.edited)
            {
                Lecture editedLecture = dialog.GetLecture();

                this.lectures[this.lectures.FindIndex(ind => ind.topic.Equals(lecture.topic))] = editedLecture;

                this.allLecturesListView.SetSource(lectures);
            }

            else if (dialog.deleted)
            {
                this.lectures.Remove(lecture);

                this.allLecturesListView.SetSource(lectures);
            }
        }
    }
}