using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/** Handles the flow of a conversation. It also takes care of displaying the relevant 
 * dialogue information on the UI.
 * 
 * @author ShifatKhan
 */
public class DialogueSystem : Interactable
{
    [SerializeField] private DialogueUIController dialogueUIController;
    public D_Dialogue startDialogue;        // The Dialogue data to display
    
    void Start()
    {
        // TODO: Get DialogueUIController
    }

    public override void OnInteract()
    {
        if (!interacted)
        {
            dialogueUIController.InitializeConversation(startDialogue);
        }

        base.OnInteract();
    }
}
