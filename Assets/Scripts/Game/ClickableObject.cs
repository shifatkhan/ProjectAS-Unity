using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/** Makes any GameObject clickable. 
 * TODO: Need to accomodate controller and keyboard clicks.
 * 
 * @author ShifatKhan
 * @Special thanks to Kiwasi (https://forum.unity.com/threads/ui-button-detecting-right-mouse-button.336111/?_ga=2.33559265.1150098498.1594240262-690570148.1578765376)
 */
public class ClickableObject : MonoBehaviour, IPointerClickHandler
{
    public UnityEvent leftClick;
    public UnityEvent middleClick;
    public UnityEvent rightClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            leftClick.Invoke();
        }
        else if (eventData.button == PointerEventData.InputButton.Middle)
        {
            middleClick.Invoke();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            rightClick.Invoke();
        }
    }
}
