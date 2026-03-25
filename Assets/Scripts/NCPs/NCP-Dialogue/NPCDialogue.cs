using UnityEngine;

[CreateAssetMenu(fileName = "NPCDialogue", menuName = "Scriptable Objects/NPCDialogue")]
public class NPCDialogue : ScriptableObject// makes a data container for Npc Dialogue so you can reuse it for multipule time for diffrent objects
{
    public string npcName;
    public Sprite npcPortrait;// small photo that appears when taking to a npc
    public bool[] autoProgressLines;// to make sure the line moves to the next one without player interaction
    public string[] dialogueLines;// the words you want to say
    public float typingSpeed = 0.05f;
    
    public AudioClip voiceSound;// if we want to add sound for talking
    public float voicePitch = 1f;
    public float autoProgressSpeed = 1.5f; //speed of auto progresstion
}
