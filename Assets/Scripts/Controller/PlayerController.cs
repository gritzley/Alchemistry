using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Moveable
{
    // The current PlayerPosition
    // Also used to set the inital PlayerPosition
    [SerializeField] PlayerPosition currentPosition;

    // Flag to disable inputs when moving or turning to stop weird behavior from happening
    bool inputDisabled = false;
    // The cardinal direction the player is facing in
    Vector3 cardinalDirection;

    // Reference to the camera
    Camera Camera;

    // Reference to the transform where held items are placed
    Transform handTransform;
    // The current held item
    Pickupable heldItem;

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
        handTransform = transform.Find("Hand");
    }

    // Called every frame
    void Update()
    { 
        // INPUTS ////////////////////////
        if (!inputDisabled)
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
                // Get a ray from the camera through the cursor
                Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
                // Initialize a raycast hit
                RaycastHit hit;
                // Perform the raycast and store the result in hit. If anything was hit, handle the hit
                if (Physics.Raycast(ray, out hit))
                {
                    // Get components of the hit gameobject for later reference
                    KettleController kettle = hit.collider.gameObject.GetComponent<KettleController>();
                    ItemSpot spot = hit.collider.gameObject.GetComponent<ItemSpot>();

                    // Empty-handed behaviour
                    if (heldItem == null)
                    {

                        // Handle ItemSpot hit
                        if (spot != null && spot.Item != null)
                        {
                            // Set heldItem to the item in the item spot
                            heldItem = spot.Item;
                            // set the spots item to null
                            spot.Item = null;
                            // Attach item to own transform 
                            heldItem.transform.parent = transform;

                            // disable input to prevent glitches while taking an item in hand
                            inputDisabled = true;

                            // Move item to position and rotation of handTransform
                            StartCoroutine(heldItem.MoveTowards(handTransform.position));
                            StartCoroutine(heldItem.TurnTowards(handTransform.rotation));

                            // C# does not allow yield return in annonymus functions so we define a new coroutine to reenambe input
                            // after held item animation ends.
                            IEnumerator coroutine () {
                                yield return new WaitForSeconds(heldItem.animationTime);
                                inputDisabled = false;
                            }
                            StartCoroutine(coroutine());
                        }

                        // Handle Kettle hit
                        if ( kettle != null )
                        {
                            // Stop cooking
                            kettle.cooking = false;
                        }

                    }
                    // Behaviours for when you are holding an item
                    else {
                        // Get held item components for later reference
                        IngredientContainer ingredientContainer = heldItem.gameObject.GetComponent<IngredientContainer>();
                        // Handle kettle hit
                        if (kettle != null) 
                        {
                            // if the held item is an ingredient
                            if (ingredientContainer != null)
                            {
                                // If the kettle is not already cooking, start now
                                if (!kettle.cooking) StartCoroutine(kettle.Cooking());

                                // Get the ingredient from the container
                                Ingredient ingredient = ingredientContainer.Ingredient;
                                // Add the ingredient to the pot
                                kettle.NewIngredient = ingredient;
                                // if the ingredient is used up, destroy it
                                if (ingredient.DestroyOnUse) Destroy(ingredientContainer.gameObject);
                            }
                        }

                        // Handle empty spot hit
                        if (spot != null && spot.Item == null)
                        {
                            // Set spots item to currently held item
                            spot.Item = heldItem;
                            // Set held item to null
                            heldItem = null;
                            // Set item parent to spot
                            spot.Item.transform.parent = spot.transform;

                            // Move item to the spots location with it's original rotation
                            StartCoroutine(spot.Item.MoveTowards(spot.transform.position));
                            StartCoroutine(spot.Item.TurnTowards(spot.Item.Rotation));
                        }

                    }
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
        // Pitch cardinal direction by the amount from new pos
        Vector3 targetDirection = Vector3.Normalize(Quaternion.Euler(newPos.Pitch, 0, 0) * cardinalDirection);

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
        dir /= Mathf.Abs(dir);
        
        // Set the target rotation to 90 degrees in the specified direction around the y axis
        cardinalDirection = Quaternion.Euler(0, dir * 90, 0) * cardinalDirection;

        // Turn towards that rotation
        StartCoroutine(TurnTowards(cardinalDirection));
    }
}
