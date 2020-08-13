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
    private GameObject dialogueBox;              // This is the dialogue box that has the box sprite
    private TextMeshProUGUI textMesh;            // The text
    private TextMeshAnimator textMeshAnimator;   // The text animator

    private GameObject responsesGameObject;
    private bool responsePressed = false;

    private Image portraitImg;
    private GameObject nameGameObject;

    public D_Dialogue startDialogue;        // The Dialogue data to display
    private D_Dialogue responseDialogue;    // Resulting Dialogue from a response
    private D_Dialogue currentDialogue;
    private int currentDialogueIndex = 0;       // The Dialogue index

    private Color showDialogueColor;
    private Color showTextColor;
    private Color hideColor;

    private string textToType = "";
    public float typingSpeed = 40f;
    private bool finishedTyping = true;

    // TODO: Might need to change how we get hold of the dialogue UI.
    // Currently, it is very coupled with how the UI is setup.
    // Example: The `Dialogue Text` GameObject must be placed above the `Name Box` GameObject.
    void Start()
    {
        // Get Components to update
        dialogueBox = GameObject.FindGameObjectWithTag("Dialogue");
        textMesh = dialogueBox.GetComponentInChildren<TextMeshProUGUI>();
        textMeshAnimator = dialogueBox.GetComponentInChildren<TextMeshAnimator>();
        
        responsesGameObject = GameObject.Find("Responses");

        portraitImg = GameObject.FindGameObjectWithTag("Portrait").GetComponent<Image>();
        nameGameObject = GameObject.FindGameObjectWithTag("Name");

        // Set portrait and name
        portraitImg.sprite = startDialogue.speaker.portrait;
        nameGameObject.GetComponentInChildren<TextMeshProUGUI>().text = startDialogue.speaker.fullName;

        showDialogueColor = new Color(1, 1, 1, 1);
        showTextColor = textMesh.color;

        hideColor = textMesh.color;
        hideColor.a = 0;
    }
    
    public override void OnInteract()
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
                //Debug.Log("TEXT LENGTH: "+ textToType.Length);
                ShowDialogueBox();

                StopAllCoroutines(); // Stop previous typewriter.
                StartCoroutine(Typewriter());
            }
            else
            {
                HideDialogueBox();
            }
        }
    }

    IEnumerator Typewriter()
    {
        for (int i = 0; i < textToType.Length; i++)
        {
            // Check for rich text. Skip them so typewriter effect doesn't apply.
            if(textToType[i] == '<')
            {
                string richtext = "";
                for (int j = i; j < textToType.Length; j++)
                {
                    richtext += textToType[j];
                    if(textToType[j] == '>')
                    {
                        // Hotfix for index out of bounds when we do i = j+1
                        if (j + 1 >= textToType.Length)
                            textToType += " ";

                        i = j+1;
                        textMesh.text += richtext;
                        break;
                    }
                }
            }
            
            textMesh.text += textToType[i];
            textMeshAnimator.SyncToTextMesh();

            yield return new WaitForSeconds(1/typingSpeed);
        }

        finishedTyping = true;

        if (currentDialogueIndex == currentDialogue.dialogue.Count - 1)
        {
            // Show responses at the end of dialogues & when typewriter finished.
            ShowResponses();
        }
        currentDialogueIndex++;
    }

    /** Make the dialogue box & text visible.
     */
    private void ShowDialogueBox()
    {
        dialogueBox.GetComponent<Image>().color = showDialogueColor;
        textMesh.color = showTextColor;

        portraitImg.color = showDialogueColor;
        nameGameObject.GetComponent<Image>().color = showDialogueColor;
        nameGameObject.GetComponentInChildren<TextMeshProUGUI>().color = showTextColor;
    }

    /** Make the dialogue box & text hidden.
     * Also, reset dialogue index.
     */
    private void HideDialogueBox()
    {
        dialogueBox.GetComponent<Image>().color = hideColor;
        textMesh.color = hideColor;

        portraitImg.color = hideColor;
        nameGameObject.GetComponent<Image>().color = hideColor;
        nameGameObject.GetComponentInChildren<TextMeshProUGUI>().color = hideColor;

        currentDialogueIndex = 0;
        responsePressed = false;
        HideResponses();
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

    // TODO: Don't run this if responses are already hidden.
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
        OnInteract();
    }

    /** Check whether the current dialogue is a response dialogue or not.
     * Response Dialogue means a dialogue to show after the player chose
     * a response option.
     */
    private D_Dialogue GetCurrentDialogue()
    {
        return responsePressed ? responseDialogue : startDialogue;
    }
}
