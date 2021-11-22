using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

[System.Serializable]
[CreateAssetMenu(fileName = "Quest", menuName = "Quest")]
public class Quest : ScriptableObject
{

    // Struct for Links, that link a Potion to a new quest
    [System.Serializable]
    public struct Link
    {
        public Potion Potion;
        public Quest NextQuest;
    }

    // A List of Links for this quest
    public List<Link> Links;

    // The First Line of the dialogue before the choice of potion
    public DialogueLine PrecedingStartLine;
    // The first line of the dialogue after the choice of potion
    public DialogueLine SucceedingStartLine;

    // The current Line
    public DialogueLine CurrentLine;

    // Stuff for the Editor

    // The Position in the editor
    public Vector2 EditorPos;
    // The Title of the Quest
    public string Title;
    // A reference to all lines that are associatec with this quest, even if not connected to the dialogue tree
    public List<DialogueLine> Lines;

    // Constructor
    public Quest()
    {
        // Init Links
        Links = new List<Link>();
    }
    void OnEnable()
    {
        UpdateLinks();
    }

    /// <summary>
    /// Update the LinkList to include a Link for each Potion asset
    /// </summary>
    public void UpdateLinks()
    {
        // Get all Potion assets and put them in a list
        List<Potion> potions = AssetDatabase.FindAssets("t:Potion")
        .Select( e => AssetDatabase.GUIDToAssetPath(e))
        .Select( e => (Potion)AssetDatabase.LoadAssetAtPath(e, typeof(Potion)))
        .ToList();

        // Craete a new Link list 
        List<Link> links = new List<Link>();

        // Add old Links if the potion still exists
        foreach (Link link in Links)
        {
            if (potions.Contains(link.Potion))
            {
                links.Add(link);
                // Remove the potions from the potion list, we made earlier
                potions.Remove(link.Potion);
            }
        }
        
        // Each Potion that is still in the list, does not have a link in the potion
        foreach (Potion potion in potions)
        {
            // Add links to these potions that point to nothing 
            Link link = new Link();
            link.Potion = potion;
            links.Add(link);
        }

        // Set this Quests Links to the new links
        Links = links;
    }
}
