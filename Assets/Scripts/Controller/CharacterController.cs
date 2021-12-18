using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class CharacterController : MonoBehaviour
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
        textDisplay = GetComponent<TextDisplay>();
        Say(StartQuest.CurrentLine.Text);
    }

    public void Say(string text)
    {
        textDisplay.DisplayText(text);
    }
}