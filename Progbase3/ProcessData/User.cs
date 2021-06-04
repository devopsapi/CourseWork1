using System;

namespace ProcessData
{
    public class User
    {
        public int id;
        public string username;
        public string password;
        public string fullname;
        public DateTime createdAt;
        public Course[] userIsAuthorOfCourses;
        public bool imported = false;

        public User()
        {
            this.createdAt = DateTime.Now;
        }
        public User(int id, string username, string password, string fullname, DateTime createdAt)
        {
            this.id = id;
            this.username = username;
            this.password = password;
            this.fullname = fullname;
            this.createdAt = createdAt;
        }

        public override string ToString()
        {
            return $"{id}) username '{username}' - {fullname}";
        }
    }
}