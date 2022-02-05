using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabFade : MonoBehaviour
{
    public void SetActive(bool active)
    {
        if (active)
            transform.LeanScaleY(1, 0.5f);
        else
            transform.LeanScaleY(0, 0.5f);
    }
}
