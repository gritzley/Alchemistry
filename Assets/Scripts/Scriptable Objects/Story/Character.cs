using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class Character : MonoBehaviour
{
    Quest currentQuest;
    TextDisplay textDisplay;
    public string Name;
    public Quest StartQuest;
    public List<Quest> Quests;
    public Potion LastGivenPotion;


    public DialogueNode CurrentDialogueLine
    {
        get { return currentQuest.CurrentLine; }
    }

    void OnEnable()
    {
        textDisplay = GetComponentInChildren<TextDisplay>();

        textDisplay.DisplayText(StartQuest.CurrentLine.Text);
    }
}