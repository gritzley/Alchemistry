using UnityEngine;

public abstract class DialogueNode : StoryNode
{
    public Quest ParentQuest;

    public override void Remove()
    {
        Debug.Log("tst");
        ParentQuest.DialogueNodes.Remove(this);
        base.Remove();
    }

    public abstract DialogueLine NextLine { get; }
}