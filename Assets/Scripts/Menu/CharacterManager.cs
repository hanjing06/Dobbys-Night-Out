using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    public class CharacterManager : MonoBehaviour
    {
        public CharacterDatabase db;
        public TMP_Text nameText;
        public Image characterSprite;

        private int selectedCharacter = 0;

        public void NextCharacter()
        {
            selectedCharacter++;
            if (selectedCharacter >= db.CharacterCount)
            {
                selectedCharacter = 0;
            }
            
            UpdateCharacter(selectedCharacter);
        }

        public void BackCharacter()
        {
            selectedCharacter--;
            if (selectedCharacter < 0)
            {
                selectedCharacter=db.CharacterCount - 1;
            }
            
            UpdateCharacter(selectedCharacter);
        }

        private void UpdateCharacter(int selectedCharacter)
        {
            Character character = db.GetCharacter(selectedCharacter);
            characterSprite.sprite = character.characterSprite;
            nameText.text = character.characterName;
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
}
