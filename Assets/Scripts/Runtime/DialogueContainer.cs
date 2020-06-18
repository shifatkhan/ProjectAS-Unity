using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** ScriptableObject that keeps data about dialogue nodes and links (edges)
 * 
 * @author ShifatKhan
 */
[Serializable]
public class DialogueContainer : ScriptableObject
{
    public List<NodeLinkData> nodeLinks = new List<NodeLinkData>();
    public List<DialogueNodeData> dialogueNodeData = new List<DialogueNodeData>();
}
