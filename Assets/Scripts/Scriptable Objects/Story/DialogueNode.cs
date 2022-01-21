using UnityEngine;

public abstract class DialogueNode : StoryNode
{
    public Quest ParentQuest;

#if UNITY_EDITOR

    public override void Remove()
    {
        ParentQuest.DialogueNodes.Remove(this);
        base.Remove();
    }

#endif

    /// <summary>
    /// This returns self when called on a DialogueLine or the next line when called on any other type of dialogue node
    /// </summary>
    public abstract DialogueLine NextLine { get; }
}