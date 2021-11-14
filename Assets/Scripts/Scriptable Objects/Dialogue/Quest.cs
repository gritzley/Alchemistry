using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "Quest")]
public class Quest : ScriptableObject
{
    public DialogueBit StartBit;

    DialogueBit currentBit;

    [System.Serializable]
    public struct Link
    {
        public Potion Potion;
        public Quest NextQuest;
    }

    public List<Link> Links;

    public Quest()
    {
        currentBit = StartBit;
        Links = new List<Link>();
        foreach (Potion potion in GameManager.Instance.Potions)
        {
            Link link = new Link();
            link.Potion = potion;
        }
    }
}
