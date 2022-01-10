using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class KettleController : Interactible
{
    private bool cooking = false;
    private IngredientDefinition NewIngredient;
    private Light Light;
    private ParticleSystem Orchestra;
    private List<Potion.Step> Steps;

    public void OnEnable()
    {
        Light = GetComponentInChildren<Light>();
        Orchestra = GetComponentInChildren<ParticleSystem>();

        Steps = new List<Potion.Step>();
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
        Potion.Step step = new Potion.Step();
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
            Potion.Step last = Steps.Last();
            last.Time = Time.time - last.Time;
            Steps[Steps.Count - 1] = last;
        }
    }

    private void FinishPotion(PlayerController player)
    {
        Debug.Log("Finishing");
        Debug.Log(String.Join(", ", Steps.Select( e => e.Ingredient.name)));
        LockInLastStep();
        if (Steps.Count > 0)
        {
            List<Potion> Potions = Potion.GetAllPotionAssets().Where( e => e.ValidateSteps(Steps) ).ToList();
            if (Potions.Count != 1)
                Debug.Log("Failed Potion");
            else
            {
                Debug.Log(Potions[0].name);
                GameManager.Instance.CurrentCustomer.ReceivePotion(Potions[0]);
            }
        }

        Steps = new List<Potion.Step>();
    }
}
