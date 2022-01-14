using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerController : Moveable
{
    public static PlayerController Instance;
    [SerializeField] private Vector3 lookAtBoardPosition;
    [SerializeField] private PlayerPosition currentPosition;
    private Vector3 cardinalDirection;
    private Camera fpCamera;
    public Transform HandTransform;
    public Pickupable HeldItem;
    [HideInInspector] public bool InAction;

    PlayerController()
    {
        Assert.IsNull(Instance, "There can only be one instance of PlayerController");
        Instance = this;
    }
    void OnEnable()
    {
        fpCamera = GetComponentInChildren<Camera>();
        transform.position = currentPosition.Position;

        cardinalDirection = Vector3.forward;
        Vector3 initalDirection = Vector3.Normalize(Quaternion.Euler(0, currentPosition.Pitch, 0) * cardinalDirection);
        transform.rotation = Quaternion.LookRotation(initalDirection);

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
        if (!InAction)
        {
            Ray ray = fpCamera.ScreenPointToRay(Input.mousePosition);

            Debug.DrawRay(ray.origin, ray.direction, Color.green, 200.0f);

            // Get a list of all interactibles hit, sorted by distance, with the closest first;
            List<Interactible> interactibles = Physics.RaycastAll(ray)
            .OrderBy( e => e.distance)
            .Select( e => e.collider.gameObject.GetComponent<Interactible>())
            .Where( e => e != null)
            .ToList();

            int indexOfFirstSolid = interactibles.FindIndex( e => !(e is ItemSpot) );
            if (indexOfFirstSolid == -1)
            {
                interactibles
                .FindLastIndex( e => e.OnInteract(this) );
            }
            else if (!(bool)interactibles[indexOfFirstSolid].OnInteract(this))
            {
                interactibles
                .GetRange(0, indexOfFirstSolid)
                .FindLastIndex( e => e.OnInteract(this) );
            }
        }
    }

    /// <summary>
    /// Move the player to a new PlayerPosition and adjust the camera pitch
    /// </summary>
    /// <param name="newPos">The new PlayerPosition</param>
    void MoveToPos(PlayerPosition newPos)
    {
        float pitchLat = newPos.Pitch * Mathf.Abs(cardinalDirection.z);
        float pitchLon = newPos.Pitch * -Mathf.Abs(cardinalDirection.x);
        Vector3 targetDirection = Vector3.Normalize(Quaternion.Euler(pitchLat, 0, pitchLon) * cardinalDirection);

        StartCoroutine(MoveTowards(newPos.Position));
        StartCoroutine(TurnTowards(targetDirection));

        currentPosition = newPos;
    }

    /// <summary>
    /// Turn the player in a 90 degree angle
    /// </summary>
    /// <param name="dir">The turn direction. 1 for clockwise and -1 for anticlockwise</param>
    void TurnCorner (float dir)
    {
        dir = Mathf.Sign(dir);
        cardinalDirection = Quaternion.Euler(0, dir * 90, 0) * cardinalDirection;

        StartCoroutine(TurnTowards(cardinalDirection));
    }

    public void LookAtBoard(float seconds = 1.5f, float moveTime = 3.0f) => StartCoroutine(LookAtBoardTask(seconds, moveTime));
    public IEnumerator LookAtBoardTask(float seconds, float moveTime)
    {
        if (!InAction)
        {
            InAction = true;
            Vector3 returnPosition = transform.position;
            Vector3 returnRotation = transform.forward;
            Task MoveToBoard = new Task(MoveTowards(lookAtBoardPosition, moveTime));
            Task TurnToBoard = new Task(TurnTowards(Vector3.right, moveTime));

            while(MoveToBoard.Running || TurnToBoard.Running) yield return new WaitForSeconds(0.2f);
            yield return new WaitForSeconds(seconds);

            MoveToBoard = new Task(MoveTowards(returnPosition, moveTime));
            TurnToBoard = new Task(TurnTowards(returnRotation, moveTime));

            while(MoveToBoard.Running || TurnToBoard.Running) yield return new WaitForSeconds(0.2f);
            InAction = false;
        }
        yield return null;
    }
}
