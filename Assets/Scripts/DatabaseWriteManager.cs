using System.IO;
using UnityEngine;

public class DatabaseWriteManager : MonoBehaviour
{
    private string usersFilePath;
    private string coursesFilePath;
    private string enrollmentsFilePath;

    void Start()
    {
        // Define file paths (these are saved in the persistent data path of the device)
        usersFilePath = Path.Combine(Application.persistentDataPath, "users.csv");
        coursesFilePath = Path.Combine(Application.persistentDataPath, "courses.csv");
        enrollmentsFilePath = Path.Combine(Application.persistentDataPath, "enrollments.csv");

        // Write data to the CSV files
        WriteUserData();
        WriteCourseData();
        WriteEnrollmentData();
    }

    // Write data to users.csv
    void WriteUserData()
    {
        if (!File.Exists(usersFilePath))
        {
            using (StreamWriter writer = new StreamWriter(usersFilePath))
            {
                writer.WriteLine("Id,Name,Email,Password,UserType"); // Headers
                writer.WriteLine("1,John Doe,john@example.com,hashed_password,Teacher");
                writer.WriteLine("2,Jane Smith,jane@example.com,hashed_password,Student");
            }
        }
    }

    // Write data to courses.csv
    void WriteCourseData()
    {
        if (!File.Exists(coursesFilePath))
        {
            using (StreamWriter writer = new StreamWriter(coursesFilePath))
            {
                writer.WriteLine("Id,CourseName,CourseDescription"); // Headers
                writer.WriteLine("1,Math 101,An introductory course to Mathematics");
                writer.WriteLine("2,Science 101,An introductory course to Science");
            }
        }
    }

    // Write data to enrollments.csv
    void WriteEnrollmentData()
    {
        if (!File.Exists(enrollmentsFilePath))
        {
            using (StreamWriter writer = new StreamWriter(enrollmentsFilePath))
            {
                writer.WriteLine("Id,CourseId,TeacherId,StudentId"); // Headers
                writer.WriteLine("1,1,1,2");
            }
        }
    }
}
