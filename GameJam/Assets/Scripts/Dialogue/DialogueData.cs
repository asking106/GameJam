using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueChoice
{
    public string text;
    public int nextNodeIndex = -1; // -1 means end conversation
}

[System.Serializable]
public class DialogueNode
{
    [TextArea]
    public string text;
    public List<DialogueChoice> choices = new List<DialogueChoice>();
    public int nextNodeIndex = -1; // used when there are no choices for linear flow; -1 means end
    
    // Speaker metadata (optional)
    public string speakerName;
    public Sprite speakerPortrait;
}

[CreateAssetMenu(fileName = "NewDialogueData", menuName = "Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    public List<DialogueNode> nodes = new List<DialogueNode>();
} 