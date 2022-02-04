using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class IngredientDispenser : Clickable
{
    public bool IsEndless;
    public List<GameObject> IngredientModels;
    public IngredientDefinition Definition;
    private List<GameObject> currentlyHeldIngredients;
    private GameObject randomIngredient => currentlyHeldIngredients[new System.Random().Next(currentlyHeldIngredients.Count)];
    private bool hasIngredients => currentlyHeldIngredients.Count > 0;
    public AudioClip OnCutSound;
    
    private void OnEnable()
    {
        currentlyHeldIngredients = IngredientModels;
    }
    public override void OnClick()
    {
        PlayerController player = PlayerController.Instance;
        if (player.HeldItem == null && hasIngredients)
        {
            GameObject harvest = randomIngredient;
            GameObject go = Instantiate(harvest);
            go.transform.position = harvest.transform.position;
            go.transform.rotation = harvest.transform.rotation;
            Ingredient ingredient = go.AddComponent<Ingredient>();
            ingredient.Definition = Definition;
            if (!IsEndless) harvest.SetActive(false);
            StartCoroutine(GiveIngredientToPlayer(ingredient));
        }
    }

    private IEnumerator GiveIngredientToPlayer(Ingredient ingredient)
    {
        AudioManager.PlaySoundAtLocationOnBeat(OnCutSound, transform.position);
        
        yield return ingredient.PickUp(PlayerController.Instance.HandTransform);
        PlayerController.Instance.HeldItem = ingredient;
    }
}
