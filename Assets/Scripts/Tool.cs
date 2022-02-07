using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class Tool : Clickable
{
    [System.Serializable]
    private struct Conversion
    {
        public IngredientDefinition Input;
        public GameObject Output;
    }
    [SerializeField] private List<Conversion> Conversions;
    private Animator animator;
    private Ingredient input, output;
    public Transform InputSpot, OutputSpot;
    public float TransformationDelay = 0.75f;
    public float TransformationDuration = 0.75f;

    void OnEnable()
    {
        animator = GetComponent<Animator>();
    }

    public override void OnClick()
    {
        PlayerController player = PlayerController.Instance;
        if (player.HeldItem != null && player.HeldItem is Ingredient)
        {
            StartCoroutine(PutItemInTool(player.HeldItem as Ingredient));
            player.HeldItem = null;
        }
        else if (player.HeldItem == null && input != null)
        {
            StartCoroutine(TransformItem());
        }
    }

    IEnumerator PutItemInTool(Ingredient ingredient)
    {   
        input = ingredient;
        yield return ingredient.PickUp(InputSpot);
    }

    IEnumerator TransformItem()
    {
        Conversion[] conversions = Conversions.Where(e => e.Input == input.Definition).ToArray();
        if (conversions.Length == 1)
        {
            input.IsClickable = false;
#if UNITY_EDITOR
            GameObject go = PrefabUtility.InstantiatePrefab(conversions[0].Output) as GameObject;
#else
            GameObject go = Instantiate(conversions[0].Output) as GameObject;
#endif
            go.transform.parent = OutputSpot;
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.zero;
            output = go.GetComponent<Ingredient>();
            output.IsClickable = false;

            animator.Play("ToolMainAnimation", -1, 0f);
            yield return new WaitForSeconds(TransformationDelay);
            input.transform.LeanScale(Vector3.zero, TransformationDuration);
            output.transform.LeanScale(Vector3.one, TransformationDuration);
            yield return new WaitForSeconds(TransformationDuration);

            input = null;
            output.IsClickable = true;
        }
    }
}
