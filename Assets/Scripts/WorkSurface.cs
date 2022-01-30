using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorkSurface : MonoBehaviour
{

    void OnMouseDown()
    {
        if (PlayerController.Instance.HeldItem != null)
        {
            RaycastHit hit;
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit);
            
            StartCoroutine(PlaceItem(hit.point));
        }
    }

    IEnumerator PlaceItem(Vector3 position)
    {
        Pickupable item = PlayerController.Instance.HeldItem;
        Collider itemCollider = item.GetComponentInChildren<MeshCollider>();

        Vector3 direction;
        float distance;
        foreach (Collider collider in GetComponentsInChildren<MeshCollider>())
            if (Physics.ComputePenetration(
                collider, collider.transform.position, transform.rotation * collider.transform.localRotation,
                itemCollider, position, transform.rotation * itemCollider.transform.localRotation,
                out direction, out distance))
                    yield break;
        
        PlayerController.Instance.HeldItem = null;
        item.transform.LeanRotate(Vector3.zero, 0.15f);
        item.transform.LeanMove(position, 0.15f);
        item.transform.parent = transform;
        yield return new WaitForSeconds(0.15f);
    }

}
