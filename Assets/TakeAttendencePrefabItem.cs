using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TakeAttendencePrefabItem : MonoBehaviour
{
    
    public TMP_Text StudentName;
    public TMP_Text StudentAttendencePercentText;
    public Toggle StudentAttendenceToggle;
    public UsersData studentData;


    // public void OnStudentAttendenceDropdownValueChanged()
    // {
    //     // Handle the dropdown value change here
    //     // You can access the selected value using the 'value' parameter
    //     bool selected = StudentAttendenceToggle.isOn;
    //     Debug.Log("Selected value: " + selected);
    //     Debug.Log("Student Name: " + studentData.Name);
    //     Debug.Log("Student ID: " + studentData.Id);

    // }
}
