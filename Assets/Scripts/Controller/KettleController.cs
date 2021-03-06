using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class KettleController : MonoBehaviour
{
    private IngredientDefinition NewIngredient;
    private Light Light;
    private ParticleSystem Orchestra;
    private List<PotionDefinition.Step> Steps;
    public PotionDefinition FailedPotionDefinition;
    public GameObject PotionPrefab;
    public Transform TransitionPoint;
    private float animationTime = 0.2f;

    public void OnEnable()
    {
        Light = GetComponentInChildren<Light>();
        Orchestra = GetComponentInChildren<ParticleSystem>();

        Steps = new List<PotionDefinition.Step>();
    }

    public void OnMouseDown()
    {
        PlayerController player = PlayerController.Instance;

        if (player.HeldItem == null)
            FinishPotion(player);

        if (player.HeldItem != null && player.HeldItem is Ingredient)
            AddIngredient((player.HeldItem as Ingredient));
    }

    private void AddIngredient(Ingredient ingredient) => StartCoroutine(AddIngredientCoroutine(ingredient));
    private IEnumerator AddIngredientCoroutine(Ingredient ingredient)
    {
        if (ingredient.Definition.IsConsumable)
        {
            yield return ingredient.PickUp(TransitionPoint, animationTime);
            yield return ingredient.PickUp(transform, animationTime);
        }
        else
        {
            yield return ingredient.PickUp(TransitionPoint, animationTime);
            ingredient.transform.LeanRotateX(90, 0.2f);
            yield return new WaitForSeconds(0.5f);
            ingredient.transform.LeanRotateX(0, 0.2f);
            yield return new WaitForSeconds(0.5f);
            yield return ingredient.PickUp(PlayerController.Instance.HandTransform);
        }

        LockInLastStep();
        PotionDefinition.Step step = new PotionDefinition.Step();
        step.Ingredient = ingredient.Definition;
        step.Time = Time.time;
        Steps.Add(step);

        if (ingredient.Definition.IsConsumable)
            Destroy(ingredient.gameObject);
        
        yield return null;
    }

    /// <summary>
    /// Lock in the Time for the last step done, if there was one.
    /// </summary>
    private void LockInLastStep()
    {
        if (Steps.Count > 0)
        {
            PotionDefinition.Step last = Steps.Last();
            last.Time = Time.time - last.Time;
            Steps[Steps.Count - 1] = last;
        }
    }

    /// <summary>
    /// Finish the current Potion and fill its definiton in the potion held by the player
    /// </summary>
    /// <param name="potion"></param>
    private void FinishPotion(PlayerController player)
    {
        LockInLastStep();
        if (Steps.Count > 0)
        {            
            PotionDefinition finishedPotionDefiniton = FailedPotionDefinition;
            List<PotionDefinition> Potions = GameManager.Instance.Potions.Where( e => e.ValidateSteps(Steps) ).ToList();

            if (Potions.Count == 1) finishedPotionDefiniton = Potions[0];

            GameObject go = Instantiate(PotionPrefab);
            go.transform.parent = transform;
            go.transform.localPosition = Vector3.zero;
            go.transform.localEulerAngles = Vector3.zero;

            Potion finishedPotion = go.GetComponent<Potion>();
            finishedPotion.Definition = finishedPotionDefiniton;
            finishedPotion.UpdateName();
            player.HeldItem = finishedPotion;

            StartCoroutine(GivePotionToPlayer(finishedPotion));
        }

        Steps = new List<PotionDefinition.Step>();
    }
    private IEnumerator GivePotionToPlayer(Potion potion)
    {
        yield return potion.PickUp(TransitionPoint);
        yield return potion.PickUp(PlayerController.Instance.HandTransform);
    }
}
