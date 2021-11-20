using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "Quest", menuName = "Quest")]
public class Quest : ScriptableObject
{

    [System.Serializable]
    public struct Link
    {
        public Potion Potion;
        public Quest NextQuest;
    }

    public List<Link> Links;

    public DialogueLine StartLine;
    private DialogueLine currentLine;
    public DialogueLine CurrentLine
    {
        get { return currentLine; }
    }

    public Vector2 EditorPos;
    public string Title;

    public List<DialogueLine> Lines;

    public Quest()
    {
        Lines = new List<DialogueLine>();
        Links = new List<Link>();
    }

    void OnEnable()
    {
        UpdateLinks();
    }
    public void UpdateLinks()
    {
        List<Potion> potions = AssetDatabase.FindAssets("t:Potion")
        .Select( e => AssetDatabase.GUIDToAssetPath(e))
        .Select( e => (Potion)AssetDatabase.LoadAssetAtPath(e, typeof(Potion)))
        .ToList();

        List<Link> links = new List<Link>();

        foreach (Link link in Links)
        {
            if (potions.Contains(link.Potion))
            {
                links.Add(link);
                potions.Remove(link.Potion);
            }
        }
        
        foreach (Potion potion in potions)
        {
            Link link = new Link();
            link.Potion = potion;
            links.Add(link);
        }

        Links = links;
    }
}
