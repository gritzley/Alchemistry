using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[System.Serializable]
[CreateAssetMenu(fileName = "DialogueLine", menuName = "DialogueLine")]
public class DialogueLine : ScriptableObject
{
    // Title is for editor only
    public string Title;

    // Text of the line
    public string Text;

    // Left and right answer options (there are always two answer options)
    public string AnswerLeft;
    public string AnswerRight;

    // If false, answers will not be displayed and next line will be NextRight
    public bool HasAnswers = true;

    // References to the next Lines for left and right answer option
    public DialogueLine NextLeft;
    public DialogueLine NextRight;

    // Position in Editor (relative to parent quest node)
    public Vector2 EditorPos = Vector2.zero;
}
