using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Newtonsoft.Json;
public class AdminManager : MonoBehaviour
{
     // Create Users panel reference
    public TMP_Text CreateHeaderText; // UI text to show the login header
    public TMP_InputField emailInput; // UI input field for email
    public TMP_InputField nameField; // UI input field for password
    public TMP_InputField passwordInput; // UI input field for password
    public TMP_Text feedbackText; // UI text to show feedback (e.g., success or error message)

    [Header("NAVIGATION BUTTONS")]
    public Button createTeacher; // Button for student login
    public Button createStudent; // Button for teacher login
    public Button createCourse; // Button for admin login
    public Button showCourses; // Button for super admin login
    public Button AssignCourse; // Button for super admin login

    [Header("Panels")]
    public GameObject createPanel; // Panel for student login
    public GameObject createCoursePanel; // Panel for teacher login
    public GameObject showCoursesPanel; // Panel for admin login
    public GameObject AssignCoursePanel; // Panel for super admin login

    [Header("create Course Panel")]
    // create course panel reference
    // SELECT `id`, `name`, `scope`, `details`, `credit_hours`, `duration`, `semester`, `created_at` FROM `courses` WHERE 1
    public TMP_InputField courseName; // UI input field for course name
    public TMP_InputField courseScope; // UI input field for course scope
    public TMP_InputField courseDetails; // UI input field for course details
    public TMP_InputField courseCreditHours; // UI input field for course credit hours
    public TMP_InputField courseDuration; // UI input field for course duration
    public TMP_InputField courseSemester; // UI input field for course semester
    public TMP_Text courseFeedbackText; // UI text to show feedback (e.g., success or error message)

    [Header("Edit Course Panel")]
    // edit course panel reference
    // public TMP_InputField editCourseName; // UI input field for course name
    // public TMP_InputField editCourseScope; // UI input field for course scope
    // public TMP_InputField editCourseDetails; // UI input field for course details
    // public TMP_InputField editCourseCreditHours; // UI input field for course credit hours
    // public TMP_InputField editCourseDuration; // UI input field for course duration
    // public TMP_InputField editCourseSemester; // UI input field for course semester
    // public TMP_Text editCourseFeedbackText; // UI text to show feedback (e.g., success or error message)

    [Header("Course List Panel")]
    // show courses list panel reference
    public GameObject CourseListItem;
    public GameObject CourseListParent; // Panel for student login

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

    // course data edit panel reference
    [Header("Course Data Edit Panel")]
    public GameObject courseDataEditPanel; // Panel for course data edit
    public TMP_InputField courseDataEditName; // UI input field for course name
    public TMP_InputField courseDataEditScope; // UI input field for course scope
    public TMP_InputField courseDataEditDetails; // UI input field for course details
    public TMP_InputField courseDataEditCreditHours; // UI input field for course credit hours
    public TMP_InputField courseDataEditDuration; // UI input field for course duration
    public TMP_InputField courseDataEditSemester; // UI input field for course semester
    public TMP_Text courseDataEditFeedbackText; // UI text to show feedback (e.g., success or error message)


    [Header("Assign Course Panel")]
    public TMP_Dropdown courseDropdownList;
    public TMP_Dropdown teacherDropdownList;
    public TMP_Text assigncourseFeed; // UI text to show feedback (e.g., success or error message)




    private string userType;




    void Start()
    {
        OnPanelSelectButtonClicked(0); // Default to student login
        GameManager.Instance.adminManager = this; // Set the GameManager's adminManager reference to this instance
    }
    private void HighLightButton(Button button)
    {
        // Reset all buttons to default color
        createTeacher.GetComponent<Image>().color = Color.white;
        createStudent.GetComponent<Image>().color = Color.white;
        createCourse.GetComponent<Image>().color = Color.white;
        showCourses.GetComponent<Image>().color = Color.white;
        AssignCourse.GetComponent<Image>().color = Color.white;

        // Highlight the selected button
        button.GetComponent<Image>().color = Color.yellow;
    }
    public void OnPanelSelectButtonClicked(int type){
        // type 0 = create student, 1 = create teacher, 2 = create course, 3 = show courses, 4 = assign course
        feedbackText.text = ""; // Clear feedback text
        emailInput.text = ""; // Clear email input field
        passwordInput.text = ""; // Clear password input field
        // disable all panels
        createPanel.SetActive(false); // Hide student login panel
        createCoursePanel.SetActive(false); // Hide teacher login panel
        showCoursesPanel.SetActive(false); // Hide admin login panel
        AssignCoursePanel.SetActive(false); // Hide super admin login panel

        if (type == 0){
            CreateHeaderText.text = "Create Student"; // Set header text for student login
            createPanel.SetActive(true); // Show student login panel
            HighLightButton(createStudent); // Highlight student button
            userType = "student"; // Set user type to student
        } else if (type == 1){
            CreateHeaderText.text = "Create Teacher"; 
            createPanel.SetActive(true); // Show teacher login panel
            HighLightButton(createTeacher); // Highlight teacher button
            userType = "teacher"; // Set user type to teacher

        } else if (type == 2){
            // create course panel
            createCoursePanel.SetActive(true); // Show admin login panel
            HighLightButton(createCourse); // Highlight admin button
            courseFeedbackText.text = ""; // Clear feedback text

        } else if (type == 3){
            // show courses panel
            showCoursesPanel.SetActive(true); // Show super admin login panel
            HighLightButton(showCourses); // Highlight admin button
            ViewAllCourses(); 
        } else if (type == 4){
            // assign course panel
            AssignCoursePanel.SetActive(true); // Show super admin login panel
            HighLightButton(AssignCourse); // Highlight admin button
            StartCoroutine(OnAssingCourseVewClicked()); // Call the coroutine to send the request
        } else {
            Debug.Log("Invalid type selected"); // Log error for invalid type
        }
    } 
    public void OnCreateUserButtonClicked(){
        // type 0 = create student, 1 = create teacher, 2 = create course, 3 = show courses, 4 = assign course
        string email = emailInput.text; // Get email from input field
        string password = passwordInput.text; // Get password from input field
        string name = nameField.text; // Get name from input field
        // Validate input fields
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(name))
        {
            feedbackText.text = "Please fill in all fields"; // Show error message
            return; // Exit the function if validation fails
        }
        // Construct the URL for the API request
        string url = GameManager.Instance.BASE_URL + "api/create_user.php?email=" + email + "&password=" + password + "&name=" + name + "&type=" + userType;
        Debug.Log("URL: " + url); // Log the URL for debugging
        // Start the coroutine to send the request
        StartCoroutine(AddUser(url)); // Call the coroutine to send the request


    }

    IEnumerator AddUser(string url)
    {
        // Create a new UnityWebRequest for the URL
        using (UnityEngine.Networking.UnityWebRequest webRequest = UnityEngine.Networking.UnityWebRequest.Get(url))
        {
            // Send the request and wait for a response
            yield return webRequest.SendWebRequest();

            // Check for network errors
            if (webRequest.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                feedbackText.text = "Error: " + webRequest.error;
            }
            else
            {
                // Handle the response from the server
                string response = webRequest.downloadHandler.text;
                Debug.Log("Response: " + response); // Log the response for debugging 
                Response res = JsonConvert.DeserializeObject<Response>(response);
                if(res.StatusCode == 200)
                {
                    feedbackText.text = userType+" added successfully!";
                    // Optionally, clear the input fields after successful addition
                    nameField.text = "";
                    emailInput.text = "";
                    passwordInput.text = "";
                    Invoke(nameof(HideFeedbackText), 2f); // Hide feedback text after 2 seconds
                }
                else
                {
                    feedbackText.text = res.StatusMessage;
                }
            }
        }
    }
    void HideFeedbackText()
    {
        feedbackText.text = ""; // Clear feedback text
    }

    public void OnCreateCourseButtonClicked(){
        // reset feedback text
        courseFeedbackText.text = ""; // Clear feedback text
        // Get course details from input fields
        string name = courseName.text; // Get course name from input field
        string scope = courseScope.text; // Get course scope from input field
        string details = courseDetails.text; // Get course details from input field
        string credit_hours = courseCreditHours.text; // Get course credit hours from input field
        string duration = courseDuration.text; // Get course duration from input field
        string semester = courseSemester.text; // Get course semester from input field
        // Validate input fields
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(scope) || string.IsNullOrEmpty(details) || string.IsNullOrEmpty(credit_hours) || string.IsNullOrEmpty(duration) || string.IsNullOrEmpty(semester))
        {
            courseFeedbackText.text = "Please fill in all fields"; // Show error message
            return; // Exit the function if validation fails
        }
        // Construct the url but post method
        string url = GameManager.Instance.BASE_URL + "api/create_course.php";
        Debug.Log("URL: " + url); // Log the URL for debugging
        // Start the coroutine to send the request
        StartCoroutine(CreateCourse(url)); // Call the coroutine to send the request
    }
    IEnumerator CreateCourse(string url)
    {
//         $name = isset($_POST['name']) ? trim($_POST['name']) : '';
// $scope = isset($_POST['scope']) ? trim($_POST['scope']) : '';
// $details = isset($_POST['details']) ? trim($_POST['details']) : '';
// $creditHours = isset($_POST['credit_hours']) ? (int)$_POST['credit_hours'] : 0;
// $duration = isset($_POST['duration']) ? trim($_POST['duration']) : '';
// $semester = isset($_POST['semester']) ? trim($_POST['semester']) : '';
        // create form data
        WWWForm form = new WWWForm();
        form.AddField("name", courseName.text);
        form.AddField("scope", courseScope.text);
        form.AddField("details", courseDetails.text);
        form.AddField("credit_hours", courseCreditHours.text);
        form.AddField("duration", courseDuration.text);
        form.AddField("semester", courseSemester.text);
        
        // Create a new UnityWebRequest for the URL
        using (UnityEngine.Networking.UnityWebRequest webRequest = UnityEngine.Networking.UnityWebRequest.Post(url, form))
        {
            // Send the request and wait for a response
            yield return webRequest.SendWebRequest();

            // Check for network errors
            if (webRequest.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                courseFeedbackText.text = "Error: " + webRequest.error;
            }
            else
            {
                // Handle the response from the server
                string response = webRequest.downloadHandler.text;
                Debug.Log("Response: " + response); // Log the response for debugging 
                Response res = JsonConvert.DeserializeObject<Response>(response);
                if(res.StatusCode == 200)
                {
                    courseFeedbackText.text = "Course added successfully!";
                    // Optionally, clear the input fields after successful addition
                    courseName.text = "";
                    courseScope.text = "";
                    courseDetails.text = "";
                    courseCreditHours.text = "";
                    courseDuration.text = "";
                    courseSemester.text = "";
                    Invoke(nameof(HideCourseFeedbackText), 2f); // Hide feedback text after 2 seconds
                }
                else
                {
                    courseFeedbackText.text = res.StatusMessage;
                }
            }
        }
    }
    void HideCourseFeedbackText()
    {
        courseFeedbackText.text = ""; // Clear feedback text
    }

    // public void OnEditCourseButtonClicked(){
    //     // reset feedback text
    //     courseFeedbackText.text = ""; // Clear feedback text
    //     // Get course details from input fields
    //    if(string.IsNullOrEmpty(editCourseName.text) || string.IsNullOrEmpty(editCourseScope.text) || string.IsNullOrEmpty(editCourseDetails.text) || string.IsNullOrEmpty(editCourseCreditHours.text) || string.IsNullOrEmpty(editCourseDuration.text) || string.IsNullOrEmpty(editCourseSemester.text))
    //     {
    //         editCourseFeedbackText.text = "Please fill in all fields"; // Show error message
    //         return; // Exit the function if validation fails
    //     }
    //     // Construct the url but post method
    //     string url = GameManager.Instance.BASE_URL + "api/edit_course.php";
    //     Debug.Log("URL: " + url); // Log the URL for debugging
    //     // Start the coroutine to send the request
    //     StartCoroutine(EditCourse(url)); // Call the coroutine to send the request
    

    // }
    // IEnumerator EditCourse(string url)
    // {
    //     // create form data
    //     WWWForm form = new WWWForm();
    //     form.AddField("name", editCourseName.text);
    //     form.AddField("scope", editCourseScope.text);
    //     form.AddField("details", editCourseDetails.text);
    //     form.AddField("credit_hours", editCourseCreditHours.text);
    //     form.AddField("duration", editCourseDuration.text);
    //     form.AddField("semester", editCourseSemester.text);
        
    //     // Create a new UnityWebRequest for the URL
    //     using (UnityEngine.Networking.UnityWebRequest webRequest = UnityEngine.Networking.UnityWebRequest.Post(url, form))
    //     {
    //         // Send the request and wait for a response
    //         yield return webRequest.SendWebRequest();

    //         // Check for network errors
    //         if (webRequest.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
    //         {
    //             editCourseFeedbackText.text = "Error: " + webRequest.error;
    //         }
    //         else
    //         {
    //             // Handle the response from the server
    //             string response = webRequest.downloadHandler.text;
    //             Debug.Log("Response: " + response); // Log the response for debugging 
    //             Response res = JsonConvert.DeserializeObject<Response>(response);
    //             if(res.StatusCode == 200)
    //             {
    //                 editCourseFeedbackText.text = "Course edited successfully!";
    //                 // Optionally, clear the input fields after successful addition
    //                 editCourseName.text = "";
    //                 editCourseScope.text = "";
    //                 editCourseDetails.text = "";
    //                 editCourseCreditHours.text = "";
    //                 editCourseDuration.text = "";
    //                 editCourseSemester.text = "";
    //                 Invoke(nameof(HideEditFeedbackText), 2f); // Hide feedback text after 2 seconds
    //             }
    //             else
    //             {
    //                 editCourseFeedbackText.text = res.StatusMessage;
    //             }
    //         }
    //     }
    // }
    // void HideEditFeedbackText()
    // {
    //     editCourseFeedbackText.text = ""; // Clear feedback text
    // }

    public void ViewAllCourses(){
        courseDataEditPanel.SetActive(false);
        courseDetailsPanel.SetActive(false);
        courseDataEditFeedbackText.text = ""; // Clear feedback text
        // get all courses from the server

        string url = GameManager.Instance.BASE_URL + "api/get_courses.php";
        Debug.Log("URL: " + url); // Log the URL for debugging
        // Start the coroutine to send the request
        StartCoroutine(GetCourses(url)); // Call the coroutine to send the request
    }
    private CourseData[] allCourses;
    IEnumerator GetCourses(string url, int id=0){
        if(id != 0){
            url += "?id=" + id;
        }
        // Create a new UnityWebRequest for the URL
        using (UnityEngine.Networking.UnityWebRequest webRequest = UnityEngine.Networking.UnityWebRequest.Get(url))
        {
            // Send the request and wait for a response
            yield return webRequest.SendWebRequest();

            // Check for network errors
            if (webRequest.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                courseFeedbackText.text = "Error: " + webRequest.error;
            }
            else
            {
                // Handle the response from the server
                string response = webRequest.downloadHandler.text;
                Debug.Log("Response: " + response); // Log the response for debugging 
                Response res = JsonConvert.DeserializeObject<Response>(response);
                if(res.StatusCode == 200)
                {
                    if(id == 0){
                        allCourses =  res.Data.ToObject<CourseData[]>();
                        ShowCoursesInUI(allCourses);
                    }
                    else{
                        CourseData course = res.Data.ToObject<CourseData>();
                        // Display the course details in the UI (e.g., in a list or table)
                    }
                }
                else
                {
                    courseFeedbackText.text = res.StatusMessage;
                }
            }
        }
    }
    void ShowCoursesInUI(CourseData[] courses){
        // destroy all previous courses
        foreach (Transform child in CourseListParent.transform) {
            Destroy(child.gameObject);
        }
        // create a course item for each course
        foreach (CourseData course in courses) {
            GameObject courseItem = Instantiate(CourseListItem, CourseListParent.transform);
            CourseDetailsItem courseDetailsItem = courseItem.GetComponent<CourseDetailsItem>();
            courseDetailsItem.courseNameText.text = course.Name;
            courseDetailsItem.courseSemesterText.text =  course.Semester;
            courseDetailsItem.courseCreditText.text =  course.Credit_hours.ToString();
            courseDetailsItem.courseDurationText.text = course.Duration.ToString();
            // set the course data to the item
            courseDetailsItem.courseData = course;
        }
    }
    // on click of course item, show the course details in the UI
    private CourseData lastSelectedCourse;
    public void ShowCourseDetailsInUI(CourseData course){
        lastSelectedCourse = course; // Store the selected course data
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
    // click on edit button from course details panel, show the course data edit panel
    public void OnEditCourseDataButtonClicked_ShowCourseEdit(){

        // get all data from the course details panel and set it to the edit course data panel
        courseDataEditPanel.SetActive(true); // Show course data edit panel
        courseDataEditName.text = lastSelectedCourse.Name; // Set course name text
        courseDataEditSemester.text = lastSelectedCourse.Semester; // Set course semester text
        courseDataEditCreditHours.text = lastSelectedCourse.Credit_hours.ToString(); // Set course credit hours text
        courseDataEditDuration.text = lastSelectedCourse.Duration.ToString(); // Set course duration text
        courseDataEditDetails.text = lastSelectedCourse.Details; // Set course details text
        courseDataEditScope.text = lastSelectedCourse.Scope; // Set course scope text
        // courseDataEditSemester.text = courseSemesterText.text; // Set course semester text
        // courseDataEditCreditHours.text = courseCreditText.text; // Set course credit hours text
        // courseDataEditDuration.text = courseDurationText.text; // Set course duration text
        // courseDataEditDetails.text = courseDetailsText.text; // Set course details text
        // courseDataEditScope.text = courseScopeText.text; // Set course scope text
        // clear the feedback text
        courseDataEditFeedbackText.text = ""; // Clear feedback text
    }

    public void OnEditCourseDataButtonClicked(){
        // reset feedback text
        courseDataEditFeedbackText.text = ""; // Clear feedback text
        // Get course details from input fields
        if(string.IsNullOrEmpty(courseDataEditName.text) || string.IsNullOrEmpty(courseDataEditScope.text) || string.IsNullOrEmpty(courseDataEditDetails.text) || string.IsNullOrEmpty(courseDataEditCreditHours.text) || string.IsNullOrEmpty(courseDataEditDuration.text) || string.IsNullOrEmpty(courseDataEditSemester.text))
        {
            courseDataEditFeedbackText.text = "Please fill in all fields"; // Show error message
            return; // Exit the function if validation fails
        }
        // Construct the url but post method
        string url = GameManager.Instance.BASE_URL + "api/edit_course.php";
        Debug.Log("URL: " + url); // Log the URL for debugging
        // Start the coroutine to send the request
        StartCoroutine(EditCourseData(url)); // Call the coroutine to send the request

    }
    IEnumerator EditCourseData(string url)
    {
        // create form data
        WWWForm form = new WWWForm();
        form.AddField("id", lastSelectedCourse.Id); // Add course ID to the form data
        form.AddField("name", courseDataEditName.text); // Add course name to the form data
        form.AddField("scope", courseDataEditScope.text); // Add course scope to the form data
        form.AddField("details", courseDataEditDetails.text); // Add course details to the form data
        form.AddField("credit_hours", courseDataEditCreditHours.text); // Add course credit hours to the form data
        form.AddField("duration", courseDataEditDuration.text); // Add course duration to the form data
        form.AddField("semester", courseDataEditSemester.text); // Add course semester to the form data
        // form.AddField("details", ); // Add course details to the form data
        // form.AddField("credit_hours", ); // Add course credit hours to the form data
        // form.AddField("duration", ); // Add course duration to the form data
        // form.AddField("semester", ); // Add course semester to the form data

        
        // Create a new UnityWebRequest for the URL
        using (UnityEngine.Networking.UnityWebRequest webRequest = UnityEngine.Networking.UnityWebRequest.Post(url, form))
        {
            // Send the request and wait for a response
            yield return webRequest.SendWebRequest();

            // Check for network errors
            if (webRequest.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                courseDataEditFeedbackText.text = "Error: " + webRequest.error;
            }
            else
            {
                // Handle the response from the server
                string response = webRequest.downloadHandler.text;
                Debug.Log("Response: " + response); // Log the response for debugging 
                Response res = JsonConvert.DeserializeObject<Response>(response);
                if(res.StatusCode == 200)
                {
                    courseDataEditFeedbackText.text = "Course edited successfully!";
                    // Optionally, clear the input fields after successful addition
                    courseDataEditName.text = "";
                    courseDataEditScope.text = "";
                    courseDataEditDetails.text = "";
                    courseDataEditCreditHours.text = "";
                    courseDataEditDuration.text = "";
                    courseDataEditSemester.text = "";
                    Invoke(nameof(HideCourseDataEditFeedbackText), 2f); // Hide feedback text after 2 seconds
                }
                else
                {
                    courseDataEditFeedbackText.text = res.StatusMessage;
                }
            }
        }
    }
    void HideCourseDataEditFeedbackText()
    {
        courseDataEditFeedbackText.text = "";
        courseDetailsPanel.SetActive(false); 
        courseDataEditPanel.SetActive(false); 
        OnPanelSelectButtonClicked(3);
    }

    IEnumerator OnAssingCourseVewClicked(){
        // get all courses from the server
        string url = GameManager.Instance.BASE_URL + "api/get_courses.php";
        Debug.Log("URL: " + url); // Log the URL for debugging
        yield return StartCoroutine(GetCoursesForView(url)); // Call the coroutine to send the request
        yield return StartCoroutine(GetTeachersForView(GameManager.Instance.BASE_URL + "api/get_users.php?type=teacher")); // Call the coroutine to send the request
        // clear old dropdown list
        courseDropdownList.ClearOptions(); // Clear the course dropdown list
        teacherDropdownList.ClearOptions(); // Clear the teacher dropdown list
        // make dropdown list for courses and teachers
        foreach (CourseData course in assignCoursesList) {
            // add course to the dropdown list
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(course.Name);
            courseDropdownList.options.Add(option);
        }
        foreach (UsersData teacher in assignTeachersList) {
            // add teacher to the dropdown list
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(teacher.Name);
            teacherDropdownList.options.Add(option);
        }

        // show default dropdown value
        courseDropdownList.value = 0; // Set default value for course dropdown list
        teacherDropdownList.value = 0; // Set default value for teacher dropdown list
        // refresh the dropdown list
        courseDropdownList.RefreshShownValue(); // Refresh the course dropdown list
        teacherDropdownList.RefreshShownValue(); // Refresh the teacher dropdown list
    }
    CourseData[] assignCoursesList;
    IEnumerator GetCoursesForView(string url, int id=0){
        if(id != 0){
            url += "?id=" + id;
        }
        // Create a new UnityWebRequest for the URL
        using (UnityEngine.Networking.UnityWebRequest webRequest = UnityEngine.Networking.UnityWebRequest.Get(url))
        {
            // Send the request and wait for a response
            yield return webRequest.SendWebRequest();

            // Check for network errors
            if (webRequest.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                // courseFeedbackText.text = "Error: " + webRequest.error;
                Debug.Log("Error: " + webRequest.error); // Log the error for debugging
            }
            else
            {
                // Handle the response from the server
                string response = webRequest.downloadHandler.text;
                Debug.Log("Response: " + response); // Log the response for debugging 
                Response res = JsonConvert.DeserializeObject<Response>(response);
                if(res.StatusCode == 200)
                {
                    if(id == 0){
                        // allCourses =  res.Data.ToObject<CourseData[]>();
                        // ShowCoursesInUI(allCourses);
                        assignCoursesList =  res.Data.ToObject<CourseData[]>();
                    }
                    else{
                        // CourseData course = res.Data.ToObject<CourseData>();
                        // Display the course details in the UI (e.g., in a list or table)
                        Debug.Log(" Course: " + res.Data.ToString()); // Log the course data for debugging
                    }
                }
                else
                {
                    courseFeedbackText.text = res.StatusMessage;
                }
            }
        }
    }
    UsersData[] assignTeachersList;
    // also get teachers list from the server
    IEnumerator GetTeachersForView(string url){
        // Create a new UnityWebRequest for the URL
        using (UnityEngine.Networking.UnityWebRequest webRequest = UnityEngine.Networking.UnityWebRequest.Get(url))
        {
            // Send the request and wait for a response
            yield return webRequest.SendWebRequest();

            // Check for network errors
            if (webRequest.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                // courseFeedbackText.text = "Error: " + webRequest.error;
                Debug.Log("Error: " + webRequest.error); // Log the error for debugging
            }
            else
            {
                // Handle the response from the server
                string response = webRequest.downloadHandler.text;
                Debug.Log("Response: " + response); // Log the response for debugging 
                Response res = JsonConvert.DeserializeObject<Response>(response);
                if(res.StatusCode == 200)
                {
                    // allCourses =  res.Data.ToObject<CourseData[]>();
                    // ShowCoursesInUI(allCourses);
                    assignTeachersList = res.Data.ToObject<UsersData[]>();
                }
                else
                {
                    // courseFeedbackText.text = res.StatusMessage;
                    Debug.Log("Error: " + res.StatusMessage); // Log the error for debugging
                }
            }
        }
    }

    public void OnAssignCourseButtonClicked(){
        // get all data from the course details panel and set it to the edit course data panel
        // get selected course id and teacher id from the dropdown list
        int courseId = assignCoursesList[courseDropdownList.value].Id; // Get selected course ID from the dropdown list
        int teacherId = assignTeachersList[teacherDropdownList.value].Id; // Get selected teacher ID from the dropdown list
        // construct the url for the api request
        string url = GameManager.Instance.BASE_URL + "api/assign_course.php?course_id=" + courseId + "&teacher_id=" + teacherId;
        Debug.Log("URL: " + url); // Log the URL for debugging
        // Start the coroutine to send the request
        StartCoroutine(AssignCourseTOTeacher(url)); // Call the coroutine to send the request

    }
    IEnumerator AssignCourseTOTeacher(string url)
    {
        // Create a new UnityWebRequest for the URL
        using (UnityEngine.Networking.UnityWebRequest webRequest = UnityEngine.Networking.UnityWebRequest.Get(url))
        {
            // Send the request and wait for a response
            yield return webRequest.SendWebRequest();

            // Check for network errors
            if (webRequest.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                assigncourseFeed.text = "Error: " + webRequest.error;
            }
            else
            {
                // Handle the response from the server
                string response = webRequest.downloadHandler.text;
                Debug.Log("Response: " + response); // Log the response for debugging 
                Response res = JsonConvert.DeserializeObject<Response>(response);
                if(res.StatusCode == 200)
                {
                    assigncourseFeed.text = "Course assigned successfully!";
                    Invoke(nameof(HideAssignCourseFeedbackText), 2f); // Hide feedback text after 2 seconds
                }
                else
                {
                    assigncourseFeed.text = res.StatusMessage;
                }
            }
        }
    }
    void HideAssignCourseFeedbackText()
    {
        assigncourseFeed.text = ""; // Clear feedback text
    }

    public  void LogOutButtonClicked(){
        // log out the user and go to the login panel
        GameManager.Instance.Logout(); // Call the LogOut method from GameManager
    }

}

// $courseData = [
//         "Id" => $courseId,
//         "Name" => $name,
//         "Scope" => $scope,
//         "Details" => $details,
//         "Credit_hours" => $creditHours,
//         "Duration" => $duration,
//         "Semester" => $semester,
//         "Created_at" => date("Y-m-d H:i:s")
//     ];
