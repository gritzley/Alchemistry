public abstract class DialogueNode : StoryNode
{
    public Quest ParentQuest;

    public override void Remove()
    {
        ParentQuest.DialogueNodes.Remove(this);
        base.Remove();
    }
}