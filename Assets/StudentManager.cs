using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.IO;
using SFB; // Add StandaloneFileBrowser namespace

public class StudentManager : MonoBehaviour
{
    public TMP_Text studentNameText;
    public TMP_Text studentScoreText;

    [Header("NAVIGATION BUTTONS")]
    public Button LeaderboardButton;
    public Button ViewAssignmentButton;
    public Button CourseMaterialButton;
    public Button OpenCoursesButton;

    [Header("PANELS")]
    public GameObject LeaderboardPanel;
    public GameObject ViewAssignmentPanel;
    public GameObject CourseMaterialPanel;
    public GameObject OpenCoursesPanel;

    [Header("Leaderboard Variables")]
    public GameObject LeaderboardSelectionPanel;
    public TMP_Dropdown leaderboardCourseDropdown;


    public GameObject LeaderboardDetailsPanel;
    public TMP_Text LeaderboardFeedbackText;
    public GameObject LeaderboardItemPrefab;
    public Transform LeaderboardPanelContent; // parent for spawn items

    [Header("View Assignment Variables")]
    public GameObject ViewAssignmentSelectionPanel;
    public GameObject ViewAssignmentDetailsPanel;
    public TMP_Dropdown selectCourseDropdownAssignment;
    public TMP_Text ViewAssignmentFeedbackText;
    public TMP_Text AssignmentDetailsText;

    [Header("Course Material Variables")]
    public GameObject CourseMaterialSelectionPanel;
    public GameObject CourseMaterialDetailsPanel;
    public TMP_Dropdown selectCourseDropdownMaterial;
    public TMP_Text CourseMaterialFeedbackText;
    public TMP_Text MaterialDetailsText;
    public Button DownloadMaterialButton;

    [Header("Open Courses Variables")]
    public Transform OpenCoursesPanelContent;
    public GameObject CourseItemPrefab;
    public TMP_Text OpenCoursesFeedbackText;

        // Course details data
    [Header("Course Details Panel")]
    public GameObject courseDetailsPanel; // Panel for course details
    public TMP_Text courseNameText; // UI text to show course name
    public TMP_Text courseSemesterText; // UI text to show course semester
    public TMP_Text courseCreditText; // UI text to show course credit hours
    public TMP_Text courseDurationText; // UI text to show course duration
    public TMP_Text courseDetailsText; // UI text to show course details
    public TMP_Text courseScopeText; // UI text to show course scope
    public TMP_Text courseCreatedAtText; // UI text to show course created at date





    private CourseData[] studentCoursesList;
    // private CourseData[] allCoursesList;
    private LeaderboardData[] allLeaderboardData;

    void Start()
    {
        Debug.Log("Loading Student with ID: " + PlayerPrefs.GetInt("Id") + " and Name: " + PlayerPrefs.GetString("FullName"));
        OnPanelSelectButtonClicked(3); // Default to leaderboard
        studentNameText.text = PlayerPrefs.GetString("FullName");
        // StartCoroutine(GetStudentCourseList());
        StartCoroutine(GetStudentScore());
        StartCoroutine(GetStudentCourseList());
        // StartCoroutine(GetAvailableCourses());
    }
    IEnumerator GetStudentScore(){
        string url = GameManager.Instance.BASE_URL + "api/get_student_score.php?student_id=" + PlayerPrefs.GetInt("Id");
        Debug.Log("Fetching updated score from: " + url);
        
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            webRequest.SetRequestHeader("Cache-Control", "no-cache, no-store, must-revalidate");
            webRequest.SetRequestHeader("Pragma", "no-cache");
            webRequest.SetRequestHeader("Expires", "0");
            
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error fetching score: " + webRequest.error);
            }
            else
            {
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log("Student Score Data: " + jsonResponse);
                
                Response response = JsonConvert.DeserializeObject<Response>(jsonResponse);
                if(response.StatusCode==200){
                    UsersData userdata = response.Data.ToObject<UsersData>();
                    Debug.Log("Updated Score: " + userdata.Score);
                    studentScoreText.text = userdata.Score.ToString();
                }else
                {
                    Debug.LogError("Error getting score: " + response.StatusMessage);
                }
            }
        }
    }

    private void HighLightButton(Button button)
    {
        // Reset all buttons to default color
        LeaderboardButton.GetComponent<Image>().color = Color.white;
        ViewAssignmentButton.GetComponent<Image>().color = Color.white;
        CourseMaterialButton.GetComponent<Image>().color = Color.white;
        OpenCoursesButton.GetComponent<Image>().color = Color.white;

        // Highlight the selected button
        button.GetComponent<Image>().color = Color.yellow;
    }

    public void OnPanelSelectButtonClicked(int type){
        // 0= leaderboard, 1= view assignment, 2 = course material, 3 = open courses
        switch (type)
        {
            case 0:
                HighLightButton(LeaderboardButton);
                OnLeaderboardPanelSelected();
                break;
            case 1:
                HighLightButton(ViewAssignmentButton);
                OnViewAssignmentPanelSelected();
                break;
            case 2:
                HighLightButton(CourseMaterialButton);
                OnCourseMaterialPanelSelected();
                break;
            case 3:
                HighLightButton(OpenCoursesButton);
                OnOpenCoursesPanelSelected();
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
        ViewAssignmentPanel.SetActive(false);
        CourseMaterialPanel.SetActive(false);
        OpenCoursesPanel.SetActive(false);

        // panel settings
        LeaderboardDetailsPanel.SetActive(false);
        LeaderboardSelectionPanel.SetActive(true);
        LeaderboardFeedbackText.text = "";
        if(studentCoursesList != null && studentCoursesList.Length > 0){

            
        // Load the course data for the dropdown
            leaderboardCourseDropdown.ClearOptions();
            foreach (CourseData course in studentCoursesList)
            {
                if(course.EnrollmentStatus==0) continue; // skip if not enrolled

                leaderboardCourseDropdown.options.Add(new TMP_Dropdown.OptionData(course.Name));
            }
            leaderboardCourseDropdown.RefreshShownValue();
            // Remove old listener to avoid multiple subscriptions
            leaderboardCourseDropdown.onValueChanged.RemoveAllListeners();

            // Add the listener for the dropdown
            leaderboardCourseDropdown.onValueChanged.AddListener(OnLeaderboardDropdownValueChanged);
            if (leaderboardCourseDropdown.options.Count > 0)
            {
                leaderboardCourseDropdown.value = 0;
                OnLeaderboardDropdownValueChanged(0); // Set the default selected value to the first option
            }
        }else{
            LeaderboardFeedbackText.text = "No courses available for leaderboard.";
            Debug.Log("No courses available for leaderboard.");
        }
        // getting new leaderboard data optinal way
        StopCoroutine(GetStudentCourseList());
    }
    string lastSelectedCourseNameForLeaderboard;
    void OnLeaderboardDropdownValueChanged(int index)
    {
        // Handle the dropdown value change
        Debug.Log("Selected course: " + leaderboardCourseDropdown.options[index].text);
        lastSelectedCourseNameForLeaderboard = leaderboardCourseDropdown.options[index].text;

    }
    public void OnLeaderboardViewButtonClicked()
    {
        // Show the leaderboard details panel
        LeaderboardDetailsPanel.SetActive(true);
        LeaderboardSelectionPanel.SetActive(false);
        LeaderboardFeedbackText.text = "";
        StartCoroutine(GetLeaderboardData());
        
    }

    IEnumerator GetStudentCourseList()
    {
        Debug.Log("Getting student courses list...");
        // get all courses for the student where he enrolled
        string url = GameManager.Instance.BASE_URL + "api/get_student_courses.php?student_id=" + PlayerPrefs.GetInt("Id");
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(webRequest.error);
            }
            else
            {
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log("Student Courses Data: " + jsonResponse);
                
                Response response = JsonConvert.DeserializeObject<Response>(jsonResponse);
                if(response.StatusCode==200){
                    studentCoursesList = response.Data.ToObject<CourseData[]>();
                    // StartCoroutine(GetLeaderboardData());
                }else
                {
                    Debug.Log("Error: " + response.StatusMessage);
                }
            }
        }
    }

    IEnumerator GetLeaderboardData()
    {
        string url = GameManager.Instance.BASE_URL + "api/get_leaderboard.php?course_name=" + lastSelectedCourseNameForLeaderboard;
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
            LeaderboardItem leaderboardItem = item.GetComponent<LeaderboardItem>();
            leaderboardItem.StudentPosition.text = rank.ToString();
            rank++;
            leaderboardItem.ScoreText.text = data.Score.ToString();
            leaderboardItem.StudentNameText.text = data.Name;
            leaderboardItem.leaderboardData = data;
        }
        yield return null; // Wait for the UI to update
    }


    // View Assignment section =========================================================================================
    public void OnViewAssignmentPanelSelected()
    {
        // Show the view assignment panel and hide others
        ViewAssignmentPanel.SetActive(true);
        LeaderboardPanel.SetActive(false);
        CourseMaterialPanel.SetActive(false);
        OpenCoursesPanel.SetActive(false);

        // panel settings
        ViewAssignmentDetailsPanel.SetActive(false);
        ViewAssignmentSelectionPanel.SetActive(true);
        ViewAssignmentFeedbackText.text = "";
        AssignmentDetailsText.text = "";

        // Load the course data for the dropdown
        selectCourseDropdownAssignment.ClearOptions();
        foreach (CourseData course in studentCoursesList)
        {
            if(course.EnrollmentStatus==0) continue; // skip if not enrolled

            selectCourseDropdownAssignment.options.Add(new TMP_Dropdown.OptionData(course.Name));
        }
        selectCourseDropdownAssignment.RefreshShownValue();
        
        // Remove old listener to avoid multiple subscriptions
        selectCourseDropdownAssignment.onValueChanged.RemoveAllListeners();

        // Add the listener for the dropdown
        selectCourseDropdownAssignment.onValueChanged.AddListener(OnViewAssignmentDropdownValueChanged);
        
        // Set the default selected value to the first option
        if (selectCourseDropdownAssignment.options.Count > 0)
        {
            selectCourseDropdownAssignment.value = 0;
            OnViewAssignmentDropdownValueChanged(0);
        }
    }

    string lastSelectedCourseNameAssignment;
    public void OnViewAssignmentDropdownValueChanged(int index)
    {
        // Handle the dropdown value change
        Debug.Log("Selected course: " + selectCourseDropdownAssignment.options[index].text);
        lastSelectedCourseNameAssignment = selectCourseDropdownAssignment.options[index].text;
    }

    public void OnViewAssignmentViewButtonClicked()
    {
        // Show the assignment details panel
        ViewAssignmentDetailsPanel.SetActive(true);
        ViewAssignmentSelectionPanel.SetActive(false);
        ViewAssignmentFeedbackText.text = "";

        // TODO: Get assignment details from server
        StartCoroutine(GetAssignmentDetails(lastSelectedCourseNameAssignment));
    }

    IEnumerator GetAssignmentDetails(string courseName)
    {
        // This function will get the assignment details from the server
        string url = GameManager.Instance.BASE_URL + "api/get_assignment_details.php?course_name=" + UnityWebRequest.EscapeURL(courseName);
        Debug.Log("URL: " + url);
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Show loading message
            AssignmentDetailsText.text = "Loading assignment details for " + courseName + "...";

            // Send the request and wait for the response
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                // Handle error
                Debug.LogError("Error fetching assignment details: " + webRequest.error);
                AssignmentDetailsText.text = "Failed to load assignment details. Please try again later.";
            }
            else
            {
                // Process the response
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log("Assignment Details Data: " + jsonResponse);

                Response response = JsonConvert.DeserializeObject<Response>(jsonResponse);
                if (response.StatusCode == 200)
                {
                    // Parse and display the assignment details
                    AssignmentData assignmentData = response.Data.ToObject<AssignmentData>();
                    AssignmentDetailsText.text = "Assignment for " + assignmentData.CourseName + ":\n\n" +
                        "Description: " + assignmentData.Description + "\n\n";
                }
                else
                {
                    // Handle server error
                    Debug.LogError("Error: " + response.StatusMessage);
                    // AssignmentDetailsText.text = "Error loading assignment details: " + response.StatusMessage;
                    AssignmentDetailsText.text = "No assignment available for this course.";
                }
            }
        }
    }

    public void OnUploadAssignmentButtonClicked()
    {
        OpenAssignmentFilePicker();
    }

    private string selectedAssignmentFilePath;

    public void OpenAssignmentFilePicker()
    {
        // Open the file browser dialog for picking a file (single file selection)
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Select Assignment PDF File", "", "pdf", false);

        if (paths.Length > 0)
        {
            // Display the selected file path
            Debug.Log("Assignment file selected: " + paths[0]);
            selectedAssignmentFilePath = Path.Combine(Application.persistentDataPath, Path.GetFileName(paths[0])).Replace("\\", "/");
            
            try {
                // Copy the file to persistent data path
                File.Copy(paths[0], selectedAssignmentFilePath, true);
                Debug.Log("Assignment file copied successfully to: " + selectedAssignmentFilePath);
                AssignmentDetailsText.text += "\n\nSelected file: " + Path.GetFileName(paths[0]) + "\nClick upload to submit your assignment.";
                
                // Upload the assignment immediately
                StartCoroutine(UploadAssignmentToServer());
            }
            catch (System.Exception e) {
                Debug.LogError("Error copying assignment file: " + e.Message);
                AssignmentDetailsText.text += "\n\nError selecting file: " + e.Message;
                selectedAssignmentFilePath = null;
            }
        }
        else
        {
            Debug.Log("No assignment file selected.");
            AssignmentDetailsText.text += "\n\nNo file selected.";
        }
    }

    IEnumerator UploadAssignmentToServer()
    {
        if (string.IsNullOrEmpty(selectedAssignmentFilePath) || !File.Exists(selectedAssignmentFilePath))
        {
            AssignmentDetailsText.text += "\n\nError: No file selected or file not found.";
            yield break;
        }

        AssignmentDetailsText.text += "\n\nUploading assignment...";
        
        string url = GameManager.Instance.BASE_URL + "api/upload_student_assignment.php";
        byte[] fileData = File.ReadAllBytes(selectedAssignmentFilePath);
        string fileName = Path.GetFileName(selectedAssignmentFilePath);
        
        WWWForm form = new WWWForm();
        form.AddField("student_id", PlayerPrefs.GetInt("Id"));
        form.AddField("course_name", lastSelectedCourseNameAssignment);
        form.AddBinaryData("file", fileData, fileName, "application/pdf");
        
        using (UnityWebRequest webRequest = UnityWebRequest.Post(url, form))
        {
            yield return webRequest.SendWebRequest();
            
            if (webRequest.result == UnityWebRequest.Result.ConnectionError || 
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error uploading assignment: " + webRequest.error);
                AssignmentDetailsText.text += "\n\nError uploading assignment: " + webRequest.error;
            }
            else
            {
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log("Assignment Upload Response: " + jsonResponse);
                
                Response response = JsonConvert.DeserializeObject<Response>(jsonResponse);
                if (response.StatusCode == 200)
                {
                    AssignmentDetailsText.text = response.StatusMessage;
                    
                    // Immediately update the score display - this is important
                    yield return new WaitForSeconds(1); // Wait a moment for the server to process
                    StartCoroutine(GetStudentScore());
                    
                    // Force another score update after a delay to ensure it's refreshed
                    yield return new WaitForSeconds(2);
                    StartCoroutine(GetStudentScore());
                }
                else
                {
                    AssignmentDetailsText.text += "\n\nError: " + response.StatusMessage;
                }
            }
        }
    }

    // Course Material section =========================================================================================
    public void OnCourseMaterialPanelSelected()
    {
        // Show the course material panel and hide others
        CourseMaterialPanel.SetActive(true);
        LeaderboardPanel.SetActive(false);
        ViewAssignmentPanel.SetActive(false);
        OpenCoursesPanel.SetActive(false);

        // panel settings
        CourseMaterialDetailsPanel.SetActive(false);
        CourseMaterialSelectionPanel.SetActive(true);
        CourseMaterialFeedbackText.text = "";
        MaterialDetailsText.text = "";

        // Load the course data for the dropdown
        selectCourseDropdownMaterial.ClearOptions();
        foreach (CourseData course in studentCoursesList)
        {
            if(course.EnrollmentStatus==0) continue; // skip if not enrolled
            selectCourseDropdownMaterial.options.Add(new TMP_Dropdown.OptionData(course.Name));
        }
        selectCourseDropdownMaterial.RefreshShownValue();
        
        // Remove old listener to avoid multiple subscriptions
        selectCourseDropdownMaterial.onValueChanged.RemoveAllListeners();

        // Add the listener for the dropdown
        selectCourseDropdownMaterial.onValueChanged.AddListener(OnCourseMaterialDropdownValueChanged);
        // Set the default selected value to the first option
        if (selectCourseDropdownMaterial.options.Count > 0)
        {
            selectCourseDropdownMaterial.value = 0;
            OnCourseMaterialDropdownValueChanged(0);
        }
    }

    string lastSelectedCourseNameMaterial;
    public void OnCourseMaterialDropdownValueChanged(int index)
    {
        // Handle the dropdown value change
        Debug.Log("Selected course: " + selectCourseDropdownMaterial.options[index].text);
        lastSelectedCourseNameMaterial = selectCourseDropdownMaterial.options[index].text;
    }

    public void OnCourseMaterialViewButtonClicked()
    {
        // Show the course material details panel
        CourseMaterialDetailsPanel.SetActive(true);
        CourseMaterialSelectionPanel.SetActive(false);
        CourseMaterialFeedbackText.text = "";

        // TODO: Get course material details from server
        StartCoroutine(GetCourseMaterialDetails(lastSelectedCourseNameMaterial));
    }
    string lastMaterialDownlaodURL;
    IEnumerator GetCourseMaterialDetails(string courseName)
    {
        
        // This function will get the course material details from the server
        string url = GameManager.Instance.BASE_URL + "api/get_course_material.php?course_name=" + UnityWebRequest.EscapeURL(courseName);
        Debug.Log("URL: " + url);
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Show loading message
            MaterialDetailsText.text = "Loading course material for " + courseName + "...";

            // Send the request and wait for the response
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                // Handle error
                Debug.LogError("Error fetching course material: " + webRequest.error);
                MaterialDetailsText.text = "Failed to load course material. Please try again later.";
            }
            else
            {
                // Process the response
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log("Course Material Data: " + jsonResponse);

                Response response = JsonConvert.DeserializeObject<Response>(jsonResponse);
                if (response.StatusCode == 200)
                {
                    // Parse and display the course material details
                    AssignmentData courseMaterialData = response.Data.ToObject<AssignmentData>();
                    MaterialDetailsText.text = "Course Material for " + courseMaterialData.CourseName + ":\n\n" +
                        "Description: " + courseMaterialData.Description + "\n\n";
                        // "File URL: " + courseMaterialData.FileURL;
                    DownloadMaterialButton.interactable = true; // Enable download button
                    lastMaterialDownlaodURL = courseMaterialData.FileURL;
                }
                else
                {
                    // Handle server error
                    Debug.LogError("Error: " + response.StatusMessage);
                    // MaterialDetailsText.text = "Error loading course material: " + response.StatusMessage;
                    MaterialDetailsText.text = "No course material available for this course.";
                    DownloadMaterialButton.interactable = false; // Disable download button
                }
            }
        }
    }

    public void OnDownloadMaterialButtonClicked()
    {
        StartCoroutine(DownloadAndSaveFile(lastMaterialDownlaodURL));
    }

    private IEnumerator DownloadAndSaveFile(string fileUrl)
    {
        string fileName = Path.GetFileName(fileUrl);
        
        // First download the file
        using (UnityWebRequest webRequest = UnityWebRequest.Get(fileUrl))
        {
            MaterialDetailsText.text = "Downloading file...";
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error downloading file: " + webRequest.error);
                MaterialDetailsText.text = "Failed to download file. Please try again later.";
                yield break;
            }
            
            // If download was successful, prompt user to select where to save
            byte[] fileData = webRequest.downloadHandler.data;
            
            // Show file save dialog - returns an array of paths
            var paths = StandaloneFileBrowser.SaveFilePanel("Save File", "", fileName, "pdf");
            
            if (paths.Length > 0 && !string.IsNullOrEmpty(paths))
            {
                try
                {
                    // Save the file to the user-selected location
                    File.WriteAllBytes(paths, fileData);
                    Debug.Log("File downloaded and saved to: " + paths);
                    MaterialDetailsText.text = "File downloaded successfully to user-selected location";
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error saving file: " + e.Message);
                    MaterialDetailsText.text = "Error saving file: " + e.Message;
                }
            }
            else
            {
                Debug.Log("Save operation cancelled by user");
                MaterialDetailsText.text = "Save operation cancelled";
            }
        }
    }

    // Open Courses section =========================================================================================
    public void OnOpenCoursesPanelSelected()
    {
        // Show the open courses panel and hide others
        OpenCoursesPanel.SetActive(true);
        LeaderboardPanel.SetActive(false);
        ViewAssignmentPanel.SetActive(false);
        CourseMaterialPanel.SetActive(false);

        // panel settings
        // OpenCoursesDetailsPanel.SetActive(false);
        // OpenCoursesSelectionPanel.SetActive(true);
        OpenCoursesFeedbackText.text = "";

        // Get available courses from server
        // StartCoroutine(GetAvailableCourses());
        StartCoroutine(GetStudentCourseList());
       Invoke(nameof( ShowCourseList),2f); // Show the available courses in the UI


    }
    void ShowCourseList()
    {
        // Clear existing course items
        foreach (Transform child in OpenCoursesPanelContent)
        {
            Destroy(child.gameObject);
        }

        // Instantiate and populate course items
        foreach (CourseData course in studentCoursesList)
        {
            GameObject item = Instantiate(CourseItemPrefab, OpenCoursesPanelContent);
            CourseDetailsItemEnroll courseItem = item.GetComponent<CourseDetailsItemEnroll>();
            courseItem.courseNameText.text = course.Name;
            courseItem.courseSemesterText.text = course.Semester;
            courseItem.courseCreditText.text = course.Credit_hours.ToString();
            courseItem.courseDurationText.text = course.Duration.ToString() ;
            if (course.EnrollmentStatus == 1)
            {
                courseItem.EnrollButtonText.text = "Unenroll";
                // courseItem.EnrollButton.interactable = false;
                courseItem.EnrollButton.onClick.AddListener(() => OnUnenrollButtonClicked(course.Name));
            }
            else
            {
                courseItem.EnrollButtonText.text = "Enroll";
                // courseItem.EnrollButton.interactable = true;
                courseItem.EnrollButton.onClick.AddListener(() => OnEnrollButtonClicked(course.Name));
            }
            courseItem.courseData = course; // Assign the course data to the item
            courseItem.FullPanelButton.onClick.AddListener(() => ShowCourseDetailsInUI(course)); 


        }
    }
    public void ShowCourseDetailsInUI(CourseData course){
        // lastSelectedCourse = course; // Store the selected course data
        // visiable course details panel, and there may have a edit option to edit the course details
        courseDetailsPanel.SetActive(true);
        courseNameText.text = "Course Name: "+ course.Name; // Set course name text
        courseSemesterText.text = "Semester Name: "+ course.Semester; // Set course semester text
        courseCreditText.text ="Credit Hours: "+ course.Credit_hours.ToString(); // Set course credit hours text
        courseDurationText.text = "Credit Duration: "+course.Duration.ToString(); // Set course duration text
        courseDetailsText.text = course.Details; // Set course details text
        courseScopeText.text = course.Scope; // Set course scope text
        courseCreatedAtText.text = course.Created_at; // Set course created at text

    }
    // IEnumerator GetAvailableCourses()
    // {
    //     // This function will get the available courses from the server
    //     string url = GameManager.Instance.BASE_URL + "api/get_courses.php";
    //     using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
    //     {
    //         // Show loading message
    //         OpenCoursesFeedbackText.text = "Loading available courses...";

    //         // Send the request and wait for the response
    //         yield return webRequest.SendWebRequest();

    //         if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
    //         {
    //             // Handle error
    //             Debug.LogError("Error fetching available courses: " + webRequest.error);
    //             OpenCoursesFeedbackText.text = "Failed to load available courses. Please try again later.";
    //         }
    //         else
    //         {
    //             // Process the response
    //             string jsonResponse = webRequest.downloadHandler.text;
    //             Debug.Log("Available Courses Data: " + jsonResponse);

    //             Response response = JsonConvert.DeserializeObject<Response>(jsonResponse);
    //             if (response.StatusCode == 200)
    //             {
    //                 allCoursesList = response.Data.ToObject<CourseData[]>();
    //                 ShowCourseList(); // Show the available courses in the UI
                    
    //             }
    //             else
    //             {
    //                 // Handle server error
    //                 Debug.LogError("Error: " + response.StatusMessage);
    //                 OpenCoursesFeedbackText.text = "Error loading available courses: " + response.StatusMessage;
    //             }
    //         }
    //     }
    // }

    public void OnEnrollButtonClicked(string courseName)
    {
        // TODO: Implement course enrollment functionality
        Debug.Log("Enrolling in course: " + courseName);
        OpenCoursesFeedbackText.text = "Enrolling in " + courseName + "... (Feature not yet implemented)";
        
        // Start a coroutine to enroll in the course
        StartCoroutine(EnrollInCourse(courseName));
    }

    IEnumerator EnrollInCourse(string courseName)
    {
        
        string url = GameManager.Instance.BASE_URL + "api/student_enrollment.php?student_id=" + PlayerPrefs.GetInt("Id") + "&course_name=" + UnityWebRequest.EscapeURL(courseName) + "&type=enroll";
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Show loading message
            OpenCoursesFeedbackText.text = "Enrolling in " + courseName + "...";

            // Send the request and wait for the response
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                // Handle error
                Debug.LogError("Error enrolling in course: " + webRequest.error);
                OpenCoursesFeedbackText.text = "Failed to enroll in course. Please try again later.";
            }
            else
            {
                // Process the response
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log("Enroll Course Response: " + jsonResponse);

                Response response = JsonConvert.DeserializeObject<Response>(jsonResponse);
                if (response.StatusCode == 200)
                {
                    OpenCoursesFeedbackText.text = "Successfully enrolled in " + courseName + "!";
                    OnPanelSelectButtonClicked(3); // Refresh the open courses panel
                    // StartCoroutine(GetAvailableCourses()); // Refresh the available courses list
                }
                else
                {
                    // Handle server error
                    Debug.LogError("Error: " + response.StatusMessage);
                    OpenCoursesFeedbackText.text = "Error enrolling in course: " + response.StatusMessage;
                }
            }
        }
    }

    public void OnUnenrollButtonClicked(string courseName)
    {
        StartCoroutine(UnenrollFromCourse(courseName));
    }
    IEnumerator UnenrollFromCourse(string courseName)
    {
        string url = GameManager.Instance.BASE_URL + "api/student_enrollment.php?student_id=" + PlayerPrefs.GetInt("Id") + "&course_name=" + UnityWebRequest.EscapeURL(courseName) + "&type=unenroll";
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Show loading message
            OpenCoursesFeedbackText.text = "Unenrolling from " + courseName + "...";

            // Send the request and wait for the response
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                // Handle error
                Debug.LogError("Error unenrolling from course: " + webRequest.error);
                OpenCoursesFeedbackText.text = "Failed to unenroll from course. Please try again later.";
            }
            else
            {
                // Process the response
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log("Unenroll Course Response: " + jsonResponse);

                Response response = JsonConvert.DeserializeObject<Response>(jsonResponse);
                if (response.StatusCode == 200)
                {
                    OpenCoursesFeedbackText.text = "Successfully unenrolled from " + courseName + "!";
                    // StartCoroutine(GetStudentCourseList()); // Refresh the student courses list
                    // StartCoroutine(GetAvailableCourses()); // Refresh the available courses list
                    OnPanelSelectButtonClicked(3); // Refresh the open courses panel

                }
                else
                {
                    // Handle server error
                    Debug.LogError("Error: " + response.StatusMessage);
                    OpenCoursesFeedbackText.text = "Error unenrolling from course: " + response.StatusMessage;
                }
            }
        }
    }

    public void OnLogoutButtonClicked()
    {
        // Load the login scene
        GameManager.Instance.Logout();
    }
}
[System.Serializable]
public class AssignmentData
{
    public string CourseName { get; set; }
    public string Description { get; set; }
    public string FileURL { get; set; }
}