using UnityEngine;
using System.Collections.Generic;

public class Character : ScriptableObject
{
    // The name of the character
    public string Name;
    public Quest StartQuest;
    public List<Quest> Quests;
    Quest currentQuest;

    public DialogueNode CurrentDialogueLine
    {
        get { return currentQuest.CurrentLine; }
    }
}