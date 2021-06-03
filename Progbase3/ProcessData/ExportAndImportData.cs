using System;
using System.IO.Compression;
using System.Collections.Generic;

using ProcessXml;

namespace ProcessData
{
    public static class ExportAndImportData
    {
        private static string coursesFileName = "courses.xml";
        private static string lecturesFileName = "lectures.xml";


        public static bool ExportCourseWithLectures(string saveDirPath, string zipName, CourseRepository courseRepository, LectureRepository lectureRepository)
        {
            Course[] courses = courseRepository.GetAll();

            for (int i = 0; i < courses.Length; i++)
            {
                courses[i] = courseRepository.GetCourseWithAllLectures(courses[i].id);
            }

            Lecture[] lectures = lectureRepository.GetAll();

            string tempSavePath = $"{saveDirPath}/tempExportSave";

            string coursesFilePath = $"{tempSavePath}/{coursesFileName}";
            string lecturesFilePath = $"{tempSavePath}/{lecturesFileName}";

            try
            {
                System.IO.Directory.CreateDirectory(tempSavePath);

                XmlSerialization.Serialize<Course[]>(courses, coursesFilePath);

                XmlSerialization.Serialize<Lecture[]>(lectures, lecturesFilePath);

                ZipFile.CreateFromDirectory(tempSavePath, $"{saveDirPath}/{zipName}");

                System.IO.File.Delete(coursesFilePath);
                System.IO.File.Delete(lecturesFilePath);
                System.IO.Directory.Delete(tempSavePath);
            }
            catch
            {
                if (System.IO.File.Exists(coursesFilePath))
                {
                    System.IO.File.Delete(coursesFilePath);
                }
                else if (System.IO.File.Exists(lecturesFilePath))
                {
                    System.IO.File.Delete(lecturesFilePath);
                }
                else if (System.IO.Directory.Exists(tempSavePath))
                {
                    System.IO.Directory.Delete(tempSavePath);
                }

                return false;
            }

            return true;
        }



        public static bool ImportPostsWithComments(string zipPath, UserRepository userRepository, CourseRepository courseRepository, LectureRepository lectureRepository)
        {
            string saveDirPath = GetDirPath(zipPath);
            string tempSaveDirPath = $"{saveDirPath}/tempImportSave";

            string tempPostsPath = $"{tempSaveDirPath}/{coursesFileName}";
            string tempCommentsPath = $"{tempSaveDirPath}/{lecturesFileName}";

            Course[] courses = null;
            Lecture[] lectures = null;

            try
            {
                System.IO.Directory.CreateDirectory(tempSaveDirPath);
                ZipFile.ExtractToDirectory(zipPath, tempSaveDirPath);

                XmlSerialization.Deserialize<Course[]>(ref courses, tempPostsPath);

                XmlSerialization.Deserialize<Lecture[]>(ref lectures, tempCommentsPath);

                System.IO.File.Delete(tempPostsPath);
                System.IO.File.Delete(tempCommentsPath);
                System.IO.Directory.Delete(tempSaveDirPath);
            }

            catch
            {
                if (System.IO.File.Exists(tempPostsPath))
                {
                    System.IO.File.Delete(tempPostsPath);
                }
                else if (System.IO.File.Exists(tempCommentsPath))
                {
                    System.IO.File.Delete(tempCommentsPath);
                }
                else if (System.IO.Directory.Exists(tempSaveDirPath))
                {
                    System.IO.Directory.Delete(tempSaveDirPath);
                }

                return false;
            }
            for (int i = 0; i < courses.Length; i++)
            {
                if (!courseRepository.CourseExist(courses[i].id))
                {
                    if (!userRepository.UserExistsById(courses[i].id))
                    {
                        User user = new User
                        {
                            id = courses[i].userId,
                            username = $"user{courses[i].userId}",
                            password = $"passuser{courses[i].userId}",
                            fullname = "",
                            createdAt = DateTime.Now,
                            imported = true,
                        };

                        userRepository.InsertImport(user);
                    }

                    courseRepository.InsertImport(courses[i]);
                }
            }


            for (int i = 0; i < lectures.Length; i++)
            {
                if (!lectureRepository.LectureExists(lectures[i].id))
                {

                    User user = new User
                    {
                        id = courses[i].userId,
                        username = $"user{courses[i].userId}",
                        password = $"passuser{courses[i].userId}",
                        fullname = "",
                        createdAt = DateTime.Now,
                        imported = true,
                    };

                    userRepository.InsertImport(user);

                    lectureRepository.InsertImport(lectures[i]);
                }
            }

            return true;
        }


        private static string GetDirPath(string fullPath)
        {
            string[] fullPathMass = fullPath.Split('/');

            string dirPath = String.Join('/', fullPathMass, 0, fullPathMass.Length - 1);

            return dirPath;
        }
    }
}
