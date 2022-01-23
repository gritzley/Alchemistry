using System;
using System.Linq;
using System.Collections;
using UnityEngine;

public class PlayerPosition : MonoBehaviour
{
    public PlayerPosition W, A, S, D;
    public PlayerPosition GetNextPosition(Vector3 direction)
    {
        if ( direction == Vector3.forward) return W;
        if ( direction == Vector3.left) return A;
        if ( direction == Vector3.back) return S;
        if ( direction == Vector3.right) return D;

        throw new Exception("This should not happen.");
    }
}
