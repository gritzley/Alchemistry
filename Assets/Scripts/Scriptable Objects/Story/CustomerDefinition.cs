using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;


[CreateAssetMenu(fileName = "Customer Definition", menuName = "Customer Definition")]
public class CustomerDefinition : ScriptableObject
{
    [NonSerialized] Quest currentQuest;
    public string Name;
    public Quest StartQuest;
    public List<Quest> Quests;
    public List<NewspaperArticle> Articles;

#if UNITY_EDITOR
    public static List<CustomerDefinition> GetAllCustomerDefinitions()
    {
        return AssetDatabase.FindAssets("t:CustomerDefinition")
        .Select( e => AssetDatabase.GUIDToAssetPath(e))
        .Select( e => (CustomerDefinition)AssetDatabase.LoadAssetAtPath(e, typeof(CustomerDefinition)))
        .ToList();
    }
#endif

}

