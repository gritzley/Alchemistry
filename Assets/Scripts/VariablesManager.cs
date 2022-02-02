using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
public class VariablesManager : MonoBehaviour
{
    private static VariablesManager _instance;
    private Dictionary<string, string> _variables;
    public VariablesManager()
    {
        Assert.IsNull(_instance, "YOu are trying to create a second instance of VariableManager.");
        _variables = new Dictionary<string, string>();
        _instance = this;

        Debug.Log("instanciated variables Manager");
    }

    public static void SetVariable(string name, string value) => _instance._variables.Add(name, value);
    public static string GetVariable(string name) => _instance._variables.ContainsKey(name) ? _instance._variables[name] : null;
}
