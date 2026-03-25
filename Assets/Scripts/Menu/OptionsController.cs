using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class OptionsController : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button characterButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button controlsButton;
    [SerializeField] private Button helpButton;
    [SerializeField] private Button xButton;

    [Header("Panels")]
    [SerializeField] private GameObject characterPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject helpPanel;
    [SerializeField] private GameObject optionsMenu;

    [Header("Whole Menu")]
    [SerializeField] private GameObject optionsMenuRoot;

    private void Start()
    {
        characterButton.onClick.AddListener(OpenCharacterTab);
        settingsButton.onClick.AddListener(OpenSettingsTab);
        controlsButton.onClick.AddListener(OpenControlsTab);
        helpButton.onClick.AddListener(OpenHelpTab);

        if (xButton != null)
            xButton.onClick.AddListener(CloseOptionsMenu);

        OpenCharacterTab();
    }

    public void OpenCharacterTab()
    {
        HideAllPanels();
        characterPanel.SetActive(true);
    }

    public void OpenSettingsTab()
    {
        HideAllPanels();
        settingsPanel.SetActive(true);
    }

    public void OpenControlsTab()
    {
        HideAllPanels();
        controlsPanel.SetActive(true);
    }

    public void OpenHelpTab()
    {
        HideAllPanels();
        helpPanel.SetActive(true);
    }
    
    public static class GameState
    {
        public static bool openedFromMainMenu = true;
    }

    public void CloseOptionsMenu()
    {
        if (GameState.openedFromMainMenu)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            optionsMenu.SetActive(false);
        }
    }

    private void HideAllPanels()
    {
        characterPanel.SetActive(false);
        settingsPanel.SetActive(false);
        controlsPanel.SetActive(false);
        helpPanel.SetActive(false);
    }
}
