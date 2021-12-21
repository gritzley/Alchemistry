using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Moveable
{
    [SerializeField] PlayerPosition currentPosition;
    Vector3 cardinalDirection;
    Camera fpCamera;
    public Transform HandTransform;
    public Pickupable HeldItem;
    [HideInInspector] public bool InAction;

    void OnEnable()
    {
        fpCamera = GetComponentInChildren<Camera>();
        // Set positiont o start position
        transform.position = currentPosition.Position;

        // Initially the character is facing north
        cardinalDirection = Vector3.forward;
        // Pitch the actual view direction down by the start positions pitch value
        Vector3 initalDirection = Vector3.Normalize(Quaternion.Euler(0, currentPosition.Pitch, 0) * cardinalDirection);
        // Set rotation to new rotation
        transform.rotation = Quaternion.LookRotation(initalDirection);

        // Init Hand
        HandTransform = transform.Find("Hand");
    }

    public void Turn() {
        if (!InAction && !currentPosition.TurnDisabled)
        {
            TurnCorner(Input.GetAxisRaw("Turn"));   
        }
    }

    public void Move(float sign) {
        Vector3 moveDirection = cardinalDirection * sign;
        PlayerPosition nextPos = currentPosition.GetNextPosition(moveDirection);
        if (!InAction && nextPos != null)
        {
            MoveToPos(nextPos);
        }
    }

    public void Interact()
    {
        Ray ray = fpCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (!InAction && Physics.Raycast(ray, out hit))
        {
            hit.collider.gameObject.GetComponent<Clickable>()?.OnClick(this);
        }
    }

    /// <summary>
    /// Move the player to a new PlayerPosition and adjust the camera pitch
    /// </summary>
    /// <param name="newPos">The new PlayerPosition</param>
    void MoveToPos (PlayerPosition newPos)
    {
        // Pitch cardinal direction by the amount from new pos
        float pitchLat = newPos.Pitch * Mathf.Abs(cardinalDirection.z);
        float pitchLon = newPos.Pitch * -Mathf.Abs(cardinalDirection.x);
        Vector3 targetDirection = Vector3.Normalize(Quaternion.Euler(pitchLat, 0, pitchLon) * cardinalDirection);

        // Move to position
        StartCoroutine(MoveTowards(newPos.Position));
        // Turn to direction
        StartCoroutine(TurnTowards(targetDirection));

        // Overwrite the new position
        currentPosition = newPos;
    }

    /// <summary>
    /// Turn the player in a 90 degree angle
    /// </summary>
    /// <param name="dir">The turn direction. 1 for clockwise and -1 for anticlockwise</param>
    void TurnCorner (float dir)
    {
        // Normalize the direction
        dir = Mathf.Sign(dir);
        
        // Set the target rotation to 90 degrees in the specified direction around the y axis
        cardinalDirection = Quaternion.Euler(0, dir * 90, 0) * cardinalDirection;

        // Turn towards that rotation
        StartCoroutine(TurnTowards(cardinalDirection));
    }
}
