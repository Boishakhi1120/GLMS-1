using UnityEngine;
using TMPro;
using SFB; // StandaloneFileBrowser namespace
using System.IO;
using EasyUI.Toast;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class StudentManager2 : MonoBehaviour
{
    public TMP_Text studentNameText;
    public TMP_Text scoreText;
    public TMP_Text assignmenText;


    void Start()
    {
        studentNameText.text = DatabaseManager.instance.GetCurrentStudent().StudentName;
        scoreText.text = DatabaseManager.instance.GetCurrentStudent().StudentScore.ToString();
        assignmenText.text = PlayerPrefs.GetString(PlayerPrefData.TEACHER_ASSIGNTEXT);
    }

    private string savedFilePath;
    public void OpenFilePicker()
    {

        
        // Open the file browser dialog for picking a file (single file selection)
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Select PDF File", "", "pdf", false);

        if (paths.Length > 0)
        {
            // Display the selected file path
            Debug.Log("File selected: " + paths[0]);
            savedFilePath = Path.Combine(Application.persistentDataPath, Path.GetFileName(paths[0]));
            File.Copy(paths[0], savedFilePath, true); // Copy the file to persistent data path
                                                      //filePathText.text = "File Saved: " + savedFilePath;

            
            Toast.Show("File uploaded succesfully");
            UpdateScore();
            //DisplayFile(savedFilePath);

            // Here, you can implement the file upload logic (e.g., sending the file to a server, reading its contents, etc.)
        }
        else
        {
            Debug.Log("No file selected.");
        }
    }

    public void OpenFileInExternalViewer()
    {
        try
        {
            // Open the file using the default program associated with the file type
            Process.Start(DatabaseManager.instance.GetAssignmentPath(DatabaseManager.instance.currentStudent.StuentID));
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError("Error opening file: " + e.Message);
        }
    }

    public void UpdateScore()
    {
        Student student = DatabaseManager.instance.GetCurrentStudent();
        student.StudentScore += 100;
        scoreText.text = student.StudentScore.ToString();
        DatabaseManager.instance.UpdateStudentScoreByEmail(student.StudentEmail,student.StudentScore);

    }
}
