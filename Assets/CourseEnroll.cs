using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CourseEnroll : MonoBehaviour
{
    public Button enrollButton;
    public TMP_Text enrollButtonText;
    public Course course;

    private int teacherID;
    private int studentID;

    void Start()
    {
         teacherID = DatabaseManager.instance.GetTeacherIDByTeacherName(course.courseTeacher);
         studentID = DatabaseManager.instance.GetCurrentStudent().StuentID;

        if (DatabaseManager.instance.CheckEnrollmentExists(studentID, course.courseID))
        {
            enrollButton.interactable = false;
            enrollButtonText.text = "Enrolled";
        }
    }

    // Update is called once per frame
    public void Enroll()
    {             
      DatabaseManager.instance.EnrollStudentInCourse(teacherID, studentID, course.courseID);
        enrollButton.interactable = false;
        enrollButtonText.text = "Enrolled";
    }
}
