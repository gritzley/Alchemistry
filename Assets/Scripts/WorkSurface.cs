using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[ExecuteInEditMode]
public class WorkSurface : MonoBehaviour
{
    public Vector3 Size;
    public BoxCollider Surface;
    public BoxCollider LeftBounds;
    public BoxCollider RightBounds;
    public BoxCollider UpperBounds;
    public BoxCollider BackBounds;
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

    void OnValidate()
    {
        Surface.size = new Vector3(Size.x, 0, Size.z);
        LeftBounds.size = new Vector3(0, Size.y, Size.z);
        RightBounds.size = new Vector3(0, Size.y, Size.z);
        BackBounds.size = new Vector3(Size.x, Size.y, 0);
        UpperBounds.size = new Vector3(Size.x, 0, Size.z);
        LeftBounds.transform.localPosition = new Vector3(Size.x / -2, Size.y / 2, 0);
        RightBounds.transform.localPosition = new Vector3(Size.x / 2, Size.y / 2, 0);
        BackBounds.transform.localPosition = new Vector3(0, Size.y / 2, Size.z / 2);
        UpperBounds.transform.localPosition = new Vector3(0, Size.y, 0);
    }
}
