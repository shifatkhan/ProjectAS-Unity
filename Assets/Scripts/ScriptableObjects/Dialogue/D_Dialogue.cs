using System.Collections;
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

    [Header("Speaker")]
    public D_Character speaker;
    public bool isLeftSpeaker;

    [Header("Dialogues")]
    public float typingSpeed = 40f; // Letters per second.
    [TextArea]
    public List<string> dialogue; //Are you shaking in your<color=red> <shake>Boots </color>!?
    public D_Dialogue nextDialogue;

    [Header("Responses")]
    public List<D_Response> responseOptions;
}
