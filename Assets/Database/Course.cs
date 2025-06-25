using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Course : MonoBehaviour
{
    public TMP_Text coursenameText;
    public TMP_Text courseDescText;
    public TMP_Text courseTeacherText;
    public TMP_Text noOfStudentsTExt;
    public TMP_Text text;
    

    public string courseName;
    public string courseDescription;
    public int courseID;
    public string courseTeacher;
    public string noOfStudents;
    public string assignmentPath;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        coursenameText.text = courseName;
        courseTeacherText.text = courseTeacher;
        courseDescText.text = courseDescription;
        noOfStudentsTExt.text = noOfStudents.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

  
}
