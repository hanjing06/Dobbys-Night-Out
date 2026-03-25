using Menu;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public CharacterDatabase db;
    public TMP_Text nameText;
    public Image characterSprite;

    private int selectedCharacter = 0;
    
    private void UpdateCharacter(int selectedCharacter)
    {
        Character character = db.GetCharacter(selectedCharacter);
        characterSprite.sprite = character.characterSprite;
    }

    private void Load()
    {
        selectedCharacter = PlayerPrefs.GetInt("SelectedCharacter");
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PlayerPrefs.HasKey("SelectedCharacter"))
        {
            selectedCharacter = 0;
        }
        else
        {
            Load();
        }
        UpdateCharacter(selectedCharacter);
    }
    }
