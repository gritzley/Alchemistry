using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;
using System;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Instance of GameManager
    public static GameManager Instance;
    public CustomerController CurrentCustomer;
    public GameObject IngredientPrefab;
    [HideInInspector] public FadeCamera fade;
    [HideInInspector] public List<PotionDefinition> Potions;
    public Light BoardLight;
    public Image Article;
    public GameManager()
    {
        // Instantiate the GameManager. Throw an error if there are multiple GameManagers.
        Assert.IsNull(Instance, "There can only be one instance of GameManager");
        Instance = this;
    }

    private void OnEnable()
    {
        Article.gameObject.SetActive(false);
        BoardLight.enabled = false;
        fade = PlayerController.Instance.GetComponentInChildren<FadeCamera>();
#if UNITY_EDITOR
        Potions = PotionDefinition.GetAllPotionAssets();
#endif
    }

    public void AdvanceScene(SceneNode scene)
    {
        new Task(SceneTransition(scene));
    }

    private IEnumerator SceneTransition(SceneNode scene)
    {
        CurrentCustomer
        .GetComponentsInChildren<DiegeticText>()
        .ToList()
        .ForEach(e => e.ClickActive = false);

        fade.Out();
        yield return new WaitForSeconds(1.5f);
        fade.In();

        if (scene == null)
        {
            CurrentCustomer.gameObject.SetActive(false);
            PlayerController.Instance.MoveToHiddenMenu(1.0f);
        }
        else if (scene is Quest)
        {
            CurrentCustomer.currentQuest = scene as Quest;
            CurrentCustomer.HandleCurrentDialogueLine();
        }
        else if (scene is NewspaperArticle)
        {
            CurrentCustomer.gameObject.SetActive(false);
            PlayerController.Instance.MoveToBoard(1.0f);
            BoardLight.enabled = true;
            Article.sprite = (scene as NewspaperArticle).Sprite;
            Article.gameObject.SetActive(true);

            yield return new WaitForSeconds(5f);
            AdvanceScene(null);
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
}