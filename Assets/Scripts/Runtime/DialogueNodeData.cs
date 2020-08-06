using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Class that keeps data about a dialogue node
 * 
 * @author ShifatKhan
 */
[Serializable]
public class DialogueNodeData
{
    public string GUID;
    public string dialogueText;
    public D_Dialogue dialogueObject;
    public Vector2 position;

}
