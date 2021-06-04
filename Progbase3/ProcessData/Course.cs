using System;

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
        public int userId;

        public override string ToString()
        {
            return $" {this.title} ";
        }
    }
}
