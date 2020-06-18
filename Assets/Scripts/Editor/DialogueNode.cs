using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

/** Class that represents Nodes in the dialogue graph
 * 
 * @author ShifatKhan
 */
public class DialogueNode : Node
{
    public string GUID; // Node's unique ID

    public string dialogueText;

    public bool entryPoint = false; // Is this the START node or not.
}
