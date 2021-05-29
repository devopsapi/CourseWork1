using System.Collections.Generic;
using ProcessData;
using Terminal.Gui;

namespace TerminalGUIApp
{
    public class AllCourseLecturesDialog : Dialog
    {
        private static string databasePath = "C:/Users/Yuli/Desktop/CourseWork/progbase3/data/database.db";
        static LectureRepository lectureRepository;
        public AllCourseLecturesDialog(int courseId)
        {
            lectureRepository = new LectureRepository(databasePath);

            this.Title = " All lectures";

            Rect frame = new Rect(2, 8, 50, 20);

            List<Lecture> lectures = new List<Lecture>(lectureRepository.GetAllCourseLectures(courseId));
            ListView allLecturesListView = new ListView(frame, lectures);
            allLecturesListView.OpenSelectedItem += OnOpenLecture;
            this.Add(allLecturesListView);

            Button backBtn = new Button("Back");
            backBtn.Clicked += OnCreateDialogSubmit;
            this.AddButton(backBtn);
        }

        private void OnOpenLecture(ListViewItemEventArgs args)
        {
            Lecture lecture = (Lecture)args.Value;

            OpenLectureDialog dialog = new OpenLectureDialog();

            dialog.SetLecture(lecture);

            Application.Run(dialog);

        }

        private void OnCreateDialogSubmit()
        {
            Application.RequestStop();
        }
    }
}