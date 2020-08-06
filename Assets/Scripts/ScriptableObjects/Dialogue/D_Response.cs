using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Holds data about response option to a certain Dialogue and which
 * Dialogue to display after this response was selected.
 * 
 * @author ShifatKhan
 */
[CreateAssetMenu(fileName = "New Response", menuName = "Dialogue/Response")]
public class D_Response : ScriptableObject
{
    public string responseText;

    public D_Dialogue dialogueObject;
}
