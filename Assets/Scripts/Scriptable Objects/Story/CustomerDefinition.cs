using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Customer Definition", menuName = "Customer Definition")]
public class CustomerDefinition : ScriptableObject
{
    Quest currentQuest;
    public string Name;
    public Quest StartQuest;
    public List<Quest> Quests;
}
