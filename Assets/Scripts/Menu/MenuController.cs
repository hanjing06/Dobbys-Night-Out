using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    void OnPlayButton()
    {
        SceneManager.LoadScene(1);
    }

    void OnQuitButton()
    {
        Application.Quit();
    }
}
