using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArticleAppear : MonoBehaviour
{
    void OnEnable()
    {
        // transform.localPosition += transform.forward * -100;
        gameObject.LeanMoveLocalZ(-100, 0);
        gameObject.LeanMoveLocalZ(0, 1.5f);
    }
}
