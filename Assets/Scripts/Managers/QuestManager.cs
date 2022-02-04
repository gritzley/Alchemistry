using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public List<Quest> Quests;

    void OnEnable()
    {
        foreach(Quest quest in Quest.GetAllQuestAssets().Where(e => !Quests.Contains(e)))
            Quests.Add(quest);
    }
}