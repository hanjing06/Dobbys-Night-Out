using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class OptionsController : MonoBehaviour
{
    public Button CharacterButton, SettingsButton, ControlsButton, HelpButton;
    
    void BackButton()
    {
        SceneManager.LoadScene(0);
    }

    void CharacterButtonClick()
    {
        CharacterButton.onClick.Invoke();
    }
}
