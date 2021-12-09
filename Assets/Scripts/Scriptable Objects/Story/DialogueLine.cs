using UnityEngine;

[System.Serializable]
public class DialogueLine : ScriptableObject
{
    // Text of the line
    public string Text;

    // Left and right answer options (there are always two answer options)
    public string AnswerLeft;
    public string AnswerRight;

    // If false, answers will not be displayed and next line will be NextRight
    public bool HasAnswers = false;

    // References to the next Lines for left and right answer option
    public DialogueLine NextLeft;
    public DialogueLine NextRight;

    // Position in Editor (relative to parent quest node)
    public Vector2 EditorPos = Vector2.zero;
}
