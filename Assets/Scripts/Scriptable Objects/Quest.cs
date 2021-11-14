using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Encounter", menuName = "Encounter")]
public class Encounter : ScriptableObject
{
    public DialogueBit StartBit;

    DialogueBit currentBit;

    [System.Serializable] public struct Link
    {
        public Potion Potion;
        public Encounter NextEncounter;
    }

    public List<Link> Links;

    public Encounter()
    {
        currentBit = StartBit;
    }
}
