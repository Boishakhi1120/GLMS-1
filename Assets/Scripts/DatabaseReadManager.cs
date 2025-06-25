using System.IO;
using UnityEngine;

public class DatabaseReadManager : MonoBehaviour
{
    private string usersFilePath;
    private string coursesFilePath;
    private string enrollmentsFilePath;

    void Start()
    {
        // Define file paths
        usersFilePath = Path.Combine(Application.persistentDataPath, "users.csv");
        coursesFilePath = Path.Combine(Application.persistentDataPath, "courses.csv");
        enrollmentsFilePath = Path.Combine(Application.persistentDataPath, "enrollments.csv");

        // Read and display data
        ReadUserData();
        ReadCourseData();
        ReadEnrollmentData();
    }

    // Read and display users data from users.csv
    void ReadUserData()
    {
        if (File.Exists(usersFilePath))
        {
            using (StreamReader reader = new StreamReader(usersFilePath))
            {
                reader.ReadLine(); // Skip header row
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] values = line.Split(',');
                    int id = int.Parse(values[0]);
                    string name = values[1];
                    string email = values[2];
                    string password = values[3];
                    string userType = values[4];

                    Debug.Log($"User: {name}, Email: {email}, Type: {userType}");
                }
            }
        }
    }

    // Read and display courses data from courses.csv
    void ReadCourseData()
    {
        if (File.Exists(coursesFilePath))
        {
            using (StreamReader reader = new StreamReader(coursesFilePath))
            {
                reader.ReadLine(); // Skip header row
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] values = line.Split(',');
                    int id = int.Parse(values[0]);
                    string courseName = values[1];
                    string courseDescription = values[2];

                    Debug.Log($"Course: {courseName}, Description: {courseDescription}");
                }
            }
        }
    }

    // Read and display enrollments data from enrollments.csv
    void ReadEnrollmentData()
    {
        if (File.Exists(enrollmentsFilePath))
        {
            using (StreamReader reader = new StreamReader(enrollmentsFilePath))
            {
                reader.ReadLine(); // Skip header row
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] values = line.Split(',');
                    int id = int.Parse(values[0]);
                    int courseId = int.Parse(values[1]);
                    int teacherId = int.Parse(values[2]);
                    int studentId = int.Parse(values[3]);

                    Debug.Log($"Enrollment: CourseId {courseId}, TeacherId {teacherId}, StudentId {studentId}");
                }
            }
        }
    }
}
