using System;
using System.Collections.Generic;

namespace ProcessStydingData
{
    public class Course
    {
        public int id;
        public string title;
        public string description;

        public string professor;
        /*    public List<string> professors; */

        public string lectures;
        /*  public List<Lecture> lectures; */
        public long amountOfSubscribers;
        public double rating;
        public bool isPrivate;
        public double price;
    }
}
