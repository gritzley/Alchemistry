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


    public DialogueLine CurrentDialogueLine
    {
        get { return currentQuest.CurrentLine; }
    }

    void OnEnable()
    {
        textDisplay = GetComponent<TextDisplay>();
        currentQuest = StartQuest;
        Say(currentQuest.CurrentLine.Text);

    }

    public void Say(string text)
    {
        textDisplay.DisplayText(text);
    }

    public void ReceiveAnswer(int answer)
    {
        bool didDialogueAdvance = currentQuest.AdvanceDialogue(answer);
        if (didDialogueAdvance)
        {
            Say(CurrentDialogueLine.Text);
        }
    }
}