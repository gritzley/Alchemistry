using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;
using System;
using System.Collections;

public class GameManager : MonoBehaviour
{
    // Instance of GameManager
    public static GameManager Instance;
    public CustomerController CurrentCustomer;
    public List<Quest> Quests;
    public GameObject IngredientPrefab;
    public FadeCamera fade;
    public List<PotionDefinition> Potions;
    public GameManager()
    {
        // Instantiate the GameManager. Throw an error if there are multiple GameManagers.
        Assert.IsNull(Instance, "There can only be one instance of GameManager");
        Instance = this;
    }

    private void OnEnable()
    {
        fade = PlayerController.Instance.GetComponentInChildren<FadeCamera>();
#if UNITY_EDITOR
        Potions = PotionDefinition.GetAllPotionAssets();
#endif
    }

    public void AdvanceScene()
    {
        new Task(SceneTransition());
    }

    private IEnumerator SceneTransition()
    {
        fade.Out();
        yield return new WaitForSeconds(1.5f);
        CurrentCustomer.HandleCurrentDialogueLine();
        fade.In();
    }
}