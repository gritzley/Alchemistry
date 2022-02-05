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
        int newCount = (int) Mathf.Max(1, Count);
        yield return null;
        int coinCount = BaseCoin.GetComponentsInChildren<Ingredient>().Count();
        if (newCount > coinCount)
            for (int i = 0; i < newCount - coinCount; i++)
            {
#if UNITY_EDITOR
                BaseCoin.AddToStack((PrefabUtility.InstantiatePrefab(CoinPrefab) as GameObject).GetComponent<Ingredient>(), false);
#else
                BaseCoin.AddToStack((Instantiate(CoinPrefab) as GameObject).GetComponent<Ingredient>(), false);
#endif
            }
        else if (newCount < coinCount)
            for (int i = 0; i < coinCount - newCount; i++)
                DestroyImmediate(BaseCoin.RemoveHighestFromStack().gameObject);

        yield return null;
    }
}
