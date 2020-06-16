using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueSystem : Interactable
{
    public GameObject dialogueBox;              // This is the dialogue box that has the box sprite
    public TextMeshProUGUI textMesh;            // The text
    public TextMeshAnimator textMeshAnimator;   // The text animator

    public DialogueObject dialogueObject;       // The Dialogue data to display
    private int currentDialogueIndex = 0;       // The Dialogue index

    private Color showColor = new Color(1, 1, 1, 1);
    private Color hideColor = new Color(1, 1, 1, 0);

    void Start()
    {
        dialogueBox = GameObject.FindGameObjectWithTag("Dialogue");
        textMesh = dialogueBox.GetComponentInChildren<TextMeshProUGUI>();
        textMeshAnimator = dialogueBox.GetComponentInChildren<TextMeshAnimator>();
    }
    
    public override void OnInteract()
    {
        // Check if there are any other dialogue to display.
        // If not, we close the dialogue.
        if(currentDialogueIndex < dialogueObject.dialogue.Length)
        {
            textMeshAnimator.text = dialogueObject.dialogue[currentDialogueIndex];
            currentDialogueIndex++;
            ShowDialogueBox();
        }
        else
        {
            HideDialogueBox();
        }
    }

    /** Make the dialogue box & text visible.
     */
    private void ShowDialogueBox()
    {
        dialogueBox.GetComponent<Image>().color = showColor;
        textMesh.color = showColor;
    }

    /** Make the dialogue box & text hidden.
     * Also, reset dialogue index.
     */
    private void HideDialogueBox()
    {
        dialogueBox.GetComponent<Image>().color = hideColor;
        textMesh.color = hideColor;
        currentDialogueIndex = 0;
    }
}
