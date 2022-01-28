using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Ingredient : Pickupable
{
    public IngredientDefinition Definition;

    void OnValidate()
    {
        if (isActiveAndEnabled)
        {
            StartCoroutine(ReplaceModel());
        }
    }

    IEnumerator ReplaceModel()
    {
        yield return null;
        foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
        {
            DestroyImmediate(mr.gameObject);
        }
        if (Definition != null)
        {
            AttachModel();
        }        
    }

    public void AttachModel()
    {
        GameObject go = Instantiate(Definition.Model);
        go.transform.parent = transform;
        go.transform.localPosition = Vector3.zero;
    }
}
