using UnityEngine;

public class Character : ScriptableObject
{
    // The name of the character
    public string Name;
    public Quest StartQuest;
    Quest currentQuest;

    public DialogueNode CurrentDialogueLine
    {
        get { return currentQuest.CurrentLine; }
    }
}