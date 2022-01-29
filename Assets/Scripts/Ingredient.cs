using UnityEngine;
using System.Collections;
using System.Linq;

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

        SendClickEvent sce = go.AddComponent<SendClickEvent>();
        sce.Recipient = this;
        MeshCollider mc = go.GetComponent<MeshCollider>();
        if (mc != null) mc.convex = true;
        
        foreach (MeshCollider _mc in go.GetComponentsInChildren<MeshCollider>())
        {
            _mc.convex = true;
            SendClickEvent _sce = _mc.gameObject.AddComponent<SendClickEvent>();
            _sce.Recipient = this;
        }

    }
}
