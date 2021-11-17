using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[CreateAssetMenu(fileName = "DialogueLine", menuName = "DialogueLine")]
public class DialogueLine : ScriptableObject
{
    public string Title;
    public string Text;
    public string AnswerLeft;
    public string AnswerRight;
    public bool HasAnswers = true;
    public DialogueLine NextLeft;
    public DialogueLine NextRight;

    public Vector2 EditorPos = Vector2.zero;
}
