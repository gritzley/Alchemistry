using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moveable : MonoBehaviour
{
    // Flag to disable movement
    bool movementDisabled = false;
    // Flag to disable rotation;
    bool rotationDisabled = false;

    // The time it takes for a movement or turn animation to complete
    public float animationTime = 0.15f;

    /// <summary>
    /// Coroutine that turns the player until he is looking in a specified direction
    /// </summary>
    /// <param name="targetDirection">The direction the player is supposed to look at</param>
    public IEnumerator TurnTowards(Vector3 targetDirection)
    {
        if (!rotationDisabled)
        {
            // Disable inputs while moving
            rotationDisabled = true;

            // Save start position for lerp
            Vector3 startDirection = transform.forward;

            // Initialize alpha
            float alpha = 0;

            // rotate until looking in desired direction
            while (transform.forward != targetDirection)
            {
                // Calculate alpha by deltatime over time
                alpha = Mathf.Min(alpha + (Time.deltaTime / animationTime));

                // Lerp between initial direction and target direction
                Vector3 newDirection = Vector3.Lerp(startDirection, targetDirection, alpha);

                // Set the new rotation
                transform.rotation = Quaternion.LookRotation(newDirection);

                // Next frame
                yield return null;
            }

            // Re-enable inouts
            rotationDisabled = false;
        }
        // exit
        yield return null;
    }

    /// <summary>
    /// Coroutine that turns the player until his rotation matches a specified quaternion
    /// </summary>
    /// <param name="targetRotation">The desired rotation</param>
    public IEnumerator TurnTowards(Quaternion targetRotation) {
        if (!rotationDisabled)
        {
            // Disable inputs while moving
            rotationDisabled = true;

            // Save start rotation for lerp
            Quaternion startRotation = transform.rotation;

            // Initialize alpha
            float alpha = 0;

            // rotate until at desired rotation
            while (transform.rotation != targetRotation)
            {
                // Calculate alpha by deltatime over time
                alpha = Mathf.Min(alpha + (Time.deltaTime / animationTime));

                // Lerp between initial direction and target direction
                transform.rotation = Quaternion.Lerp(startRotation, targetRotation, alpha);

                // Next frame
                yield return null;
            }

            // Re-enable inouts
            rotationDisabled = false;
        }
    }


    /// <summary>
    /// Coroutine that moves the player to a target position in world space
    /// </summary>
    /// <param name="targetPosition">The positiont the player is supposed to move to</param>
    public virtual IEnumerator MoveTowards(Vector3 targetPosition)
    {
        if (!movementDisabled)
        {
            // Disable inputs while moving
            movementDisabled = true;

            // Save start position for lerp
            Vector3 startPosition = transform.position;

            // Initialize alpha
            float alpha = 0;

            // Move while not at target;
            while(transform.position != targetPosition)
            {
                // Calculate alpha by deltatime over time
                alpha = Mathf.Min(alpha + (Time.deltaTime / animationTime));

                // Lerp between initial position and target position
                transform.position = Vector3.Lerp(startPosition, targetPosition, alpha);

                // Next frame
                yield return null;
            }

            // Re-enable inputs
            movementDisabled = false;
        }
    }
}
