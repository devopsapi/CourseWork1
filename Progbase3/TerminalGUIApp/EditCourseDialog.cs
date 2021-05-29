using ProcessData;
using Terminal.Gui;

namespace TerminalGUIApp
{
    public class EditCourseDialog : CreateCourseDialog
    {
        public EditCourseDialog()
        {
            this.Title = "Edit course";
        }

        public void SetCourse(Course course)
        {
            this.titleInput.Text = course.title.ToString();
            this.descriptionInput.Text = course.description.ToString();
            this.subscribers.Text = course.amountOfSubscribers.ToString();
            this.authorInput.Text = course.author.ToString();
            this.priceInput.Text = course.price.ToString();
            this.courseCreatedAtDateField.Text = course.publishedAt.ToString();
            this.rating.Text = course.rating.ToString();
            this.isPrivateCheckBox.Checked = course.isPrivate;
            this.courseUserIdInput.Text = course.userId.ToString();

        }

    }
}