using System.Collections;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

public class SuperAdminManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TMP_Text feedbackText; // UI text to show feedback (e.g., success or error message)
    public TMP_InputField NameInput; // UI text to show the login header
    public TMP_InputField emailInput; // UI input field for email
    public TMP_InputField passwordInput; // UI input field for password
    public void OnLogoutButtonClicked()
    {
        GameManager.Instance.Logout();
    }
    void Start()
    {
        // clear all input fields
        feedbackText.text = ""; // Clear feedback text
        NameInput.text = ""; // Clear email input field
        emailInput.text = ""; // Clear email input field
        passwordInput.text = ""; // Clear password input field
    }

    public void OnAddAdminButtonClicked()
    {
        // Open the Add Admin Scene
        string name = NameInput.text;
        string email = emailInput.text;
        string password = passwordInput.text;

        string url = GameManager.Instance.BASE_URL + "api/create_user.php?name=" + name + "&email=" + email + "&password=" + password + "&type=admin";
        Debug.Log("URL: " + url); // Log the URL for debugging

        // validate the input fields
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            feedbackText.text = "Please enter all fields.";
            return;
        }
        // validate the email format
        if (!IsValidEmail(email))
        {
            feedbackText.text = "Please enter a valid email address.";
            return;
        }
        // validate name and password length 
        if (name.Length < 3)
        {
            feedbackText.text = "Name must be at least 3 characters long.";
            return;
        }
        if (password.Length < 6)
        {
            feedbackText.text = "Password must be at least 6 characters long.";
            return;
        }
        // web request to add admin
        StartCoroutine(AddAdmin(url));

    }
    private bool IsValidEmail(string email)
    {
        // Simple email validation logic (you can improve this regex for better validation)
        return System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }
    IEnumerator AddAdmin(string url)
    {
        // Create a new UnityWebRequest for the URL
        using (UnityEngine.Networking.UnityWebRequest webRequest = UnityEngine.Networking.UnityWebRequest.Get(url))
        {
            // Send the request and wait for a response
            yield return webRequest.SendWebRequest();

            // Check for network errors
            if (webRequest.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                feedbackText.text = "Error: " + webRequest.error;
            }
            else
            {
                // Handle the response from the server
                string response = webRequest.downloadHandler.text;
                Debug.Log("Response: " + response); // Log the response for debugging 
                Response res = JsonConvert.DeserializeObject<Response>(response);
                if(res.StatusCode == 200)
                {
                    feedbackText.text = "Admin added successfully!";
                    // Optionally, clear the input fields after successful addition
                    NameInput.text = "";
                    emailInput.text = "";
                    passwordInput.text = "";
                    Invoke(nameof(HideFeedbackText), 2f); // Hide feedback text after 2 seconds
                }
                else
                {
                    feedbackText.text = res.StatusMessage;
                }



                // if(response.Contains("200"))
                // {
                //     feedbackText.text = "Admin added successfully!";
                //     // Optionally, clear the input fields after successful addition
                //     NameInput.text = "";
                //     emailInput.text = "";
                //     passwordInput.text = "";
                //     Invoke(nameof(HideFeedbackText), 2f); // Hide feedback text after 2 seconds
                // }
                // else
                // {
                //     feedbackText.text = response;
                // }
            }
        }
    }
    void HideFeedbackText()
    {
        feedbackText.text = ""; // Clear feedback text
    }
}
