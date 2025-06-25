using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UserManager : MonoBehaviour
{
    private string filePath;
    public string database_name;
    public int loadscene;
    public TMP_Text logText;
    
    public TMP_InputField user_name,email, password;

    // Start is called before the first frame update
    void Start()
    {
       Debug.Log(Application.persistentDataPath);
        // Define the file path where user data will be stored
        filePath = Application.persistentDataPath + database_name;

        // Ensure the file exists
        if (!File.Exists(filePath))
        {
            File.Create(filePath).Close();
        }
    }

    public void OnSignUpButtonClick()
    {
        AddUser(email.text, user_name.text, password.text);
    }

    public void OnLoginButtonClick()
    {
        if (VerifyUser(email.text, password.text)) 
        {
            PlayerPrefs.SetString(StudentData.STUDENT_EMAIL, email.text);
            PlayerPrefs.SetString(StudentData.STUDENT_NAME, user_name.text);
            SceneManager.LoadScene(loadscene);
        }
    }


    // Method to add a new user
    public void AddUser(string email,string name, string password)
    {
        string hashedPassword = HashPassword(password);
        string userData = email + ":" + name + ":" + hashedPassword;

        // Write user data to the file
        File.AppendAllText(filePath, userData + Environment.NewLine);
        Debug.Log("User added!");
    }

    // Method to verify if a user exists
    public bool VerifyUser(string email, string password)
    {
        string hashedPassword = HashPassword(password);
        string[] lines = File.ReadAllLines(filePath);

        foreach (string line in lines)
        {
            string[] userData = line.Split(':');
            if (userData.Length == 3 && userData[0] == email && userData[2] == hashedPassword)
            {
                Debug.Log("User exists and password matches");
                return true; // User exists and password matches
            }
        }

        Debug.Log("User not found or incorrect password");
        return false; // User not found or incorrect password
    }

    // Method to hash passwords using SHA256
    private string HashPassword(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                stringBuilder.Append(b.ToString("x2"));
            }
            return stringBuilder.ToString();
        }
    }
}
