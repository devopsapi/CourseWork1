using ProcessData;

namespace TerminalGUIApp.Windows.LectureWindow
{
    public class EditLectureDialog : CreateLectureDialog
    {
        public EditLectureDialog()
        {
            this.Title = "Edit lecture";
        }

        public void SetLecture(Lecture lecture)
        {
            this.topicInput.Text = lecture.topic;
            this.descriptionInput.Text = lecture.description;
            this.duration.Text = lecture.duration;
        }
    }
}