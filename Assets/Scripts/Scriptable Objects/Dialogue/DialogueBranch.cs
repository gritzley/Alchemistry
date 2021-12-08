using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class DialogueBranch : ScriptableObject
{
    [System.Serializable]
    public struct Link
    {
        public Potion Potion;
        public DialogueLine NextLine;
    }
    public List<Link> Links;

    public DialogueBranch()
    {
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