using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

[ExecuteInEditMode]
public class Ingredient : Pickupable
{
    public IngredientDefinition Definition;
    private Stack<Ingredient> _stackedIngredients;
    private Stack<Ingredient> StackedIngredients
    {
        get
        {
            if (_stackedIngredients == null)
                _stackedIngredients = new Stack<Ingredient>( GetComponentsInChildren<Ingredient>().Where(e => e != this) );

            return _stackedIngredients;
        }
    }
    public override void OnClick()
    {
        Ingredient heldItem = PlayerController.Instance.HeldItem as Ingredient;
        if (Definition.IsStackable && heldItem != null && heldItem.Definition == Definition)
        {
            AddToStack(heldItem);
            PlayerController.Instance.HeldItem = null;
        }
        else if (Definition.IsStackable && heldItem == null && StackedIngredients.Count > 0)
        {
            RemoveHighestFromStack().OnClick();
        }
        else base.OnClick();
    }

    public void AddToStack(Ingredient ingredient, bool animatePath = true)
    {
        StackedIngredients.Push(ingredient);
        ingredient.transform.parent = transform;
        Vector3 calculatedLocalPosition = Vector3.up * GetComponentInChildren<MeshCollider>().bounds.size.y * (StackedIngredients.Count);
#if UNITY_EDITOR
        animatePath &= EditorApplication.isPlaying;
#endif
        if (animatePath)
        {
            ingredient.transform.LeanRotate(transform.rotation.eulerAngles, 0.15f);
            ingredient.transform.LeanMoveLocal(calculatedLocalPosition, 0.15f);
        }
        else
        {
            ingredient.transform.rotation = transform.rotation;
            ingredient.transform.localPosition = calculatedLocalPosition;
        }
        ingredient.GetComponentInChildren<SendClickEvent>().Recipient = this;
    }

    public Ingredient RemoveHighestFromStack()
    {
        Ingredient highestInStack = StackedIngredients.Pop();
        highestInStack.GetComponentInChildren<SendClickEvent>().Recipient = highestInStack;
        highestInStack.enabled = true;
        return highestInStack;
    }
}
