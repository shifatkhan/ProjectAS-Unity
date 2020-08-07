﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Holds data about a series of Dialogues. Each dialogue has a set amount of possible responses.
 * 
 * @author ShifatKhan
 */
[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/Dialogue")]
public class D_Dialogue : ScriptableObject
{
    public string dialogueID;

    public string GUID;

    public D_Character speaker;

    [TextArea]
    public List<string> dialogue; //Are you shaking in your<color=red> <shake>Boots </color>!?

    public List<D_Response> responseOptions;

    public bool entryPoint = false; // Is this the START node or not.

    public bool endPoint = false; // Is this a LEAF node or not.

    //public GameObject speaker; // TODO: maybe have a hold of the speaker's gameobject to display sprite
}