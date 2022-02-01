using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[ExecuteInEditMode]
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


        Vector3 direction;
        float distance;
        foreach( Collider itemCollider in item.GetComponentsInChildren<MeshCollider>())
        {
            Vector3 calculatedBoundsPosition = position + Vector3.up * itemCollider.bounds.extents.y;
            foreach (Collider collider in Physics.OverlapBox(calculatedBoundsPosition, itemCollider.bounds.extents, transform.rotation).Where( e => e.gameObject != gameObject))
            {
                bool convex = false;
                if (collider is MeshCollider)
                {
                    convex = (collider as MeshCollider).convex;
                    (collider as MeshCollider).convex = true;
                }            
                bool intersects = Physics.ComputePenetration(
                    collider, collider.transform.position, transform.rotation * collider.transform.localRotation,
                    itemCollider, position, transform.rotation * itemCollider.transform.localRotation,
                    out direction, out distance);

                if (collider is MeshCollider)
                    (collider as MeshCollider).convex = convex;

                if (intersects) yield break;
            }
        }
        
        PlayerController.Instance.HeldItem = null;
        item.transform.LeanRotate(Vector3.zero, 0.15f);
        item.transform.LeanMove(position, 0.15f);
        item.transform.parent = transform;
        Pickupable.SetLayerRecursively(item.gameObject, 0);
        yield return new WaitForSeconds(0.15f);
    }
}
