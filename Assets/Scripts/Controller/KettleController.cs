using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class KettleController : MonoBehaviour
{
    // Flag for cooking. Set to false to stop cooking
    public bool cooking = false;
    // Field for a new ingredient that is added to the pot
    public Ingredient NewIngredient;

    // Enumerator to handle cooking
    public IEnumerator Cooking() {
        // set cooking flag to true
        cooking = true;

        // Initialize steps
        List<Potion.Step> Steps = new List<Potion.Step>();

        // initialize reference to lastStep
        Potion.Step lastStep;

        // Initialize time
        float time = 0;

        // Keep cooking until stop flag is set
        while (cooking)
        {
            // Increase timer
            time += Time.deltaTime;
            // Handle a new ingredient in the pot
            if (NewIngredient != null)
            {
                // Create a new step
                Potion.Step step = new Potion.Step();
                // Set step ingredient
                step.Ingredient = NewIngredient;

                // If this is not the first step, set the previous steps cooking time
                if (Steps.Count > 0)
                {
                    // Elements from Lists of structs have to be changed the hard way
                    lastStep = Steps[Steps.Count - 1];
                    lastStep.Time = time;
                    Steps[Steps.Count - 1] = lastStep;
                }

                // Add the step to the list
                Steps.Add(step);

                // reset timer
                time = 0;

                // Set new ingredient to NULL again
                NewIngredient = null;
            }
            // Next frame
            yield return null;
        }

        // Set the cooking time for the last ingredient
        lastStep = Steps[Steps.Count - 1];
        lastStep.Time = time;
        Steps[Steps.Count - 1] = lastStep;

        // Filter out Potions that don't fit the recipe
        List<Potion> Potions = GameManager.Instance.Potions.Where( e => e.ValidateSteps(Steps) ).ToList();
        
        // If there are no or several potions that fit the recipe, make a failed potion
        if (Potions.Count != 1)
        {
            Debug.Log("Failed Potion");
        }
        // Otherwise, make the one potion that fits the recipe
        else {
            Debug.Log(Potions[0].Name);
        }
    }
}
