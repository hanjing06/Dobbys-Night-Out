using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menu
{
    public class MenuController : MonoBehaviour
    {

        public void OnPlayButton()
        {
            SceneManager.LoadScene("Scenes/Level-1/TheClinic/TheClinic"); //need to make it the scene to the actual game
        }

        public void OnQuitButton()
        {
            Application.Quit();
        }

        public void OnOptionsButton()
        {
            // HideAllPanel();
            SceneManager.LoadScene(1);

        }
    }
}
