using UnityEngine;
using UnityEngine.UI;
using Mono.Data.Sqlite;
using System.Data;
using System.Collections.Generic;
using TMPro;

public class LeaderBoard : MonoBehaviour
{
    public GameObject contentPanel; // The panel that will hold the student list
    public GameObject studentPrefab; // Prefab for a single student entry (Text for rank, name, and score)
    private string dbName = "URI=file:school.db"; // Database connection string

    private List<GameObject> gameObjects = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        LoadStudentScores();
    }

    private IDbConnection GetConnection()
    {
        //dbName = "URI=file:school.db";
        Debug.Log(Application.persistentDataPath);
        string connectingString = "URI=file:" + Application.persistentDataPath + "/school.db";
        return new SqliteConnection(connectingString);
    }

    // Function to load student names and scores from the database
    public void LoadStudentScores()
    {
        // Clear existing student entries in the panel
        foreach (Transform child in contentPanel.transform)
        {
            Destroy(child.gameObject);
        }

        if (gameObjects != null)
        {
            foreach (var v in gameObjects)
            {
                Destroy(v.gameObject);
            }
        }
        

        List<Student> students = new List<Student>();

        using (var connection = GetConnection())
        {
            connection.Open();
            IDbCommand command = connection.CreateCommand();
            // Get student names and scores, ordered by score from highest to lowest
            command.CommandText = "SELECT Name, Score FROM Students ORDER BY Score DESC";
            IDataReader reader = command.ExecuteReader();

            // Loop through the results and add student data to the list
            while (reader.Read())
            {
                string studentName = reader["Name"].ToString();
                int score = int.Parse(reader["Score"].ToString());
                students.Add(new Student { StudentName = studentName, StudentScore = score });
            }
        }

        // Display students in the UI
        int rank = 1;
        foreach (Student student in students)
        {
            // Instantiate a new student UI element (Rank, Name, Score)
            GameObject studentEntry = Instantiate(studentPrefab);
            studentEntry.transform.SetParent(contentPanel.transform, false);

            TMP_Text[] textComponents = studentEntry.GetComponentsInChildren<TMP_Text>();
            textComponents[0].text = rank.ToString(); // First text is for the rank
            textComponents[1].text = student.StudentName;  // Second text is for the student name
            textComponents[2].text = student.StudentScore.ToString(); // Third text is for the score

            rank++;
            gameObjects.Add(studentEntry);
        }
    }
}

// Class to hold student data

