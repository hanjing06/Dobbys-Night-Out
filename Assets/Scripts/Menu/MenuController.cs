using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitButton;
    void OnPlayButton()
    {
        SceneManager.LoadScene(1);
    }

    void OnQuitButton()
    {
        Application.Quit();
    }

    void OnOptionsButton()
    {
        // HideAllPanel();
    }
}
