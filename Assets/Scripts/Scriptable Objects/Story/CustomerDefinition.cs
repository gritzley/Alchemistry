using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;


[CreateAssetMenu(fileName = "Customer Definition", menuName = "Customer Definition")]
public class CustomerDefinition : ScriptableObject
{
    [NonSerialized] Quest currentQuest;
    public string Name;
    public Quest StartQuest;
    public List<Quest> Quests;
}
