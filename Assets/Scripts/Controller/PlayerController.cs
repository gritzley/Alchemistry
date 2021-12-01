using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Moveable
{
    // The current PlayerPosition
    // Also used to set the inital PlayerPosition
    [SerializeField] PlayerPosition currentPosition;

    // The cardinal direction the player is facing in
    Vector3 cardinalDirection;

    // Reference to the camera
    Camera Camera;

    // Reference to the transform where held items are placed
    public Transform HandTransform;
    // The current held item
    public Pickupable HeldItem;
    // Flag to disable inputs when moving or turning to stop weird behavior from happening
    public bool InputDisabled = false;

    // Called once at the start of the game
    void Start()
    {
        Camera = GetComponentInChildren<Camera>();
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

    // Called every frame
    void Update()
    { 
        // INPUTS
        if (!InputDisabled)
        {
            // On Turn Buttons (A and D)
            if (Input.GetButtonDown("Turn"))
            {
                // Playerposition can disable turning. This is usefull for "special" positions where you look at things.
                if (!currentPosition.TurnDisabled)
                {
                    TurnCorner(Input.GetAxisRaw("Turn"));   
                }
            }

            // On Move Buttons (W and S)
            if (Input.GetButtonDown("Move"))
            {
                // Multiply the current facing direction by -1 when moving backwards
                Vector3 moveDirection = cardinalDirection * Input.GetAxisRaw("Move");
                // Get the next position in the given direction
                PlayerPosition nextPos = currentPosition.GetNextPosition(moveDirection);
                // If there is a next position, move there
                if (nextPos != null)
                {
                    MoveToPos(nextPos);
                }
            }




            // Handle Mouseclick Event
            if (Input.GetMouseButtonDown(0))
            {
                // Perform the raycast and store the result in hit. If a clickable was hit, handle the hit
                Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    hit.collider.gameObject.GetComponent<IClickable>()?.OnClick(this);
                }
            }

        }
    }

    /// <summary>
    /// Move the player to a new PlayerPosition and adjust the camera pitch
    /// </summary>
    /// <param name="newPos">The new PlayerPosition</param>
    void MoveToPos (PlayerPosition newPos)
    {
        InputDisabled = true;
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
        // InputDisabled = true;
        // Normalize the direction
        dir = Mathf.Sign(dir);
        
        // Set the target rotation to 90 degrees in the specified direction around the y axis
        cardinalDirection = Quaternion.Euler(0, dir * 90, 0) * cardinalDirection;

        // Turn towards that rotation
        StartCoroutine(TurnTowards(cardinalDirection));
    }

    public override void OnMovementEnd()
    {
        InputDisabled = false;
    }
}
