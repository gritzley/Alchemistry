using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Potion", menuName = "Potion")]
public class Potion : ScriptableObject
{
    // The name of the potion
    public string Name;
    
    // Struct for one step of the recipe
    [System.Serializable] public struct Step 
    {
        // The ingredient to be added
        public Ingredient Ingredient;
        // The time it needs to cook before the next step
        public float Time;

        public float ErrorMargin;
    }

    // The list of steps required to make the potion
    public List<Step> Recipe;

    // Validate wheter a given list of steps matches this potions recipe
    public bool ValidateSteps(List<Step> steps)
    {
        Debug.Log("----------");
        Debug.Log(Name);

        Debug.Log($"{steps.Count} / {Recipe.Count}");

        // If the numbers of steps doesn't match, it's wrong
        if (steps.Count != Recipe.Count) return false;

        Debug.Log("Recipe Count Fits");

        // Go through every step
        for (int i = 0; i < steps.Count; i++) {

            // References for ease of access
            Step target = Recipe[i];
            Step actual = steps[i];

            // If the ingredient is wrong, it's out
            if (target.Ingredient != actual.Ingredient) return false;

            Debug.Log($"Ingredient {target.Ingredient} fits");

            // Calculate the difference between how long it cooked and how long it was supposed to cook
            float deltaTime = Mathf.Abs(target.Time - actual.Time);
            Debug.Log($"Time difference is {deltaTime}. Error Margin is {target.ErrorMargin}");
            // If the time difference is greater than the steps error margin, it's out
            if (deltaTime > target.ErrorMargin) return false;
        }

        // If it's not out by now, the recipe is correct
        return true;
    }
}
