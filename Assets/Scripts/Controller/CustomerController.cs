using UnityEngine;
using System.Collections.Generic;
using System;

[ExecuteInEditMode]
public class CustomerController : MonoBehaviour
{
    public TextDisplay MainTextDisplay;
    public TextDisplay LeftAnswerTextDisplay;
    public TextDisplay RightAnswerTextDisplay;
    public Potion LastGivenPotion;
    public CustomerDefinition CustomerDefinition;
    Quest currentQuest;
    bool isReceivingPotion;

    public DialogueLine CurrentDialogueLine
    {
        get { return currentQuest.CurrentLine; }
    }

    void OnEnable()
    {
        currentQuest = CustomerDefinition.StartQuest;
        RightAnswerTextDisplay.OnClickCallback = () => ReceiveAnswer(0);
        LeftAnswerTextDisplay.OnClickCallback = () => ReceiveAnswer(1);
        HandleDialogueLine(CurrentDialogueLine);
    }

    void HandleDialogueLine(DialogueLine line)
    {
        LeftAnswerTextDisplay.ClearLetters();
        RightAnswerTextDisplay.ClearLetters();

        Action displayAnswers = null;
        if (line.NextRight == null)
        {
            isReceivingPotion = true;
        }
        else if (line.HasAnswers)
        {
            displayAnswers = () => {
                LeftAnswerTextDisplay.DisplayText(line.AnswerLeft);
                RightAnswerTextDisplay.DisplayText(line.AnswerRight);
            };
        }
        else
        {
            displayAnswers = () => {
                RightAnswerTextDisplay.DisplayText("Continue");
            };
        }
        Say(line.Text, displayAnswers);
    }

    public void Say(string text, Action callback = null)
    {
        MainTextDisplay.DisplayText(text, callback);
    }

    public void ReceivePotion(Potion potion)
    {
        if (isReceivingPotion)
        {
            currentQuest = currentQuest.GetNextQuest(potion);
            LastGivenPotion = potion;
            HandleDialogueLine(CurrentDialogueLine);
        }
    }

    public void ReceiveAnswer(int answer)
    {
        bool didDialogueAdvance = currentQuest.AdvanceDialogue(answer);
        if (didDialogueAdvance)
        {
            HandleDialogueLine(CurrentDialogueLine);
        }
    }
}