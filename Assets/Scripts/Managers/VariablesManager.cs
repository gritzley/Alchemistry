using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
public class VariablesManager : MonoBehaviour
{
    private static VariablesManager Instance;
    private Dictionary<string, string> Variables;
    public VariablesManager()
    {
        Assert.IsNull(Instance, "You are trying to create a second instance of VariableManager.");
        Variables = new Dictionary<string, string>();
        Instance = this;
    }

    public static void SetVariable(string name, string value)
    {
        if (Instance.Variables.ContainsKey(name))
            Instance.Variables[name] = value;
        else
            Instance.Variables.Add(name, value);
    }
    public static string GetVariable(string name)
    {
        if (Instance.Variables.ContainsKey(name))
            return Instance.Variables[name];
        else
            return null;
    }
}
