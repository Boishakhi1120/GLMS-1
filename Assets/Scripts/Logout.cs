using UnityEngine;
using UnityEngine.SceneManagement;

public class Logout : MonoBehaviour
{
    public string sceneName;
    public int sceneNo;
    public void LogoutButtonName()
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LogoutButtonNo()
    {
        SceneManager.LoadScene(sceneNo);
    }
}
