using System.Collections;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TMP_Text LoginHeaderText; // UI text to show the login header
    public TMP_InputField emailInput; // UI input field for email
    public TMP_InputField passwordInput; // UI input field for password
    public TMP_Text feedbackText; // UI text to show feedback (e.g., success or error message)

    public Button studentButton; // Button for student login
    public Button teacherButton; // Button for teacher login
    public Button adminButton; // Button for admin login
    public Button superAdminButton; // Button for super admin login

    void Start()
    {
        OnPanelSelectButtonClicked(0); // Default to student login
        // check player prefs for user type
        if (PlayerPrefs.HasKey("user_type"))
        {
            string userType = PlayerPrefs.GetString("user_type");
            if (userType == "student")
            {
                GameManager.Instance.OpenStudentsPanelScene(); 
            }
            else if (userType == "teacher")
            {
                GameManager.Instance.OpenTeachersPanelScene(); // Open teacher panel scene
            }
            else if (userType == "admin")
            {
                GameManager.Instance.OpenAdminPanelScene(); // Open admin panel scene
            }
            else if (userType == "super_admin")
            {
                // direct login to super admin panel
                GameManager.Instance.OpenSuperAdminPanelScene(); // Open super admin panel scene
            
            }
            return; // Exit the method if user type is found
        }
        feedbackText.text = ""; 

    }
    private void HighLightButton(Button button)
    {
        // Reset all buttons to default color
        studentButton.GetComponent<Image>().color = Color.white;
        teacherButton.GetComponent<Image>().color = Color.white;
        adminButton.GetComponent<Image>().color = Color.white;
        superAdminButton.GetComponent<Image>().color = Color.white;

        // Highlight the selected button
        button.GetComponent<Image>().color = Color.yellow;
    }
    string role_type = "student"; // Default type is student
    public void OnPanelSelectButtonClicked(int type){
        // type 0 = student, type 1 = teacher, type 2 = admin, 3 = super admin
        feedbackText.text = ""; // Clear feedback text
        emailInput.text = ""; // Clear email input field
        passwordInput.text = ""; // Clear password input field
        if (type == 0){
            LoginHeaderText.text = "Student Login"; // Set header text for student login
            HighLightButton(studentButton); // Highlight student button
            role_type = "student"; // Set type to student
        } else if (type == 1){
            LoginHeaderText.text = "Teacher Login"; // Set header text for teacher login
            HighLightButton(teacherButton); // Highlight teacher button
            role_type = "teacher"; // Set type to teacher
        } else if (type == 2){
            LoginHeaderText.text = "Admin Login"; // Set header text for admin login
            HighLightButton(adminButton); // Highlight admin button
            role_type = "admin"; // Set type to admin
        } else if (type == 3){
            LoginHeaderText.text = "Super Admin Login"; // Set header text for super admin login
            HighLightButton(superAdminButton); // Highlight super admin button
            role_type = "super_admin"; // Set type to super admin
        }
    } 
    public void OnLoginButtonClicked(){
        // Get the email and password from the input fields
        string email = emailInput.text;
        string password = passwordInput.text;

        // Check if the email and password are valid (this is just a placeholder, implement your own logic)
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            feedbackText.text = "Please enter both email and password.";
            feedbackText.color = Color.red;
            return;
        }

        // Perform login logic here (e.g., check against a database or API)
        // For demonstration purposes, we'll just show a success message
        // feedbackText.text = "Login Successful!";
        // feedbackText.color = Color.green;
        StartCoroutine(GetLogin(GameManager.Instance.BASE_URL + "api/login.php?email=" + email + "&password=" + password + "&type=" + role_type));
    }
    IEnumerator GetLogin(string url)
    {
        Debug.Log("URL: " + url); // Log the URL for debugging
        // Create a new UnityWebRequest for the URL
        using (UnityEngine.Networking.UnityWebRequest webRequest = UnityEngine.Networking.UnityWebRequest.Get(url))
        {
            // Send the request and wait for a response
            yield return webRequest.SendWebRequest();

            // Check for network errors
            if (webRequest.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                feedbackText.text = "Network error: " + webRequest.error;
                feedbackText.color = Color.red;
            }
            else
            {
                // Parse the response and check the status code
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log("Response: " + jsonResponse); // Log the response for debugging
                Response response = JsonConvert.DeserializeObject<Response>(jsonResponse);
                if (response.StatusCode == 200)
                {
                    feedbackText.text = "Login Successful!";
                    feedbackText.color = Color.green;

                UsersData user = response.Data.ToObject<UsersData>();
                    // Assuming the response contains user data in a specific format
                    // You may need to adjust this based on your actual API response structure
                 
//     "Id" => $user['id'],
//     "UserID" => $user['userid'],
//     "FullName" => $user['full_name'],
//     "Email" => $user['email'],
//     "Role" => $user['role'],
//     "LoginTime" => date("Y-m-d H:i:s")
// ];




                    // Save the user type in PlayerPrefs for later use
                    PlayerPrefs.SetString("user_type", role_type);
                    PlayerPrefs.SetString("user_email", emailInput.text); // Save the email for later use
                    PlayerPrefs.SetInt("Id", user.Id); // Save the user ID for later use
                    PlayerPrefs.SetString("UserID", user.UserID); // Save the full name for later use
                    PlayerPrefs.SetString("FullName", user.Name); // Save the full name for later use
                    PlayerPrefs.SetString("Role", user.Role); // Save the role for later use
                    PlayerPrefs.SetString("LoginTime", user.Created_at); // Save the login time for later use




                    // Load the appropriate scene based on user type
                    if (role_type == "student")
                    {
                        GameManager.Instance.OpenStudentsPanelScene();
                    }
                    else if (role_type == "teacher")
                    {
                        GameManager.Instance.OpenTeachersPanelScene();
                    }
                    else if (role_type == "admin")
                    {
                        GameManager.Instance.OpenAdminPanelScene();
                    }
                    else if (role_type == "super_admin")
                    {
                        GameManager.Instance.OpenSuperAdminPanelScene();
                    }
                }
                else
                {
                    feedbackText.text = response.StatusMessage;
                    feedbackText.color = Color.red;
                }
            }
        }
    }
}



