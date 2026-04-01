using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menu
{
    public class MenuController : MonoBehaviour
    {

        public void OnPlayButton()
        {
<<<<<<< HEAD
            SceneManager.LoadScene(2); //need to make it the scene to the actual game
=======
            SceneManager.LoadScene("TheClinic"); //need to make it the scene to the actual game
>>>>>>> 0b3b3015902e1002634a9f6264c64701eaafd73f
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
