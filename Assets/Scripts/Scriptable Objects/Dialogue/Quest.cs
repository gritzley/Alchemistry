using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "Quest")]
public class Quest : ScriptableObject
{

    [System.Serializable]
    public struct Link
    {
        public Potion Potion;
        public Quest NextQuest;
    }

    public List<Link> Links;

    public DialogueLine StartLine;
    private DialogueLine currentLine;
    public DialogueLine CurrentLine
    {
        get { return currentLine; }
    }
}
