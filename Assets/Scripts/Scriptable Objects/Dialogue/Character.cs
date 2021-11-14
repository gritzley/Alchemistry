using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Character")]
public class Character : ScriptableObject
{
    // The name of the character
    public string Name;
    public DialogueBit StartBit;
    DialogueBit currentBit;
}