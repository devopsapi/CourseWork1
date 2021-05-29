using System;
using System.Collections.Generic;

namespace ProcessData
{
    public class Course
    {
        public int id;
        public string title;
        public string description;
        public string author;
        public int amountOfSubscribers;
        public double rating;
        public bool isPrivate;
        public double price;
        public DateTime publishedAt;
        public Lecture[] lectures;
        public Users_Courses[] usersAndCourses;
        public int userId;
        public User user;

        public override string ToString()
        {
            return $" {this.title} ";
        }
    }
}
