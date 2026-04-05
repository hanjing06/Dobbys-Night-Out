using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menu
{
    public class MenuController : MonoBehaviour
    {
        public string nextSceneName;

        public void OnPlayButton()
        {



            //SceneManager.LoadScene(2); //need to make it the scene to the actual game

            SceneManager.LoadScene("Scenes/Level 1 - Turkey Level/TheClinic/TheClinic"); //need to make it the scene to the actual game



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
