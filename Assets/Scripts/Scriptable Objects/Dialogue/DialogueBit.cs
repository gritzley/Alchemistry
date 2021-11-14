using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueBit", menuName = "DialogueBit")]
public class DialogueBit : ScriptableObject
{
    // The text the character is saying
    public string Text;

    // Structure of choices for dialogue bits
    [System.Serializable] public struct Choice
    {
        // The text to be displayed
        public string Text;
        // The bit this choice leads to
        public DialogueBit Next;
    }

    // A list of choices this bit offers
    public List<Choice> Choices;

    public DialogueBit()
    {
        // Initialize choices
        Choices = new List<Choice>();
    }
}