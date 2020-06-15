using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueSystem : MonoBehaviour
{
    public GameObject dialogueBox;
    public TextMeshProUGUI textMesh;
    public TextMeshAnimator textMeshAnimator;

    public DialogueObject dialogueObject;
    private int currentDialogueIndex = 0;

    public bool playerInRange = false;

    private Color showColor = new Color(1, 1, 1, 1);
    private Color hideColor = new Color(1, 1, 1, 0);

    void Start()
    {
        dialogueBox = GameObject.FindGameObjectWithTag("Dialogue");
        textMesh = dialogueBox.GetComponentInChildren<TextMeshProUGUI>();
        textMeshAnimator = dialogueBox.GetComponentInChildren<TextMeshAnimator>();
    }
    
    void Update()
    {
        if (playerInRange && Input.GetButtonDown("Interact"))
        {
            ShowDialogueBox();
            StartNextDialogue();
        }
    }
    
    void StartNextDialogue()
    {
        if(currentDialogueIndex < dialogueObject.dialogue.Length)
        {
            textMeshAnimator.text = dialogueObject.dialogue[currentDialogueIndex];
            currentDialogueIndex++;
        }
        else
        {
            HideDialogueBox();
        }
    }

    private void ShowDialogueBox()
    {
        dialogueBox.GetComponent<Image>().color = showColor;
        textMesh.color = showColor;
    }

    private void HideDialogueBox()
    {
        dialogueBox.GetComponent<Image>().color = hideColor;
        textMesh.color = hideColor;
        currentDialogueIndex = 0;
    }

    // TODO: Create new Interactable object with Ontriggerenter & exit
    // Move Update() function into Interactable.
    // Move StartNextDialogue method into Interactable and rename it to OnInteract()
    // OnInteract will be a virtual method.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInRange = false;
            HideDialogueBox();
        }
    }
}
