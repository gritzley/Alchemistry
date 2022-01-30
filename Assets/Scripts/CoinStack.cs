using UnityEngine;
using System.Linq;
using System.Collections;
using UnityEditor;

[ExecuteInEditMode]
public class CoinStack : MonoBehaviour
{
    public int Count;
    public Ingredient BaseCoin;
    public GameObject CoinPrefab;

    void OnValidate()
    {
        if (isActiveAndEnabled) StartCoroutine(UpdateStackSize());
    }

    IEnumerator UpdateStackSize()
    {
        if (Count < 1) yield break;
        yield return null;
        int coinCount = BaseCoin.GetComponentsInChildren<Ingredient>().Count();
        if (Count > coinCount)
            for (int i = 0; i < Count - coinCount; i++)
            {
                BaseCoin.AddToStack(Instantiate(CoinPrefab).GetComponent<Ingredient>(), false);
            }
        else if (Count < coinCount)
            for (int i = 0; i < coinCount - Count; i++)
                DestroyImmediate(BaseCoin.RemoveHighestFromStack().gameObject);

        yield return null;
    }
}
