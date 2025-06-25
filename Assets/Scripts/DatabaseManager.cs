using Mono.Data.Sqlite;
using NUnit.Framework.Internal;
using System;
using System.Data;
using System.Windows.Forms;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Application = UnityEngine.Application;

public class DatabaseManager : MonoBehaviour 
{

    public static DatabaseManager instance;
    public string dbName;
    public Student currentStudent;
    public Teacher currentTeacher;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(gameObject);
        PlayerPrefs.DeleteAll();
    }

    private IDbConnection GetConnection()
    {
        //dbName = "URI=file:school.db";
        Debug.Log(Application.persistentDataPath);
        string connectingString = "URI=file:" + Application.persistentDataPath + dbName;
        return new SqliteConnection(connectingString);
    }

    public void AddTeacher(string name, string email, string password)
    {
        using (var connection = GetConnection())
        {
            connection.Open();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = $"INSERT INTO Teachers (Name, Email, Password) VALUES ('{name}', '{email}', '{password}')";
            command.ExecuteNonQuery();
        }
    }

    // Method to add a new student
    public void AddStudent(string name, string email, string password, int attendance, int assignmentStatus, int score)
    {
        using (var connection = GetConnection())
        {
            connection.Open();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = $"INSERT INTO Students (Name, Email, Password, Attendance, AssignmentStatus, Score) VALUES ('{name}', '{email}', '{password}', {attendance}, {assignmentStatus}, {score})";
            command.ExecuteNonQuery();
        }
    }

    public void AddCourse(string courseName, string courseDescription, string courseTeacher, int noOfStudents)
    {
        using (var connection = GetConnection())
        {
            connection.Open();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = $"INSERT INTO Courses (CourseName, CourseDescription,CourseTeacher,NumberOfStudents) VALUES ('{courseName}', '{courseDescription}' , '{courseTeacher}' , '{noOfStudents}')";
            command.ExecuteNonQuery();
        }

        // sQLiteHelper.EnrollTeacherInCourse(teacherID, course.courseID);
    }

    public void GetStudentsByCourse(int courseID)
    {
        using (var connection = GetConnection())
        {
            connection.Open();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = $"SELECT * FROM Students WHERE AssignedCourseID = {courseID}";
            IDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Debug.Log($"Student ID: {reader["StudentID"]}, Name: {reader["Name"]}, Attendance: {reader["Attendance"]}, Assignment Status: {reader["AssignmentStatus"]}");
            }
        }
    }

    public int GetTeacherIDByTeacherName(string teacherName)
    {
        int teacherID = -1; // Default value if teacher not found

        using (var connection = GetConnection())
        {
            connection.Open();
            IDbCommand command = connection.CreateCommand();
            // Parameterized query to get TeacherID by TeacherName
            command.CommandText = "SELECT TeacherID FROM Teachers WHERE Name = @TeacherName";

            // Use parameters to prevent SQL injection
            var teacherNameParam = command.CreateParameter();
            teacherNameParam.ParameterName = "@TeacherName";
            teacherNameParam.Value = teacherName;
            command.Parameters.Add(teacherNameParam);

            IDataReader reader = command.ExecuteReader();

            // Check if a result was returned
            if (reader.Read())
            {
                teacherID = int.Parse(reader["TeacherID"].ToString()); // Get TeacherID
            }
        }

        return teacherID; // Return the TeacherID or -1 if not found
    }

    public void EnrollStudentInCourse(int teacherID, int studentID, int courseID)
    {
        using (var connection = GetConnection())
        {
            connection.Open();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = $"INSERT INTO Enrollments (TeacherID, StudentID, CourseID, Role) VALUES ({teacherID},{studentID}, {courseID}, 0)";
            command.ExecuteNonQuery();
        }
    }

    public void SetCurrentStudent(string email)
    {
        Student student = GetStudentByEmail(email);
        currentStudent = student;
    }

    public Student GetStudentByEmail(string email)
    {
        using (var connection = GetConnection())
        {
            connection.Open();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT StudentID, Name, Email, Attendance, AssignmentStatus, Score FROM Students WHERE Email = @Email";

            // Use parameters to prevent SQL injection
            var emailParam = command.CreateParameter();
            emailParam.ParameterName = "@Email";
            emailParam.Value = email;
            command.Parameters.Add(emailParam);

            IDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                Student student = new Student();
                student.StuentID = int.Parse(reader["StudentID"].ToString());
                student.StudentName = reader["Name"].ToString();
                student.StudentEmail = reader["Email"].ToString();
                student.StudentAttendance = int.Parse(reader["Attendance"].ToString());
                student.StudentAssignment = int.Parse(reader["AssignmentStatus"].ToString());
                student.StudentScore = int.Parse(reader["Score"].ToString());
                return student;
                //Debug.Log($"Student Found: ID: {studentID}, Name: {studentName}, Email: {studentEmail}, Attendance: {attendance}, AssignmentStatus: {assignmentStatus}, Score: {score}");

            }
            else
            {
                Debug.Log("Student not found.");
                return null;
               
            }
        }
    }


    public Student GetCurrentStudent()
    {
        return currentStudent;
    }

    public Teacher GetCurrentTeacher()
    {
        return currentTeacher;
    }

    public bool CheckEnrollmentExists(int studentID, int courseID)
    {
        bool exists = false;

        using (var connection = GetConnection())
        {
            connection.Open();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM Enrollments WHERE StudentID = @StudentID AND CourseID = @CourseID";

            // Use parameters to prevent SQL injection
            var studentIdParam = command.CreateParameter();
            studentIdParam.ParameterName = "@StudentID";
            studentIdParam.Value = studentID;
            command.Parameters.Add(studentIdParam);

            var courseIdParam = command.CreateParameter();
            courseIdParam.ParameterName = "@CourseID";
            courseIdParam.Value = courseID;
            command.Parameters.Add(courseIdParam);

            int count = Convert.ToInt32(command.ExecuteScalar());

            if (count > 0)
            {
                exists = true;
            }
        }

        return exists;
    }

    public void SetCurrentTeacher(string email)
    {
        Teacher teacher = GetTeacherByEmail(email);

        PlayerPrefs.SetString(PlayerPrefData.TEACHER_NAME, teacher.TeacherName);
        PlayerPrefs.SetString(PlayerPrefData.TEACHER_EMAIL, teacher.TeacherEmail);
        PlayerPrefs.SetString(PlayerPrefData.TEACHER_ASSIGNTEXT, teacher.TeacherAssignmentText);
      


        currentTeacher = teacher;
    }

    public Teacher GetTeacherByEmail(string email)
    {
        using (var connection = GetConnection())
        {
            connection.Open();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT TeacherID, Name, Email, Assignmenttext FROM Teachers WHERE Email = @Email";

            // Use parameters to prevent SQL injection
            var emailParam = command.CreateParameter();
            emailParam.ParameterName = "@Email";
            emailParam.Value = email;
            command.Parameters.Add(emailParam);

            IDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                Teacher teacher = new Teacher();
                teacher.TeacherID = int.Parse(reader["TeacherID"].ToString());
                teacher.TeacherName = reader["Name"].ToString();
                teacher.TeacherEmail = reader["Email"].ToString();
                teacher.TeacherAssignmentText = reader["Assignmenttext"].ToString();
                
                return teacher;
                //Debug.Log($"Student Found: ID: {studentID}, Name: {studentName}, Email: {studentEmail}, Attendance: {attendance}, AssignmentStatus: {assignmentStatus}, Score: {score}");

            }
            else
            {
                Debug.Log("Teacher not found.");
                return null;

            }
        }
    }

    public string GetAssignmentPath(int studentID)
    {
        ;

        int courseID = -1; // Default value if teacher not found

        using (var connection = GetConnection())
        {
            connection.Open();
            IDbCommand command = connection.CreateCommand();
            // Parameterized query to get TeacherID by TeacherName
            command.CommandText = "SELECT CourseID FROM Enrollments WHERE StudentID = @StudentID";

            // Use parameters to prevent SQL injection
            var teacherNameParam = command.CreateParameter();
            teacherNameParam.ParameterName = "@StudentID";
            teacherNameParam.Value = studentID;
            command.Parameters.Add(teacherNameParam);

            IDataReader reader = command.ExecuteReader();

            // Check if a result was returned
            if (reader.Read())
            {
                courseID = int.Parse(reader["CourseID"].ToString()); // Get TeacherID
            }
        }

        using (var connection = GetConnection())
        {
            string assignmentPath = "";
            connection.Open();
            IDbCommand command = connection.CreateCommand();
            // Parameterized query to get TeacherID by TeacherName
            command.CommandText = "SELECT AssignmentPath FROM Courses WHERE CourseID = @CouseID";

            // Use parameters to prevent SQL injection
            var teacherNameParam = command.CreateParameter();
            teacherNameParam.ParameterName = "@CouseID";
            teacherNameParam.Value = courseID;
            command.Parameters.Add(teacherNameParam);

            IDataReader reader = command.ExecuteReader();

            // Check if a result was returned
            if (reader.Read())
            {
                assignmentPath = reader["AssignmentPath"].ToString(); // Get TeacherID
            }

            return assignmentPath;
        }

        //SELECT AssignmentPath FROM Course WHERE CourseID = [YourCourseID];



    }

    public void UpdateCourseAssignmentPathByTeacherName(string teacherName, string assignmentPath)
    {
        using (var connection = GetConnection())
        {
            connection.Open();
            IDbCommand command = connection.CreateCommand();

            command.CommandText = "UPDATE Courses SET AssignmentPath = @AssignmentPath WHERE CourseTeacher = @CourseTeacher";

            var assignmentPathParam = command.CreateParameter();
            assignmentPathParam.ParameterName = "@AssignmentPath";
            assignmentPathParam.Value = assignmentPath;
            command.Parameters.Add(assignmentPathParam);

            var CourseTeacher = command.CreateParameter();
            CourseTeacher.ParameterName = "@CourseTeacher";
            CourseTeacher.Value = teacherName;
            command.Parameters.Add(CourseTeacher);

            command.ExecuteNonQuery();
        }
    }

    public void UpdateCourseAssignmentTextByTeacherName(string teacherName, string assignmentText)
    {
        using (var connection = GetConnection())
        {
            connection.Open();
            IDbCommand command = connection.CreateCommand();

            command.CommandText = "UPDATE Teachers SET Assignmenttext = @Assignmenttext WHERE Name = @Name";

            var Assignmenttext = command.CreateParameter();
            Assignmenttext.ParameterName = "@Assignmenttext";
            Assignmenttext.Value = assignmentText;
            command.Parameters.Add(Assignmenttext);

            var Name = command.CreateParameter();
            Name.ParameterName = "@Name";
            Name.Value = teacherName;
            command.Parameters.Add(Name);

            command.ExecuteNonQuery();
            PlayerPrefs.SetString(PlayerPrefData.TEACHER_ASSIGNTEXT,assignmentText);
        }
    }

    public string GetCourseAssignmentPath()
    {
        using (var connection = GetConnection())
        {
            connection.Open();
            IDbCommand command = connection.CreateCommand();

            command.CommandText = "SELECT AssignmentPath FROM Courses WHERE CourseID = 1";



            IDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                string s = reader["AssignmentPath"].ToString();
              

                return s;
                //Debug.Log($"Student Found: ID: {studentID}, Name: {studentName}, Email: {studentEmail}, Attendance: {attendance}, AssignmentStatus: {assignmentStatus}, Score: {score}");

            }
            else
            {
                return null;
            }

            
        }
    }

    public void UpdateStudentScoreByEmail(string email, int score)
    {
        using (var connection = GetConnection())
        {
            connection.Open();
            IDbCommand command = connection.CreateCommand();

            command.CommandText = "UPDATE Students SET Score = @Score WHERE Email = @Email";

            var scoreParam = command.CreateParameter();
            scoreParam.ParameterName = "@Score";
            scoreParam.Value = score;
            command.Parameters.Add(scoreParam);

            var emailParam = command.CreateParameter();
            emailParam.ParameterName = "@Email";
            emailParam.Value = email;
            command.Parameters.Add(emailParam);

            command.ExecuteNonQuery();
        }
    }


}
