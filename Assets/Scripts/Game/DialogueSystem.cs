using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TextMeshAnimator))]
public class DialogueSystem : MonoBehaviour
{
    private TextMeshAnimator textMeshAnimator;

    [TextArea]
    public string[] dialogue;

    // TODO: Testing
    private int temp = 0;
    
    void Start()
    {
        textMeshAnimator = GetComponent<TextMeshAnimator>();
        textMeshAnimator.text = dialogue[0];
    }
    
    void Update()
    {
        //Are you shaking in your<color=red> <shake>Boots </color>!?
        if (Input.GetButtonDown("Attack1"))
        {
            ChangeText();
        }
    }

    // TODO: Testing
    void ChangeText()
    {
        
        if(temp+1 < dialogue.Length)
        {
            temp++;
            textMeshAnimator.text = dialogue[temp];
        }
    }
}
