using UnityEngine;
using UnityEngine.UI;
using Mono.Data.Sqlite;
using System.Data;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class StudentLogin : MonoBehaviour
{
    //public TMP_Text studentNameText;
    public TMP_InputField emailInput;  // UI input field for email
    public TMP_InputField passwordInput;  // UI input field for password
    public TMP_Text feedbackText;  // UI text to show feedback (e.g., success or error message)
    private string dbName;

    // Function to check student login credentials

    private void Start()
    {
        
    }
    public void Login()
    {
        string email = emailInput.text;  // Get the email entered by the student
        string password = passwordInput.text;  // Get the password entered by the student

        // Call the function to check the login
        if (CheckLoginCredentials(email, password))
        {
            feedbackText.text = "Login Successful!";
            feedbackText.color = Color.green;

            DatabaseManager.instance.SetCurrentStudent(email);
            SceneManager.LoadScene("StudentPanel");

            // Proceed to the next scene or perform actions after successful login
        }
        else
        {
            feedbackText.text = "Invalid email or password!";
            feedbackText.color = Color.red;
        }
    }

    // Function to check the email and password from the Students table in the database
    public bool CheckLoginCredentials(string email, string password)
    {
        bool isValidLogin = false;
        this.dbName = "URI=file:" + Application.persistentDataPath + "/school.db";
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            IDbCommand command = connection.CreateCommand();
            // Query to check if the email and password match any student record
            command.CommandText = $"SELECT COUNT(*) FROM Students WHERE Email = @Email AND Password = @Password";

            // Use parameters to prevent SQL injection
            var emailParam = command.CreateParameter();
            emailParam.ParameterName = "@Email";
            emailParam.Value = email;
            command.Parameters.Add(emailParam);

            var passwordParam = command.CreateParameter();
            passwordParam.ParameterName = "@Password";
            passwordParam.Value = password;
            command.Parameters.Add(passwordParam);

            int count = Convert.ToInt32(command.ExecuteScalar()); // Execute query and get the count of matching records

            // If count > 0, the credentials are correct
            if (count > 0)
            {
                isValidLogin = true;
            }
        }

        return isValidLogin;
    }

    
}
