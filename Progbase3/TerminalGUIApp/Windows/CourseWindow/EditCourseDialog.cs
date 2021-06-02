using System.Collections.Generic;
using ProcessData;
using Terminal.Gui;

namespace TerminalGUIApp
{
    public class EditCourseDialog : CreateCourseDialog
    {
        protected Course currentCourse;
        public EditCourseDialog()
        {
            this.Title = "Edit course";
           
            this.allLectures.Visible = false;
        }

        public void SetCourse(Course course)
        {
            this.titleInput.Text = course.title;
            this.descriptionInput.Text = course.description;
            this.authorInput.Text = course.author;
            this.priceInput.Text = course.price.ToString();
            this.isPrivateCheckBox.Checked = course.isPrivate;

            this.currentCourse = course;
        }

        public new Course GetCourse()
        {
            double tryParsePrice;

            if (titleInput.Text.ToString() == "" || descriptionInput.Text.ToString() == "" || authorInput.Text.ToString() == "")
            {
                MessageBox.ErrorQuery("Course", "All fields must be filled", "OK");
                return this.currentCourse;
            }

            if (!double.TryParse(priceInput.Text.ToString(), out tryParsePrice) || tryParsePrice < 0)
            {
                MessageBox.ErrorQuery("Creating course", "Incorrect price value. Must be non-negative integer", "Ok");
                return this.currentCourse;
            }

            Course course = new Course();

            course.title = titleInput.Text.ToString();
            course.description = descriptionInput.Text.ToString();
            course.author = authorInput.Text.ToString();
            course.price = int.Parse(priceInput.Text.ToString());
            course.isPrivate = isPrivateCheckBox.Checked;
            course.publishedAt = this.currentCourse.publishedAt;
            course.amountOfSubscribers = this.currentCourse.amountOfSubscribers;
            course.rating = this.currentCourse.rating;
            course.userId = this.currentCourse.userId;

            return course;
        }
    }
}