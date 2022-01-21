using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[ExecuteInEditMode]
public class CustomerController : Interactible
{
    public DiegeticText MainTextDisplay;
    public DiegeticText LeftAnswerTextDisplay;
    public DiegeticText RightAnswerTextDisplay;
    public PotionDefinition LastGivenPotion;
    public CustomerDefinition CustomerDefinition;
    public Quest currentQuest;
    private bool isReceivingPotion;
    public DialogueLine CurrentDialogueLine => currentQuest.CurrentLine;

    void OnEnable()
    {
        currentQuest = CustomerDefinition.StartQuest;
        RightAnswerTextDisplay.OnClickCallback = () => ReceiveAnswer(0);
        LeftAnswerTextDisplay.OnClickCallback = () => ReceiveAnswer(1);
        HandleCurrentDialogueLine();
    }

    public void HandleCurrentDialogueLine() => HandleDialogueLine(CurrentDialogueLine);
    void HandleDialogueLine(DialogueLine line)
    {
        LeftAnswerTextDisplay.ClearLetters();
        RightAnswerTextDisplay.ClearLetters();

        Action displayAnswers = null;
        if (line.IsReceivingState)
        {
            isReceivingPotion = true;
            SetAnswersActive(false, false);
        }
        else if (line.HasAnswers)
        {
            displayAnswers = () => SetAnswerTexts(line.AnswerLeft, line.AnswerRight);
            SetAnswersActive(true, true);
        }
        else
        {
            displayAnswers = () => SetAnswerTexts(String.Empty, "Continue");
            SetAnswersActive(false, true);
        }
        Say(line.Text, displayAnswers);
    }
    
    private void SetAnswerTexts(string both) => SetAnswerTexts(both, both);
    private void SetAnswerTexts(string left, string right)
    {
        LeftAnswerTextDisplay.DisplayText(left);
        RightAnswerTextDisplay.DisplayText(right);
    }
    private void SetAnswersActive(bool left, bool right)
    {
        LeftAnswerTextDisplay.ClickActive = left;
        RightAnswerTextDisplay.ClickActive = right;
    }

    public void Say(string text, Action callback = null)
    {
        MainTextDisplay.DisplayText(text, callback);
    }

    public void ReceivePotion(PotionDefinition potion)
    {
        if (isReceivingPotion)
        {
            MainTextDisplay.ClearLetters();
            LeftAnswerTextDisplay.ClearLetters();
            RightAnswerTextDisplay.ClearLetters();
            LastGivenPotion = potion;
            isReceivingPotion = false;
            ReceiveAnswer(0);
        }
    }

    public void ReceiveAnswer(int answer)
    {
        if (currentQuest.AdvanceDialogue(answer))
            HandleDialogueLine(CurrentDialogueLine);
        else
            AdvanceQuest();
    }

    private void AdvanceQuest()
    {
        currentQuest = currentQuest.GetNextQuest(LastGivenPotion);
        GameManager.Instance.AdvanceScene(currentQuest);
    }

    public override bool OnInteract(PlayerController player)
    {
        if (isReceivingPotion && (player.HeldItem as Potion)?.Definition != null)
            ReceivePotion((player.HeldItem as Potion).Definition);

        return true;
    }
}