using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class VisibilityTracker : MonoBehaviour
{
    public List<VisibilityTracker> childrenTrackers;
    private bool visible;
    private void OnBecameVisible() => visible = true;
    private void OnBecameInvisible() => visible = false;
    public bool IsVisible => visible || (childrenTrackers.Count > 0 && childrenTrackers.Exists(e => e.IsVisible));
    private void Awake()
    {
        childrenTrackers = new List<VisibilityTracker>();
        foreach(MeshRenderer mr in GetComponentsInChildren<MeshRenderer>().Where(e => e.gameObject != gameObject))
            childrenTrackers.Add(mr.gameObject.AddComponent<VisibilityTracker>());
    }
}