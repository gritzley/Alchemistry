using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "Quest")]
public class Quest : ScriptableObject
{
    public DialogueBit StartBit;

    DialogueBit currentBit;

    [System.Serializable] public struct Link
    {
        public Potion Potion;
        public Quest NextQuest;
    }

    public List<Link> Links;

    public Quest()
    {
        currentBit = StartBit;
    }
}
