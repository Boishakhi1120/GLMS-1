using UnityEngine;
using UnityEngine.UI;
using Mono.Data.Sqlite;
using System.Data;
using System.Collections.Generic;
using TMPro;
using System;

public class TeacherDropdown : MonoBehaviour
{
    public TMP_Dropdown teacherDropdown; // Reference to the Dropdown UI element
    private  string dbName; // Update with the actual database location if needed
    public TMP_Text teacherDropdowntext;
    public TMP_Text courseNameText;
    private string teacher;
    public SQLiteHelper sQLiteHelper;
    public Course course;

    // Start is called before the first frame update

    private void Awake()
    {
        sQLiteHelper =  GameObject.FindAnyObjectByType<SQLiteHelper>();
       
    }
    void Start()
    {
       // PopulateTeacherDropdown();
    }

    public void SelectTeacher()
    {
        teacher = teacherDropdowntext.text;
        int teacherID = GetTeacherIDByName(teacher);
       // int courseID = GetCourseIDByName(course.courseName);
        
    }

   


    private int GetCourseIDByName(string courseName)
    {
        int teacherID = -1; // Default value if teacher is not found

        this.dbName = "URI=file:" + Application.persistentDataPath + "/school.db";
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            IDbCommand command = connection.CreateCommand();
            // SQL query to fetch TeacherID by Name
            command.CommandText = $"SELECT CourseID FROM Courses WHERE Name = '{courseName}'";
            IDataReader reader = command.ExecuteReader();

            // Check if a row was returned
            if (reader.Read())
            {
                // Get TeacherID from the result
                teacherID = int.Parse(reader["CourseID"].ToString());
            }
        }

        return teacherID; // Return the TeacherID or -1 if not found
    }

    public int GetTeacherIDByName(string teacherName)
    {
        int teacherID = -1; // Default value if teacher is not found

        this.dbName = "URI=file:" + Application.persistentDataPath + "/school.db";
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            IDbCommand command = connection.CreateCommand();
            // SQL query to fetch TeacherID by Name
            command.CommandText = $"SELECT TeacherID FROM Teachers WHERE Name = '{teacherName}'";
            IDataReader reader = command.ExecuteReader();

            // Check if a row was returned
            if (reader.Read())
            {
                // Get TeacherID from the result
                teacherID = int.Parse(reader["TeacherID"].ToString());
            }
        }

        return teacherID; // Return the TeacherID or -1 if not found
    }


    // Function to populate the Dropdown menu with teacher names from the database
   
}
