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
        else if (line.NextRight == null)
        {
            AdvanceQuest();
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
            MainTextDisplay.ClearLetters();
            LeftAnswerTextDisplay.ClearLetters();
            RightAnswerTextDisplay.ClearLetters();
            LastGivenPotion = potion;
            ReceiveAnswer(0);
            isReceivingPotion = false;
            if (CurrentDialogueLine.NextRight == null) AdvanceQuest();
        }
    }

    public void ReceiveAnswer(int answer)
    {
        if (currentQuest.AdvanceDialogue(answer))
        {
            HandleDialogueLine(CurrentDialogueLine);
        }
    }

    private void AdvanceQuest()
    {
        Debug.Log("Advancing with " + LastGivenPotion.name);
        currentQuest = currentQuest.GetNextQuest(LastGivenPotion);
        new Task(Leave());
    }
    private IEnumerator Leave()
    {
        while (!isVisible) yield return new WaitForSeconds(0.1f);
        while (isVisible) yield return new WaitForSeconds(0.1f);
        gameObject.SetActive(false);
        GameManager.Instance.AdvanceScene();
    }
}