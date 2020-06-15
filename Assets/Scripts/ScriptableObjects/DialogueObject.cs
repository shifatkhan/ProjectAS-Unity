using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DialogueObject : ScriptableObject
{
    public string dialogueID;

    [TextArea]
    public string[] dialogue; //Are you shaking in your<color=red> <shake>Boots </color>!?

    public ResponseObject[] responseOptions;
    
    //public GameObject speaker; // TODO: maybe have a hold of the speaker's gameobject to display sprite
}
