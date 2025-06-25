using Mono.Data.Sqlite;
using System.Data;
using UnityEngine;
using TMPro;
using EasyUI;
using EasyUI.Toast;
using System.Collections.Generic;

public class SQLiteHelper : MonoBehaviour
{

    public TMP_InputField courseNameInput, courseDescInput, courseStudent;
    public TMP_InputField teacherNameInput, teacherEmailInput, teacherPasswordInput;
    public TMP_InputField studentNameInput, studentEmailInput, studentPasswordInput;

    public TMP_Text courseTeacher;
    public TMP_Dropdown teacherDropdown;

    public GameObject coursePrefab;
    public Transform coursePanel;

    private List<GameObject> gameObjects = new List<GameObject>();
    private List<Course> courses = new List<Course>();

    private string dbName;

    public void ClearFields()
    {
        courseNameInput.text = "";
        courseDescInput.text = "";

        courseStudent.text = "";
        teacherNameInput.text = "";

        teacherEmailInput.text = "";
        teacherPasswordInput.text = "";

        studentNameInput.text = "";
        studentEmailInput.text = "";

        studentPasswordInput.text = "";
    }

    // Method to open a database connection
    private IDbConnection GetConnection()
    {
        //dbName = "URI=file:school.db";
        Debug.Log(Application.persistentDataPath);
        this.dbName = "URI=file:" + Application.persistentDataPath + "/school.db";
        return new SqliteConnection(dbName);
    }

     void Awake()
    {
        

    }

    void Start()
    {
        SQLiteHelper dbHelper = new SQLiteHelper();
        dbHelper.CreateTables(); // Create tables if they don't exist

        // Adding a teacher
       // AddTeacher("John Doe", "johndoe@email.com", "password123");

        // Adding a student
      //  AddStudent("Jane Smith", "janesmith@email.com", "password123", 1, 0, 85); // Attendance 1 (present), AssignmentStatus 0 (not submitted), Score 85
       // AddCourse("Mathematics 101", "Introduction to basic mathematical concepts.");
        //dbHelper.GetStudentsByCourse(1); // Get students assigned to course 1
    }

    // Create tables for Teachers, Students, and Courses
    public void CreateTables()
    {
        

        using (var connection = GetConnection())
        {
            connection.Open();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = @"
               CREATE TABLE IF NOT EXISTS Courses (
                CourseID INTEGER PRIMARY KEY AUTOINCREMENT,
                CourseName TEXT,
                CourseDescription TEXT,
                CourseTeacher TEXT,
                NumberOfStudents INTEGER,
                AssignmentPath TEXT
                );

                CREATE TABLE IF NOT EXISTS Teachers (
                    TeacherID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT,
                    Email TEXT,
                    Password TEXT,
                    Assignmenttext TEXT
                );

                CREATE TABLE IF NOT EXISTS Students (
                    StudentID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT,
                    Email TEXT,
                    Password TEXT,
                    Attendance INTEGER,
                    AssignmentStatus INTEGER,
                    Score INTEGER
                );

                CREATE TABLE IF NOT EXISTS Enrollments (
                    EnrollmentID INTEGER PRIMARY KEY AUTOINCREMENT,
                    TeacherID INTEGER,
                    StudentID INTEGER,
                    CourseID INTEGER,
                    Role INTEGER, -- 0 for Student, 1 for Teacher
                    FOREIGN KEY(TeacherID) REFERENCES Teachers(TeacherID),
                    FOREIGN KEY(StudentID) REFERENCES Students(StudentID),
                    FOREIGN KEY(CourseID) REFERENCES Courses(CourseID)
                );
            ";
            command.ExecuteNonQuery();
        }
    }

    // Method to add a new teacher
    public void AddTeacher(string name, string email, string password)
    {
        using (var connection = GetConnection())
        {
            connection.Open();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = $"INSERT INTO Teachers (Name, Email, Password) VALUES ('{name}', '{email}', '{password}')";
            command.ExecuteNonQuery();
        }

        ClearFields();
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

        ClearFields();
    }

    public void AddCourse()
    {
        AddCourse(courseNameInput.text, courseDescInput.text,courseTeacher.text, int.Parse(courseStudent.text),"");
        Toast.Show("Course Added succesfully");
        
    }

    public void AddTeacher()
    {
        AddTeacher(teacherNameInput.text, teacherEmailInput.text,teacherPasswordInput.text);
        Toast.Show(" Teacher Added succesfully");
    }

    public void AddStudent()
    {
        AddStudent(studentNameInput.text, studentEmailInput.text, studentPasswordInput.text,0,0,0);
        Toast.Show("Student Added succesfully");
    }

    public void AddCourse(string courseName, string courseDescription, string courseTeacher, int noOfStudents,string assignmentPath)
    {
        using (var connection = GetConnection())
        {
            connection.Open();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = $"INSERT INTO Courses (CourseName, CourseDescription,CourseTeacher,NumberOfStudents,AssignmentPath) VALUES ('{courseName}', '{courseDescription}' , '{courseTeacher}' , '{noOfStudents}','{assignmentPath}')";
            command.ExecuteNonQuery();
        }

        ClearFields();

       // sQLiteHelper.EnrollTeacherInCourse(teacherID, course.courseID);
    }

    // Example: Method to get a list of all students assigned to a course
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

    public void EnrollStudentInCourse(int studentID, int courseID)
    {
        using (var connection = GetConnection())
        {
            connection.Open();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = $"INSERT INTO Enrollments (StudentID, CourseID, Role) VALUES ({studentID}, {courseID}, 0)";
            command.ExecuteNonQuery();
        }
    }

    public void EnrollTeacherInCourse(int teacherID, int courseID)
    {
        using (var connection = GetConnection())
        {
            connection.Open();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = $"INSERT INTO Enrollments (TeacherID, CourseID, Role) VALUES ({teacherID}, {courseID}, 1)";
            command.ExecuteNonQuery();
        }
    }

    public List<Course> GetCourses()
    {
        List<Course> courses= new List<Course>();
        using (var connection = GetConnection())
        {
            connection.Open();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT CourseID, CourseName, CourseDescription,CourseTeacher,NumberOfStudents FROM Courses"; // The SQL query to get courses
            IDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Course course = new Course();

                course.courseID = int.Parse(reader["CourseID"].ToString());
                course.courseName = reader["CourseName"].ToString();
                course.courseDescription = reader["CourseDescription"].ToString();
                course.noOfStudents = reader["NumberOfStudents"].ToString();
                course.courseTeacher = reader["CourseTeacher"].ToString();
                courses.Add(course);
                //Debug.Log($"CourseID: {courseID}, Name: {courseName}, Description: {courseDescription}");
            }
        }

        return courses;
    }


    public void ShowCourses()
    {
        foreach(var v in gameObjects)
        {
            Destroy(v);
        }

        
        courses.Clear();

        courses = GetCourses();

        foreach(var v in courses)
        {
            GameObject gO = Instantiate(coursePrefab, coursePanel);
            Course c = gO.GetComponent<Course>();

            c.courseDescription = v.courseDescription;
            c.courseID = v.courseID;
            c.courseName= v.courseName;
            c.coursenameText.text = v.courseName;
            c.courseTeacher = v.courseTeacher;
            c.noOfStudents = v.noOfStudents;

            gameObjects.Add(gO);
        }

       
    }

    public string GetTeacherNameByCourseID(int courseID)
    {
        string teacherName = "Teacher not found"; // Default value if teacher is not found
        this.dbName = "URI=file:" + Application.persistentDataPath + "/school.db";
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            IDbCommand command = connection.CreateCommand();
            // Use the parameterized query to prevent SQL injection
            command.CommandText = "SELECT t.Name FROM Teachers t JOIN Enrollments e ON t.TeacherID = e.TeacherID WHERE e.CourseID = @CourseID AND e.Role = 1";

            // Use parameters to avoid SQL injection
            var courseParam = command.CreateParameter();
            courseParam.ParameterName = "@CourseID";
            courseParam.Value = courseID;
            command.Parameters.Add(courseParam);

            IDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                teacherName = reader["Name"].ToString(); // Get the teacher name
            }
        }

        return teacherName; // Return the teacher name or "Teacher not found" if no result
    }

    public void PopulateTeacherDropdown()
    {
        List<string> teacherNames = new List<string>();
        this.dbName = "URI=file:" + Application.persistentDataPath + "/school.db";
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT Name FROM Teachers"; // SQL query to fetch teacher names
            IDataReader reader = command.ExecuteReader();

            // Loop through the results and add teacher names to the list
            while (reader.Read())
            {
                teacherNames.Add(reader["Name"].ToString());
            }
        }

        // Clear the existing options in the dropdown
        teacherDropdown.ClearOptions();

        // Add the teacher names to the dropdown list
        teacherDropdown.AddOptions(teacherNames);
    }
}
