using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Camera.main.transform);

        transform.rotation = Quaternion.Euler(0.0f, transform.rotation.eulerAngles.y, 0.0f);
    }
}
