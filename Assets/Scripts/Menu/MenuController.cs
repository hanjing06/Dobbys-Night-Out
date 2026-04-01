using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menu
{
    public class MenuController : MonoBehaviour
    {

        public void OnPlayButton()
        {
            SceneManager.LoadScene("TheClinic"); //need to make it the scene to the actual game
        }

        public void OnQuitButton()
        {
            Application.Quit();
        }

        public void OnOptionsButton()
        {
            // HideAllPanel();
            SceneManager.LoadScene("OptionsMenu");

        }
    }
}
