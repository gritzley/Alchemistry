using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    // Instance of GameManager
    public static GameManager Instance;
    public CustomerController CurrentCustomer;
    public GameManager()
    {
        // Instantiate the GameManager. Throw an error if there are multiple GameManagers.
        Assert.IsNull(Instance, "There can only be one instance of GameManager");
        Instance = this;
    }
}