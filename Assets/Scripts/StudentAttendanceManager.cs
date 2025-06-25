using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using System.Data;
using TMPro;

public class StudentAttendanceManager : MonoBehaviour
{
    public GameObject contentPanel; // The panel that will hold the student list
    public GameObject studentPrefab; // Prefab for a single student entry (Text + Checkbox)
    private string dbName = "URI=file:school.db"; // Update with your database location

    // Start is called before the first frame update
    void Start()
    {
        LoadStudents();
    }

    private IDbConnection GetConnection()
    {
        //dbName = "URI=file:school.db";
        Debug.Log(Application.persistentDataPath);
        this.dbName = "URI=file:" + Application.persistentDataPath + "/school.db";
        return new SqliteConnection(dbName);

    }
        // Function to load students from the database and display them
        public void LoadStudents()
    {
        // Clear existing student entries in the panel
        foreach (Transform child in contentPanel.transform)
        {
            Destroy(child.gameObject);
        }

        using (var connection = GetConnection())
        {
            connection.Open();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT StudentID, Name, Attendance, Score FROM Students"; // Fetch all students
            IDataReader reader = command.ExecuteReader();

            // Loop through the results and create UI elements for each student
            while (reader.Read())
            {
                int studentID = int.Parse(reader["StudentID"].ToString());
                string studentName = reader["Name"].ToString();
                bool isPresent = reader["Attendance"].ToString() == "1"; // Convert attendance (1 = present, 0 = absent)
                int score = int.Parse(reader["Score"].ToString());

                // Instantiate a new student UI element (Text + Checkbox)
                GameObject studentEntry = Instantiate(studentPrefab);
                studentEntry.transform.SetParent(contentPanel.transform, false);

                // Set the student name
                TMP_Text[] textComponents = studentEntry.GetComponentsInChildren<TMP_Text>();
                textComponents[0].text = studentName; // First text is for the student name

                // Set the checkbox state
                Toggle checkbox = studentEntry.GetComponentInChildren<Toggle>();
                checkbox.isOn = false;
                // Add a listener to update the attendance when the checkbox is clicked
                checkbox.onValueChanged.AddListener((isChecked) => UpdateAttendance(studentID, isChecked, score ));

            }
        }
    }

    // Function to update attendance in the database based on checkbox status
    public void UpdateAttendance(int studentID, bool isPresent, int score)
    {
        using (var connection = GetConnection())
        {
            connection.Open();
            IDbCommand command = connection.CreateCommand();
            // Update the attendance value (1 = present, 0 = absent)
            command.CommandText = $"UPDATE Students SET Attendance = {(isPresent ? 1 : 0)} , Score = @Score WHERE StudentID = {studentID}";

            var scoreParam = command.CreateParameter();
            scoreParam.ParameterName = "@Score";
            scoreParam.Value = score + 50;
            command.Parameters.Add(scoreParam);

            command.ExecuteNonQuery();
        }
    }
}
