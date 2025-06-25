using Newtonsoft.Json.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public AdminManager adminManager;

    public string BASE_URL = "https://project.icttime.com/";
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // open diffrent scene
    public void OpenLoginScene()
    {
        Debug.Log("Opening Login Scene");
        UnityEngine.SceneManagement.SceneManager.LoadScene("LoginPanel");
    }
    public void OpenTeachersPanelScene()
    {
        Debug.Log("Opening Teachers Panel Scene");
        UnityEngine.SceneManagement.SceneManager.LoadScene("TeacherPanel");
    }
    public void OpenStudentsPanelScene()
    {
        Debug.Log("Opening Students Panel Scene");
        UnityEngine.SceneManagement.SceneManager.LoadScene("StudentPanel");
    }
    public void OpenSuperAdminPanelScene()
    {
        Debug.Log("Opening Super Admin Panel Scene");
        UnityEngine.SceneManagement.SceneManager.LoadScene("SuperAdminPanel");
    }
    public void OpenAdminPanelScene()
    {
        Debug.Log("Opening Admin Panel Scene");
        UnityEngine.SceneManagement.SceneManager.LoadScene("AdminPanel");
    }
    // logout function
    public void Logout()
    {
        // Clear any saved user data or tokens here
        PlayerPrefs.DeleteAll();
        // Load the login scene
        OpenLoginScene();
    }


    public void OnOpenCourseDetailsPanelBtnClick(CourseData courseData)
    {
        Debug.Log("Opening Course Details Panel");
        // Load the CourseDetailsPanel scene and pass the course data to it
        adminManager.ShowCourseDetailsInUI(courseData);
    }
}

[System.Serializable]
public class Response
{
    public int StatusCode { get; set; }
    public string StatusMessage { get; set; }
    public JToken Data { get; set; }
}

[System.Serializable]
public class CourseData {
    public int Id { get; set; }
    public int EnrollmentStatus { get; set; }
    public string Name { get; set; }
    public string Scope { get; set; }
    public string Details { get; set; }
    public int Credit_hours { get; set; }
    public int Duration { get; set; }
    public string Semester { get; set; }
    public string Created_at { get; set;}
}
[System.Serializable]
public class UsersData
{
    public int Id { get; set; }
    public string UserID { get; set; }
    public string Name { get; set; }
    public int Score { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
    public string Created_at { get; set;}
    public int[] PresentLectures { get; set; }
    public string PresentLecturesString { get; set; }
}
[System.Serializable]
public class LeaderboardData
{
    public int Id { get; set; }
    public int Rank { get; set; }
    public string UserID { get; set; }
    public string Name { get; set; }
    public int Score { get; set; }
    public string Date { get; set; }
}