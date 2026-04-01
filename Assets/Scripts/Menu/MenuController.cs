using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menu
{
    public class MenuController : MonoBehaviour
    {

        public void OnPlayButton()
        {

<<<<<<< HEAD
            SceneManager.LoadScene("Scenes/Level-1/TheClinic/TheClinic"); //need to make it the scene to the actual game
=======
            //SceneManager.LoadScene(2); //need to make it the scene to the actual game

            SceneManager.LoadScene("TheClinic"); //need to make it the scene to the actual game

>>>>>>> 160d460 (resolved conflicts)
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
