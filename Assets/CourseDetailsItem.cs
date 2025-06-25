using UnityEngine;
using TMPro;

public class CourseDetailsItem : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text courseNameText;
    public TMP_Text courseSemesterText;
    public TMP_Text courseCreditText;
    public TMP_Text courseDurationText;

  
    public CourseData courseData; // Reference to the CourseData scriptable object

    public void OnObjectClick()
    {
        GameManager.Instance.OnOpenCourseDetailsPanelBtnClick(this.courseData);
    }
}
