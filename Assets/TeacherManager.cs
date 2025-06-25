using UnityEngine;
using TMPro;
using SFB; // StandaloneFileBrowser namespace
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.IO;

public class TeacherManager : MonoBehaviour
{

    public TMP_Text teacherNameText;

    [Header("NAVIGATION BUTTONS")]
    public Button LeaderboardButton;
    public Button CourseMaterialButton;
    public Button CourseAssignmentButton;
    public Button TakeAttendanceButton;

    [Header("PANELS")]
    public GameObject LeaderboardPanel;
    public GameObject CourseMaterialPanel;
    public GameObject CourseAssignmentPanel;
    public GameObject TakeAttendancePanel;

    [Header("Leaderboard Variables")]
    public GameObject LeaderboardItemPrefab;
    public Transform LeaderboardPanelContent;

    [Header("Course Assignment Variables")]
    public GameObject CouseAssignmentSelectionPanel;
    public GameObject CouseAssignmentUploadPanel;
    public TMP_Dropdown selecteCourseDropdownAssignment;
    public TMP_Text CouseAssignmentFeedbackText;
    public TMP_InputField teacherAssignmentText;
    public TMP_Text CourseAssignmentUploadFeedbackText;


    [Header("Course Material Variables")]
    public GameObject CouseMaterialSelectionPanel;
    public GameObject CouseMaterialUploadPanel;
    public TMP_Dropdown selecteCourseDropdownMaterial;
    public TMP_Text CouseMaterialFeedbackText;
    public TMP_Text CouseMaterialUploadFeedbackText;
    public TMP_InputField teacherMaterialTextInputField;


    [Header("Attendance Variables")]
    public GameObject TakeAttendanceSelectionPanel;
    public GameObject TakeAttendanceUploadPanel;
    public TMP_Dropdown selecteCourseDropdownAttendance;
    public TMP_Dropdown selectLectureDropdownAttendance;
    public TMP_Text TakeAttendanceFeedbackText;
    public TMP_Text TakeAttendanceUploadFeedbackText;
    public GameObject TakeAttendanceItemPrefab;
    public Transform TakeAttendancePrefabParent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private CourseData[] teacherCoursesList;
    void Start()
    {
        OnPanelSelectButtonClicked(0); // Default to student login
        teacherNameText.text = PlayerPrefs.GetString("FullName");
        StartCoroutine(GetCourseListByTeacher());
    }
    private void HighLightButton(Button button)
    {
        // Reset all buttons to default color
        LeaderboardButton.GetComponent<Image>().color = Color.white;
        CourseMaterialButton.GetComponent<Image>().color = Color.white;
        CourseAssignmentButton.GetComponent<Image>().color = Color.white;
        TakeAttendanceButton.GetComponent<Image>().color = Color.white;

        // Highlight the selected button
        button.GetComponent<Image>().color = Color.yellow;
    }
    public void OnPanelSelectButtonClicked(int type){
        // 0= leaderboard, 1= assignment, 3 = course material, 4 = attendance
        switch (type)
        {
            case 0:
                HighLightButton(LeaderboardButton);
                OnLeaderboardPanelSelected();
                break;
            case 1:
                HighLightButton(CourseAssignmentButton);
                OnCourseAssignmentPanelSelected();
                break;
            case 2:
                HighLightButton(CourseMaterialButton);
                OnCourseMaterialPanelSelected();
                break;
            case 3:
                HighLightButton(TakeAttendanceButton);
                OnTakeAttendancePanelSelected();
                break;
            default:
                Debug.Log("Invalid type selected");
                break;
        }
    }
// Leaderboard section =========================================================================================

    public void OnLeaderboardPanelSelected()
    {
        // Show the leaderboard panel and hide others
        LeaderboardPanel.SetActive(true);
        CourseMaterialPanel.SetActive(false);
        CourseAssignmentPanel.SetActive(false);
        TakeAttendancePanel.SetActive(false);
        StartCoroutine(GetLeaderboardData());
    }
    LeaderboardData[] allLeaderboardData;
    IEnumerator GetLeaderboardData()
    {
    //     $leaderboardData[] = [
    //     "Rank" => $rank,
    //     "Id" => (int)$row['id'],
    //     "UserID" => $row['user_id'],
    //     "Name" => $row['full_name'],
    //     "Score" => (int)$row['score'],
    //     "Date" => $row['date']
    // ];get_leaderboard.php
       string url = GameManager.Instance.BASE_URL + "api/get_leaderboard.php";
        // get method
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(webRequest.error);
            }
            else
            {
                // Process the response
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log("Leaderboard Data: " + jsonResponse);
                // Parse and display the leaderboard data here
                Response response = JsonConvert.DeserializeObject<Response>(jsonResponse);
                if(response.StatusCode==200){
                    allLeaderboardData = response.Data.ToObject<LeaderboardData[]>();
                    StartCoroutine(UpdateLeaderboardUI(allLeaderboardData));
                }else
                {
                    Debug.Log("Error: " + response.StatusMessage);

                }
            }
        }
    }
    IEnumerator UpdateLeaderboardUI(LeaderboardData[] leaderboardData)
    {
        // Clear existing leaderboard items
        foreach (Transform child in LeaderboardPanelContent)
        {
            Destroy(child.gameObject);
        }
        int rank = 1;
        // Instantiate and populate leaderboard items
        foreach (LeaderboardData data in leaderboardData)
        {
            GameObject item = Instantiate(LeaderboardItemPrefab, LeaderboardPanelContent);
            LeaderboardItem leaderbaorditm =    item.GetComponent<LeaderboardItem>();
            leaderbaorditm.StudentPosition.text = rank.ToString();
            rank++;
            leaderbaorditm.ScoreText.text = data.Score.ToString();
            leaderbaorditm.StudentNameText.text = data.Name;
            leaderbaorditm.leaderboardData = data;
            
        }
        yield return null; // Wait for the UI to update
    }
        
// Assignment section =========================================================================================

    public void OnCourseAssignmentPanelSelected()
    {
        // Show the course assignment panel and hide others
        CourseAssignmentPanel.SetActive(true);
        LeaderboardPanel.SetActive(false);
        CourseMaterialPanel.SetActive(false);
        TakeAttendancePanel.SetActive(false);

        // panel settings
        CouseAssignmentUploadPanel.SetActive(false);
        CouseAssignmentSelectionPanel.SetActive(true);
        CourseAssignmentUploadFeedbackText.text = "";
        CouseAssignmentFeedbackText.text = "";
        teacherAssignmentText.text = "";


        // Load the course data for the dropdown
        selecteCourseDropdownAssignment.ClearOptions();
        foreach (CourseData course in teacherCoursesList)
        {
            selecteCourseDropdownAssignment.options.Add(new TMP_Dropdown.OptionData(course.Name));
        }
        selecteCourseDropdownAssignment.RefreshShownValue();
        // Remove old listener to avoid multiple subscriptions
        selecteCourseDropdownAssignment.onValueChanged.RemoveAllListeners();

        // Add the listener for the dropdown
        selecteCourseDropdownAssignment.onValueChanged.AddListener(OnCourseAssignmentDropdownValueChanged);

        // Set the default selected value to the first option
        if (selecteCourseDropdownAssignment.options.Count > 0)
        {
            selecteCourseDropdownAssignment.value = 0;
            OnCourseAssignmentDropdownValueChanged(0);
        }
    }
    IEnumerator GetCourseListByTeacher(){
        // get_teacher_courses.php
    //     courses[] = [
    //     "Id" => (int)$row['id'],
    //     "Name" => $row['name'],
    //     "Scope" => $row['scope'],
    //     "Details" => $row['details'],
    //     "Credit_hours" => (int)$row['credit_hours'],
    //     "Duration" => (int)$row['duration'],
    //     "Semester" => $row['semester'],
    //     "Assigned_date" => $row['assigned_date'],
    //     "Created_at" => $row['created_at']
    // ];

        string url = GameManager.Instance.BASE_URL + "api/get_teacher_courses.php?teacher_id=" + PlayerPrefs.GetInt("Id");
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(webRequest.error);
            }
            else
            {
                // Process the response
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log("Course Data: " + jsonResponse);
                // Parse and display the course data here
                Response response = JsonConvert.DeserializeObject<Response>(jsonResponse);
                if(response.StatusCode==200){
                    teacherCoursesList = response.Data.ToObject<CourseData[]>();

                    // selecteCourseDropdownAssignment.ClearOptions();
                    // foreach (CourseData course in allCourseData)
                    // {
                    //     selecteCourseDropdownAssignment.options.Add(new TMP_Dropdown.OptionData(course.Name));
                    // }
                    // selecteCourseDropdownAssignment.RefreshShownValue();
                }else
                {
                    Debug.Log("Error: " + response.StatusMessage);

                }
            }
        }
    }
    string lastSelectedCourseNameAssignment;
    public void OnCourseAssignmentDropdownValueChanged(int index)
    {
        // Handle the dropdown value change: name of index
        Debug.Log("Selected course: " + selecteCourseDropdownAssignment.options[index].text);
        lastSelectedCourseNameAssignment = selecteCourseDropdownAssignment.options[index].text;
    }
    public void OnCourseAssignmentGiveButtonClicked()
    {
        // Show the course assignment selection panel
        CouseAssignmentUploadPanel.SetActive(true);
        CouseAssignmentSelectionPanel.SetActive(false);
        CouseAssignmentFeedbackText.text = "";
        CourseAssignmentUploadFeedbackText.text = "";
        
    }

    public void OnCourseAssignmentUploadButtonClicked()
    {
        // validate the input
        if (string.IsNullOrEmpty(teacherAssignmentText.text))
        {
            CourseAssignmentUploadFeedbackText.text = "Please enter assignment text.";
            return;
        }
        // todo: 
        Debug.Log("Upload assignment by selected option " + selecteCourseDropdownAssignment.options[selecteCourseDropdownAssignment.value].text);
        StartCoroutine(UploadAssignmentTextToServer(lastSelectedCourseNameAssignment, teacherAssignmentText.text));


    }
    IEnumerator UploadAssignmentTextToServer(string courseName, string assignmentText)
    {
        string url = GameManager.Instance.BASE_URL + "api/upload_assignment.php";
        WWWForm form = new WWWForm();
        form.AddField("course_name", courseName);
        form.AddField("assignment_text", assignmentText);
        form.AddField("teacher_id", PlayerPrefs.GetInt("Id"));

        using (UnityWebRequest webRequest = UnityWebRequest.Post(url, form))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(webRequest.error);
            }
            else
            {
                // Process the response
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log("Upload Response: " + jsonResponse);
                // Parse and display the response here
                Response response = JsonConvert.DeserializeObject<Response>(jsonResponse);
                if(response.StatusCode==200){
                    CourseAssignmentUploadFeedbackText.text = "Assignment uploaded successfully.";
                    Invoke(nameof(HideAssignMentUploadFeedabackText), 3f);
                }else
                {
                    CourseAssignmentUploadFeedbackText.text = "Error: " + response.StatusMessage;

                }
            }
        }




    }
    public void HideAssignMentUploadFeedabackText()
    {
        CourseAssignmentUploadFeedbackText.text = "";
        CouseAssignmentFeedbackText.text = "";
        teacherAssignmentText.text = "";
    }

    // section of course material=================================================================================================
    // section of course material
    public void OnCourseMaterialPanelSelected()
    {
        // Show the course material panel and hide others
        CourseMaterialPanel.SetActive(true);
        LeaderboardPanel.SetActive(false);
        CourseAssignmentPanel.SetActive(false);
        TakeAttendancePanel.SetActive(false);

        // panel settings
        CouseMaterialUploadPanel.SetActive(false);
        CouseMaterialSelectionPanel.SetActive(true);
        CouseMaterialUploadFeedbackText.text = "";
        CouseMaterialFeedbackText.text = "";
        teacherMaterialTextInputField.text = "";
        // Load the course data for the dropdown
        selecteCourseDropdownMaterial.ClearOptions();
        foreach (CourseData course in teacherCoursesList)
        {
            selecteCourseDropdownMaterial.options.Add(new TMP_Dropdown.OptionData(course.Name));
        }
        selecteCourseDropdownMaterial.RefreshShownValue();
        // Remove old listener to avoid multiple subscriptions
        selecteCourseDropdownMaterial.onValueChanged.RemoveAllListeners();
        // Add the listener for the dropdown
        selecteCourseDropdownMaterial.onValueChanged.AddListener(OnCourseMaterialDropdownValueChanged);

        // Set the default selected value to the first option
        if (selecteCourseDropdownMaterial.options.Count > 0)
        {
            selecteCourseDropdownMaterial.value = 0;
            OnCourseMaterialDropdownValueChanged(0);
        }
    }

    string lastSelectedCourseNameMaterial;
    public void OnCourseMaterialDropdownValueChanged(int index)
    {
        // Handle the dropdown value change
        Debug.Log("Selected course: " + selecteCourseDropdownMaterial.options[index].text);
        lastSelectedCourseNameMaterial = selecteCourseDropdownMaterial.options[index].text;
    }
    public void OnCourseMaterialGiveButtonClicked()
    {
        // Show the course assignment selection panel
        CouseMaterialUploadPanel.SetActive(true);
        CouseMaterialSelectionPanel.SetActive(false);
        CouseMaterialFeedbackText.text = "";
        CouseMaterialUploadFeedbackText.text = "";
    }

    public void OnCourseMaterialUploadButtonClicked()
    {
        Debug.Log("Upload course material by selected option " + selecteCourseDropdownMaterial.options[selecteCourseDropdownMaterial.value].text);
        // validate the input
        if (string.IsNullOrEmpty(teacherMaterialTextInputField.text))
        {
            CouseMaterialUploadFeedbackText.text = "Please enter course material text.";
            return;
        }
        StartCoroutine(UploadCourseMaterialToServer());
    }
    public void OnCourseMaterialUploadFileButtonClicked()
    {
        OpenFilePicker();
    }

    private string savedFilePath;
    public void OpenFilePicker()
    {
        // Open the file browser dialog for picking a file (single file selection)
        //string[] paths = StandaloneFileBrowser.OpenFilePanel("Select PDF File", "", "pdf", false);

        string[] paths = StandaloneFileBrowser.OpenFilePanel("Select File", "", "", false);


        if (paths.Length > 0)
        {
            // Display the selected file path
            Debug.Log("File selected: " + paths[0]);
            savedFilePath = Path.Combine(Application.persistentDataPath, Path.GetFileName(paths[0])).Replace("\\", "/");
            
            try {
                // Copy the file to persistent data path
                File.Copy(paths[0], savedFilePath, true);
                Debug.Log("File copied successfully to: " + savedFilePath);
                CouseMaterialUploadFeedbackText.text = "File selected: " + Path.GetFileName(paths[0]);
            }
            catch (System.Exception e) {
                Debug.LogError("Error copying file: " + e.Message);
                CouseMaterialUploadFeedbackText.text = "Error selecting file: " + e.Message;
                savedFilePath = null;
            }
        }
        else
        {
            Debug.Log("No file selected.");
            CouseMaterialUploadFeedbackText.text = "No file selected.";
        }
    }

    IEnumerator UploadCourseMaterialToServer(){
        string url = GameManager.Instance.BASE_URL + "api/upload_course_material.php";
        Debug.Log("URL: " + url);
        byte[] fileData = null;
        string fileName = string.Empty;

        if (!string.IsNullOrEmpty(savedFilePath) && File.Exists(savedFilePath))
        {
            fileData = File.ReadAllBytes(savedFilePath);
            fileName = Path.GetFileName(savedFilePath);
        }
        else
        {
            Debug.LogError("File path is invalid or file does not exist: " + savedFilePath);
            CouseMaterialUploadFeedbackText.text = "Error: File not found. Please select a valid file.";
            yield break; // Exit the coroutine to prevent further execution
        }

        WWWForm form = new WWWForm();
        form.AddField("course_name", lastSelectedCourseNameMaterial);
        form.AddField("material_text", teacherMaterialTextInputField.text);
        form.AddField("teacher_id", PlayerPrefs.GetInt("Id"));
        form.AddBinaryData("file", fileData, fileName, "application/pdf");
        using (UnityWebRequest webRequest = UnityWebRequest.Post(url, form))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(webRequest.error);
            }
            else
            {
                // Process the response
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log("Upload Response: " + jsonResponse);
                // Parse and display the response here
                Response response = JsonConvert.DeserializeObject<Response>(jsonResponse);
                if(response.StatusCode==200){
                    CouseMaterialUploadFeedbackText.text = "Course material uploaded successfully.";
                    Invoke(nameof(HideCourseMaterialUploadFeedabackText), 3f);
                }else
                {
                    CouseMaterialUploadFeedbackText.text = "Error: " + response.StatusMessage;

                }
            }
        }
        
    }
    public void HideCourseMaterialUploadFeedabackText()
    {
        CouseMaterialUploadFeedbackText.text = "";
        CouseMaterialFeedbackText.text = "";
        teacherMaterialTextInputField.text = "";
        savedFilePath = "";
    }

    // section of attendance=================================================================================================


    public void OnTakeAttendancePanelSelected()
    {
        // Show the course assignment panel and hide others
        TakeAttendancePanel.SetActive(true);
        LeaderboardPanel.SetActive(false);
        CourseMaterialPanel.SetActive(false);
        CourseAssignmentPanel.SetActive(false);

        // panel settings
        TakeAttendanceUploadPanel.SetActive(false);
        TakeAttendanceSelectionPanel.SetActive(true);
        TakeAttendanceUploadFeedbackText.text = "";
        TakeAttendanceFeedbackText.text = "";
        // Load the course data for the dropdown
        selecteCourseDropdownAttendance.ClearOptions();
        foreach (CourseData course in teacherCoursesList)
        {
            selecteCourseDropdownAttendance.options.Add(new TMP_Dropdown.OptionData(course.Name));
        }
        selecteCourseDropdownAttendance.RefreshShownValue();
        // Remove old listener to avoid multiple subscriptions
        selecteCourseDropdownAttendance.onValueChanged.RemoveAllListeners();
        // Add the listener for the dropdown
        selecteCourseDropdownAttendance.onValueChanged.AddListener(OnTakeAttendanceDropdownValueChanged);

        // Load the lecture number data for the dropdown
        selectLectureDropdownAttendance.ClearOptions();
        for (int i = 1; i <= 26; i++)
        {
            selectLectureDropdownAttendance.options.Add(new TMP_Dropdown.OptionData("Lecture " + i));
        }

        // Remove old listener to avoid multiple subscriptions
        selectLectureDropdownAttendance.onValueChanged.RemoveAllListeners();
        // Add the listener for the dropdown
        selectLectureDropdownAttendance.onValueChanged.AddListener(OnTakeAttendanceLectureNumberValueChanged2);
        selectLectureDropdownAttendance.RefreshShownValue();

        // Set the default selected value for course dropdown
        if (selecteCourseDropdownAttendance.options.Count > 0)
        {
            selecteCourseDropdownAttendance.value = 0;
            OnTakeAttendanceDropdownValueChanged(0);
        }

        // Set the default selected value for lecture dropdown
        if (selectLectureDropdownAttendance.options.Count > 0)
        {
            selectLectureDropdownAttendance.value = 0;
            OnTakeAttendanceLectureNumberValueChanged2(0);
        }
    }
    string lastSelectedCourseNameAttendance;
    public void OnTakeAttendanceDropdownValueChanged(int index)
    {
        // Handle the dropdown value change
        Debug.Log("Selected course: " + selecteCourseDropdownAttendance.options[index].text);
        lastSelectedCourseNameAttendance = selecteCourseDropdownAttendance.options[index].text;
    }
    string lastSelectedLectureNumberAttendance;
    public void OnTakeAttendanceLectureNumberValueChanged2(int index)
    {
        Debug.Log("Option count: " + selectLectureDropdownAttendance.options.Count);
        // Handle the dropdown value change Selected course: Lecture 12
        Debug.Log("Index: " + index);
        Debug.Log("Selected course: " + selectLectureDropdownAttendance.options[index].text);
        lastSelectedLectureNumberAttendance = selectLectureDropdownAttendance.options[index].text;
    }
    public void OnTakeAttendanceGiveButtonClicked()
    {
        // Show the course assignment selection panel
        TakeAttendanceUploadPanel.SetActive(true);
        TakeAttendanceSelectionPanel.SetActive(false);
        TakeAttendanceFeedbackText.text = "";
        TakeAttendanceUploadFeedbackText.text = "";
        // Load the student data for the selected course
        StartCoroutine(GetStudentListByCourseName(lastSelectedCourseNameAttendance));
    }

    IEnumerator GetStudentListByCourseName(string courseName)
    {
        // get_student_list_by_course_name.php
        string url = GameManager.Instance.BASE_URL + "api/get_student_list_by_course_name.php?course_name=" + courseName;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(webRequest.error);
            }
            else
            {
                // Process the response
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log("Student List Data: " + jsonResponse);
                // Parse and display the student list data here
                Response response = JsonConvert.DeserializeObject<Response>(jsonResponse);
                if(response.StatusCode==200){
                    UsersData[] studentDataList = response.Data.ToObject<UsersData[]>();
                    StartCoroutine(UpdateAttendanceUI(studentDataList));
                }else
                {
                    Debug.Log("Error: " + response.StatusMessage);

                }
            }
        }
    }
    IEnumerator UpdateAttendanceUI(UsersData[] studentDataList)
    {
        // Clear existing attendance items
        foreach (Transform child in TakeAttendancePrefabParent)
        {
            Destroy(child.gameObject);
        }

        // Get the current lecture number from dropdown
        int currentLectureNumber = 0;
        if (!string.IsNullOrEmpty(lastSelectedLectureNumberAttendance))
        {
            string[] parts = lastSelectedLectureNumberAttendance.Split(' ');
            if (parts.Length > 1 && int.TryParse(parts[1], out int result))
            {
                currentLectureNumber = result;
            }
        }
        
        // Instantiate and populate attendance items
        foreach (UsersData data in studentDataList)
        {
            GameObject item = Instantiate(TakeAttendanceItemPrefab, TakeAttendancePrefabParent);
            TakeAttendencePrefabItem takeAttendanceItem = item.GetComponent<TakeAttendencePrefabItem>();
            takeAttendanceItem.StudentName.text = data.Name;
            takeAttendanceItem.StudentAttendencePercentText.text = data.Score.ToString() + "%";
            
            // Check if the student's PresentLectures property contains the current lecture number
            bool isPresent = false;
            
            // First convert PresentLecturesString to an array if PresentLectures is null
            if (data.PresentLectures != null)
            {
                isPresent = System.Array.IndexOf(data.PresentLectures, currentLectureNumber) >= 0;
            }
            else if (!string.IsNullOrEmpty(data.PresentLecturesString))
            {
                string[] presentLectures = data.PresentLecturesString.Split(',');
                foreach (string lecture in presentLectures)
                {
                    if (!string.IsNullOrEmpty(lecture) && int.TryParse(lecture, out int lectureNum) && lectureNum == currentLectureNumber)
                    {
                        isPresent = true;
                        break;
                    }
                }
            }
            
            // Set the toggle state based on whether the student was present in this lecture
            takeAttendanceItem.StudentAttendenceToggle.isOn = isPresent;
            
            // Add listener for the toggle value change
            takeAttendanceItem.StudentAttendenceToggle.onValueChanged.AddListener(delegate { OnTakeAttendanceCheckboxClicked(takeAttendanceItem.StudentAttendenceToggle.isOn, data.Id); });
            takeAttendanceItem.studentData = data;
        }
        yield return null; // Wait for the UI to update
    }

    
    public void OnTakeAttendanceCheckboxClicked(bool isChecked, int studentID)
    {
        string courseName = lastSelectedCourseNameAttendance;
        string lectureNumber = lastSelectedLectureNumberAttendance;
        Debug.Log("Course Name: " + courseName + ", Lecture Number: " + lectureNumber);
        Debug.Log("Checkbox clicked: " + isChecked + " for student ID: " + studentID);
        
        // Extract class number from the lecture number string (e.g., "Lecture 12" -> 12)
        int classNumber = 0;
        if (!string.IsNullOrEmpty(lectureNumber))
        {
            string[] parts = lectureNumber.Split(' ');
            if (parts.Length > 1 && int.TryParse(parts[1], out int result))
            {
                classNumber = result;
            }
        }
        
        if (classNumber <= 0)
        {
            Debug.LogError("Invalid class number format: " + lectureNumber);
            TakeAttendanceUploadFeedbackText.text = "Error: Invalid class number format.";
            return;
        }
        
        // Convert boolean to attendance status (1 = present, 0 = absent)
        int status = isChecked ? 1 : 0;
        
        // Start the coroutine to record attendance
        StartCoroutine(RecordAttendance(courseName, PlayerPrefs.GetInt("Id"), studentID, classNumber, status));
    }
    
    IEnumerator RecordAttendance(string courseName, int teacherId, int studentId, int classNumber, int status)
    {
        string url = GameManager.Instance.BASE_URL + "api/give_attendance.php";
        WWWForm form = new WWWForm();
        form.AddField("course_name", courseName);
        form.AddField("teacher_id", teacherId);
        form.AddField("student_id", studentId);
        form.AddField("class_number", classNumber);
        form.AddField("status", status);
        
        using (UnityWebRequest webRequest = UnityWebRequest.Post(url, form))
        {
            yield return webRequest.SendWebRequest();
            
            if (webRequest.result == UnityWebRequest.Result.ConnectionError || 
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error recording attendance: " + webRequest.error);
                TakeAttendanceUploadFeedbackText.text = "Error recording attendance: " + webRequest.error;
            }
            else
            {
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log("Attendance Response: " + jsonResponse);
                
                Response response = JsonConvert.DeserializeObject<Response>(jsonResponse);
                if (response.StatusCode == 200)
                {
                    string statusText = status == 1 ? "present" : "absent";
                    TakeAttendanceUploadFeedbackText.text = "Attendance marked " + statusText + " successfully.";
                }
                else
                {
                    TakeAttendanceUploadFeedbackText.text = "Error: " + response.StatusMessage;
                }
            }
        }
    }

    public void OnLogoutButtonClicked()
    {
        // Load the login scene
        GameManager.Instance.Logout();
    }

    /// <summary>
    /// ////// think of a better schema diagram for present and absent
    /// </summary>
    /// 
    public void UploadAssignmentText()
    {
        // DatabaseManager.instance.UpdateCourseAssignmentTextByTeacherName(PlayerPrefs.GetString(PlayerPrefData.TEACHER_NAME),teacherAssignmentText.text);
    }



}
