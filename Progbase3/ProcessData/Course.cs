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
        public Users_Courses[] users_courses;
        public int user_id;
        public User user;

    }
}
