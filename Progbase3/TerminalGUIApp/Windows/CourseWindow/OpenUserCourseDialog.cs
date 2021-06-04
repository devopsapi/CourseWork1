using System.Collections.Generic;
using ProcessData;
using Terminal.Gui;

namespace TerminalGUIApp.Windows.CourseWindow
{
    public class OpenUserCourseDialog : OpenCourseDialog
    {
        public bool deleted;
        public bool edited;
        public OpenUserCourseDialog()
        {
            Button editBtn = new Button(60, 4, "Edit");
            editBtn.Clicked += OnEditButtonClicked;
            this.AddButton(editBtn);

            Button deleteBtn = new Button(70, 4, "Delete");
            deleteBtn.Clicked += OnDeleteBtn;
            this.AddButton(deleteBtn);

            this.canBeEditedAndDeleted = true;
        }

        private void OnDeleteBtn()
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
            EditCourseDialog dialog = new EditCourseDialog();

            dialog.SetCourse(this.course);
            dialog.justCreated = false;

            Application.Run(dialog);

            if (!dialog.canceled)
            {
                Course editedCourse = dialog.GetCourse();
                this.courseRepository.Update(course.id, editedCourse);
                this.edited = true;
                this.SetCourse(editedCourse);

            }
        }
    }
}
