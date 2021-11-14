using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GameManager : MonoBehaviour
{
    // Instantiate Gamemanager
    public static GameManager Instance;
    public GameManager()
    {
        Assert.IsNull(Instance, "There can only be one instance of GameManager");
        Instance = this;
    }
}