using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaytesterMenu : MonoBehaviour
{
    public PlayerPosition StartPosition;
    
    public void StartGame() => PlayerController.Instance.MoveToPos(StartPosition);
}
