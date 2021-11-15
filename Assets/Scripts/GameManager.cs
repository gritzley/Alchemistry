using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    // Instance of GameManager
    public static GameManager Instance;
    public List<Potion> Potions;

    public Character test_Character;
    public GameManager()
    {
        // Instantiate the GameManager. Throw an error if there are multiple GameManagers.
        Assert.IsNull(Instance, "There can only be one instance of GameManager");
        Instance = this;
    }

    void Awake()
    {
        // Load all Potions
        Potions = AssetDatabase.FindAssets("t:Potion")
        .Select( guid => {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            return AssetDatabase.LoadMainAssetAtPath(path) as Potion;
        })
        .ToList();
    }

    void Start()
    {
        // DialogueLine line = test_Character.CurrentDialogueLine;
        // Debug.Log(line.Text);
    }
}