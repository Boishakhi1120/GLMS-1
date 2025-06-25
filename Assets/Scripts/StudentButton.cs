using UnityEngine;

public class StudentButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnrollClicked(string name)
    {
        if (name == "Math")
        {
            PlayerPrefs.SetInt(StudentData.MATH_ENROLL, 1);
        }

        else if (name == "English")
        {
            PlayerPrefs.SetInt(StudentData.ENGLISH_ENROLL, 1);
        }
        else if (name == "Science")
        {
            PlayerPrefs.SetInt(StudentData.SCIENCE_ENROLL, 1);
        }

    }
}
