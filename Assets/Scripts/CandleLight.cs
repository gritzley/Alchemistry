using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleLight : MonoBehaviour
{
    private Light light;
    [SerializeField] private float MaxMoveScale = 0.05f;

    private void OnEnable()
    {
        light = GetComponent<Light>();
        new Task(Flicker());
    }

    private IEnumerator Flicker()
    {
        while (true)
        {
            transform.LeanMoveLocalX(Random.Range(MaxMoveScale * -1, MaxMoveScale), 0.3f);
            transform.LeanMoveLocalY(Random.Range(MaxMoveScale * -1, MaxMoveScale), 0.3f);
            transform.LeanMoveLocalZ(Random.Range(MaxMoveScale * -1, MaxMoveScale), 0.3f);
            yield return new WaitForSeconds(0.3f);
        }
    }
}
