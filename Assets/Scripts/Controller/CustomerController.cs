using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[ExecuteInEditMode]
public class CustomerController : MonoBehaviour
{
    public TextDisplay MainTextDisplay;
    public TextDisplay LeftAnswerTextDisplay;
    public TextDisplay RightAnswerTextDisplay;
    public Potion LastGivenPotion;
    public CustomerDefinition CustomerDefinition;
    private Quest currentQuest;
    private bool isReceivingPotion;
    private VisibilityTracker visibilityTracker;
    private bool isVisible => visibilityTracker.IsVisible;

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

        visibilityTracker = GetComponent<VisibilityTracker>();
    }

    void Update()
    {
        if (Input.GetButtonDown("3")) Debug.Log(String.Join(", ",
            visibilityTracker.childrenTrackers
            .Where(e => e.IsVisible)
            .Select(e => e.name)
            .ToArray()));
    }

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

    public void ReceivePotion(Potion potion)
    {
        if (isReceivingPotion)
        {
            LastGivenPotion = potion;
            ReceiveAnswer(0);
            isReceivingPotion = false;
        }
    }

    public void ReceiveAnswer(int answer)
    {
        if (currentQuest.AdvanceDialogue(answer))
        {
            HandleDialogueLine(CurrentDialogueLine);
        }
        else
        {
            MainTextDisplay.ClearLetters();
            LeftAnswerTextDisplay.ClearLetters();
            RightAnswerTextDisplay.ClearLetters();
            currentQuest = currentQuest.GetNextQuest(LastGivenPotion);
            Debug.Log("Next Quest: " + currentQuest.Title);
        }
    }
}