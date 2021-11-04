using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerPosition", menuName = "PlayerPosition")]
public class PlayerPosition : ScriptableObject
{
    // The neighbours to all cardinal directions
    [SerializeField] PlayerPosition North, East, South, West;
    // The world space position of this player position
    public Vector3 Position;
    // The Pitch of the camera at the current position
    public float Pitch = 0;
    // Determines if you can look around when you are at this position
    public bool TurnDisabled = false;

    public PlayerPosition GetNextPosition(Vector3 direction)
    {

        // Return the PlayerPosition in the cardinal direction the player is looking at
        if (direction == Vector3.forward) return North;
        if (direction == Vector3.left) return West;
        if (direction == Vector3.back) return South;
        if (direction == Vector3.right) return East;
        // if anything goes fucky, return null
        return null;
    }
}
