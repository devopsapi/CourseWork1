using System;

namespace ProcessData
{
    public class Lecture
    {
        public int id;
        public Course course;
        public string topic;
        public string description;
        public string duration;
        public int courseId;
        public DateTime createdAt;

        public override string ToString()
        {
            return $"{this.topic}";
        }
    }
}