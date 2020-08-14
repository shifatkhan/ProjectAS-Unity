using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogueUIController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject dialogueBox;        // Box background sprite.
    private TextMeshProUGUI textMesh;                       // The text
    private TextMeshAnimator textMeshAnimator;              // The text animator

    // RESPONSES
    [SerializeField] private GameObject responsesGameObject;// Parent holder of all response buttons.
    private bool responsePressed = false;

    // SPEAKERS PORTRAIT
    [SerializeField] private Image leftPortraitImg;
    [SerializeField] private GameObject leftNameGameObject;

    [SerializeField] private Image rightPortraitImg;
    [SerializeField] private GameObject rightNameGameObject;

    [Header("Dialogue modifiers")]
    private string textToType = "";                     // Current text being typed (or to type)
    private bool finishedTyping = true;
    
    private Color showTextColor;        // Color for showing the Text
    private Color showDialogueColor;    // Color for showing the dialogue box
    private Color hideColor;            // Color to hide UI

    // DIALOGUE
    private D_Dialogue startDialogue;        // The Dialogue data to display
    private D_Dialogue responseDialogue;    // Resulting Dialogue from a response
    private D_Dialogue currentDialogue;
    private int currentDialogueIndex = 0;       // The Dialogue index
    private bool initializedConversation = false; // Used in the Update().
    private bool resetConversation = false;

    private void Start()
    {
        // Get Components to update
        textMesh = dialogueBox.GetComponentInChildren<TextMeshProUGUI>();
        textMeshAnimator = dialogueBox.GetComponentInChildren<TextMeshAnimator>();

        // COLORS
        showDialogueColor = new Color(1, 1, 1, 1);
        showTextColor = textMesh.color;

        hideColor = textMesh.color;
        hideColor.a = 0;
    }

    private void Update()
    {
        // We use "initializedConversation" because we don't want to run GetNextDialogue() the first time the player
        // pressed Interact (since it is ran when they press on the NPC).
        if (Input.GetButtonDown("Interact") && initializedConversation)
        {
            GetNextDialogue();
        }
    }

    public void InitializeConversation(D_Dialogue dialogue)
    {
        startDialogue = dialogue;
        currentDialogue = startDialogue;

        initializedConversation = true;
        resetConversation = false;
    }

    /** This is where the bulk of the dialogue system is.
     * It will get the appropriate dialogue to display and run the typewriter effect.
     */
    private void GetNextDialogue()
    {
        if (finishedTyping)
        {
            currentDialogue = GetCurrentDialogue();

            // Check if there are any other dialogue to display.
            // If not, we close the dialogue.
            if (currentDialogueIndex < currentDialogue.dialogue.Count)
            {
                finishedTyping = false;

                textMeshAnimator.text = currentDialogue.dialogue[currentDialogueIndex];
                textToType = textMesh.text;
                textMesh.text = "";

                ShowDialogue();

                StopAllCoroutines(); // Stop previous typewriter.
                StartCoroutine(Typewriter());
            }
            else
            {
                ExitConversation();
            }
        }
    }

    private void ExitConversation()
    {
        initializedConversation = false;
        resetConversation = true;
        currentDialogueIndex = 0;
        responsePressed = false;

        HideDialogue();
    }

    /** Create a typewriter effect by displaying letters one by one.
     */
    private IEnumerator Typewriter()
    {
        for (int i = 0; i < textToType.Length; i++)
        {
            // Check for rich text. Skip them so typewriter effect doesn't apply.
            if (textToType[i] == '<')
            {
                string richtext = "";
                for (int j = i; j < textToType.Length; j++)
                {
                    richtext += textToType[j];
                    if (textToType[j] == '>')
                    {
                        // Hotfix for index out of bounds when we do i = j+1
                        if (j + 1 >= textToType.Length)
                            textToType += " ";

                        i = j + 1;
                        textMesh.text += richtext;
                        break;
                    }
                }
            }

            textMesh.text += textToType[i];
            textMeshAnimator.SyncToTextMesh();

            yield return new WaitForSeconds(1 / currentDialogue.typingSpeed);
        }

        finishedTyping = true;
        currentDialogueIndex++;

        if (currentDialogueIndex - 1 == currentDialogue.dialogue.Count - 1)
        {
            if (currentDialogue.nextDialogue != null)
            {
                currentDialogue = currentDialogue.nextDialogue;
                currentDialogueIndex = 0;
            }
            else
            {
                // Show responses at the end of dialogues & when typewriter finished.
                ShowResponses();
            }
        }
    }

    /** Displays everything related to the dialogue.
     * This includes the portrait, the name, the dialogue box, and the text.
     */
    private void ShowDialogue()
    {
        if (currentDialogue.isLeftSpeaker)
        {
            ShowLeftSpeaker();
            HideRightSpeaker();
        }
        else
        {
            ShowRightSpeaker();
            HideLeftSpeaker();
        }

        ShowDialogueBox();
    }

    /** Makes the whole dialogue system hidden.
     */
    private void HideDialogue()
    {
        HideDialogueBox();
        HideLeftSpeaker();
        HideRightSpeaker();
    }

    /** Make the dialogue box & text visible.
     */
    private void ShowDialogueBox()
    {
        dialogueBox.GetComponent<Image>().color = showDialogueColor;
        textMesh.color = showTextColor;
    }

    /** Make the dialogue box & text hidden.
     * Also, reset dialogue index.
     */
    private void HideDialogueBox()
    {
        dialogueBox.GetComponent<Image>().color = hideColor;
        textMesh.color = hideColor;
        
        HideResponses();
    }

    /** Display the left portrait and name UI.
     */
    private void ShowLeftSpeaker()
    {
        leftPortraitImg.sprite = currentDialogue.speaker.portrait;
        leftNameGameObject.GetComponentInChildren<TextMeshProUGUI>().text = currentDialogue.speaker.fullName;

        leftPortraitImg.color = showDialogueColor;
        leftNameGameObject.GetComponent<Image>().color = showDialogueColor;
        leftNameGameObject.GetComponentInChildren<TextMeshProUGUI>().color = showTextColor;
    }

    /** Hide the left portrait and name UI.
     */
    private void HideLeftSpeaker()
    {
        leftPortraitImg.color = hideColor;
        leftNameGameObject.GetComponent<Image>().color = hideColor;
        leftNameGameObject.GetComponentInChildren<TextMeshProUGUI>().color = hideColor;
    }

    /** Display the right portrait and name UI.
     */
    private void ShowRightSpeaker()
    {
        rightPortraitImg.sprite = currentDialogue.speaker.portrait;
        rightNameGameObject.GetComponentInChildren<TextMeshProUGUI>().text = currentDialogue.speaker.fullName;

        rightPortraitImg.color = showDialogueColor;
        rightNameGameObject.GetComponent<Image>().color = showDialogueColor;
        rightNameGameObject.GetComponentInChildren<TextMeshProUGUI>().color = showTextColor;
    }

    /** Hide the left portrait and name UI.
     */
    private void HideRightSpeaker()
    {
        rightPortraitImg.color = hideColor;
        rightNameGameObject.GetComponent<Image>().color = hideColor;
        rightNameGameObject.GetComponentInChildren<TextMeshProUGUI>().color = hideColor;
    }

    /** Enable as many buttons as there are responses.
     */
    private void ShowResponses()
    {
        currentDialogue = GetCurrentDialogue();

        if (currentDialogue.responseOptions.Count > 0)
        {
            int children = responsesGameObject.transform.childCount;

            GameObject currentButton;

            for (int i = 0; i < children && i < currentDialogue.responseOptions.Count; i++)
            {
                currentButton = responsesGameObject.transform.GetChild(i).gameObject;
                currentButton.SetActive(true);
                currentButton.GetComponent<Button>().onClick.AddListener(OnResponseClick);
                currentButton.GetComponentInChildren<TextMeshProUGUI>().text = currentDialogue.responseOptions[i].responseText;
            }
        }
    }

    /** Disable all response buttons.
     */
    private void HideResponses()
    {
        int children = responsesGameObject.transform.childCount;

        GameObject currentButton;

        for (int i = 0; i < children; i++)
        {
            currentButton = responsesGameObject.transform.GetChild(i).gameObject;
            currentButton.GetComponent<Button>().onClick.RemoveAllListeners();
            currentButton.SetActive(false);
        }
    }

    /** Get the name of the button's text and use it to find the ResponseObject.
     * The button's text was set by the ResponseObject's responseText in the ShowResponses() method.
     * Therefore, allowing us to use it to find which responseObject to use.
     */
    public void OnResponseClick()
    {
        currentDialogue = GetCurrentDialogue();

        string buttonPressed = EventSystem.current.currentSelectedGameObject.GetComponentInChildren<TextMeshProUGUI>().text;
        D_Response selectedResponse = null;

        // Find response object with same text value.
        foreach (D_Response response in currentDialogue.responseOptions)
        {
            if (response.responseText == buttonPressed)
            {
                selectedResponse = response;
                break;
            }
        }

        // Update dialogue to show response.
        responseDialogue = selectedResponse.dialogueObject;
        currentDialogueIndex = 0;
        responsePressed = true;

        HideResponses();
        GetNextDialogue();
    }

    /** Check whether the current dialogue is a response dialogue or not.
     * Response Dialogue means a dialogue to show after the player chose
     * a response option.
     */
    private D_Dialogue GetCurrentDialogue()
    {
        if (resetConversation)
        {
            return startDialogue;
        }
        else if (responsePressed)
        {
            responsePressed = false;
            return responseDialogue;
        }

        return currentDialogue;
    }
}
