using UnityEngine;
using TMPro;
using UnityEngine.UI; // Changed from System.Windows.Forms to UnityEngine.UI for Unity Button

public class CourseDetailsItemEnroll : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text courseNameText;
    public TMP_Text courseSemesterText;
    public TMP_Text courseCreditText;
    public TMP_Text courseDurationText;

    public Button EnrollButton; // Reference to the button component
    public TMP_Text EnrollButtonText; // Reference to the button text component
    public CourseData courseData; // Reference to the CourseData scriptable object
    public Button FullPanelButton; // Reference to the FullPanel button component
}
