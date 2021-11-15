using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Character")]
public class Character : ScriptableObject
{
    // The name of the character
    public string Name;
    public Quest StartQuest;
    Quest currentQuest;

    public DialogueLine CurrentDialogueLine
    {
        get { return currentQuest.CurrentLine; }
    }
}