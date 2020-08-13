using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUIController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject dialogueBox;        // Box background sprite.
    private TextMeshProUGUI textMesh;                       // The text
    private TextMeshAnimator textMeshAnimator;              // The text animator

    [SerializeField] private GameObject responsesGameObject;// Parent holder of all response buttons.
    private bool responsePressed = false;

    [SerializeField] private Image leftPortraitImg;
    [SerializeField] private GameObject leftNameGameObject;

    [SerializeField] private Image rightPortraitImg;
    [SerializeField] private GameObject rightNameGameObject;

    [Header("Dialogue modifiers")]
    [SerializeField] private float typingSpeed = 40f; // Letters per second.
    private string textToType = ""; // Current text being typed (or to type)
    private bool finishedTyping = true;
    
    private Color showTextColor;        // Color for showing the Text
    private Color showDialogueColor;    // Color for showing the dialogue box
    private Color hideColor;            // Color to hide UI
    
    void Start()
    {
        // UI COMPONENTS
        dialogueBox = GameObject.FindGameObjectWithTag("Dialogue");
        textMesh = dialogueBox.GetComponentInChildren<TextMeshProUGUI>();
        textMeshAnimator = dialogueBox.GetComponentInChildren<TextMeshAnimator>();

        responsesGameObject = GameObject.Find("Responses");

        // Set portrait and name
        //portraitImg.sprite = startDialogue.speaker.portrait;
        //nameGameObject.GetComponentInChildren<TextMeshProUGUI>().text = startDialogue.speaker.fullName;

        // COLORS
        showDialogueColor = new Color(1, 1, 1, 1);
        showTextColor = textMesh.color;

        hideColor = textMesh.color;
        hideColor.a = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
