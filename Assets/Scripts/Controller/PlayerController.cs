using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerController : Moveable
{
    public static PlayerController Instance;    
    [SerializeField] private PlayerPosition currentPosition;
    private Camera fpCamera;
    [HideInInspector] public Transform HandTransform;
    [HideInInspector] public Pickupable HeldItem;
    [HideInInspector] public bool InAction;
    [SerializeField] private PlayerPosition hiddenMenu;
    [SerializeField] private PlayerPosition board;
    [SerializeField] private PlayerPosition pauseMenu;
    private bool _paused = false;
    public bool Paused
    {
        get => _paused;
        set
        {
            _paused = value;
            InAction = _paused;
            MoveCamera(_paused ? pauseMenu : currentPosition, 0.5f);
        }
    }
    PlayerController()
    {
        Assert.IsNull(Instance, "There can only be one instance of PlayerController");
        Instance = this;
    }
    void OnEnable()
    {
        fpCamera = GetComponentInChildren<Camera>();
        transform.position = currentPosition.transform.position;
        transform.rotation = currentPosition.transform.rotation;

        HandTransform = transform.Find("Hand");
    }

    public void Move(Vector3 direction) => MoveToPos(currentPosition.GetNextPosition(direction));
    void MoveToPos(PlayerPosition newPos, float seconds = 0)
    {
        if (newPos == null) return;
        
        if (seconds <= 0) seconds = animationTime;
        LeanTween.move(gameObject, newPos.transform.position, seconds);
        LeanTween.rotate(gameObject, newPos.transform.rotation.eulerAngles, seconds);
        
        currentPosition = newPos;
    }

    public void MoveCamera(PlayerPosition position, float seconds = 0) => MoveCamera(position.transform, seconds);
    public void MoveCamera(Transform target, float seconds = 0) => MoveCamera(target.position, target.rotation, seconds);
    public void MoveCamera(Vector3 position, Quaternion rotation, float seconds = 0)
    {
        if (seconds <= 0) seconds = animationTime;
        LeanTween.move(fpCamera.gameObject, position, seconds);
        LeanTween.rotate(fpCamera.gameObject, rotation.eulerAngles, seconds);
    }
    public void LookAtHiddenMenu(float seconds = 0) => MoveCamera(hiddenMenu, seconds);
    public void LookAtBoard(float seconds = 0) => MoveCamera(board, seconds);
    public void LookAtPauseMenu(float seconds = 0) => MoveCamera(pauseMenu, seconds);
}
