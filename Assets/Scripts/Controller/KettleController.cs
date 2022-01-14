using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class KettleController : Interactible
{
    private IngredientDefinition NewIngredient;
    private Light Light;
    private ParticleSystem Orchestra;
    private List<PotionDefinition.Step> Steps;
    public PotionDefinition FailedPotionDefinition;

    public void OnEnable()
    {
        Light = GetComponentInChildren<Light>();
        Orchestra = GetComponentInChildren<ParticleSystem>();

        Steps = new List<PotionDefinition.Step>();
    }

    public override bool OnInteract(PlayerController player)
    {
        if (player.HeldItem == null)
            FinishPotion(player);

        if (player.HeldItem != null && player.HeldItem is Ingredient)
            AddIngredient((player.HeldItem as Ingredient));

        return true;
    }

    private void AddIngredient(Ingredient ingredient)
    {
        LockInLastStep();
        PotionDefinition.Step step = new PotionDefinition.Step();
        step.Ingredient = ingredient.Definition;
        step.Time = Time.time;
        Steps.Add(step);

        if (ingredient.Definition.IsConsumable)
            Destroy(ingredient.gameObject);
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

    private void FinishPotion(PlayerController player)
    {
        LockInLastStep();
        if (Steps.Count > 0)
        {
            List<PotionDefinition> Potions = PotionDefinition.GetAllPotionDefinitions().Where( e => e.ValidateSteps(Steps) ).ToList();
            if (Potions.Count != 1)
            {
                Debug.Log("Failed Potion");
                GameManager.Instance.CurrentCustomer.ReceivePotion(FailedPotionDefinition);
            }
            else
            {
                Debug.Log(Potions[0].name);
                GameManager.Instance.CurrentCustomer.ReceivePotion(Potions[0]);
            }
        }

        Steps = new List<PotionDefinition.Step>();
    }
}
