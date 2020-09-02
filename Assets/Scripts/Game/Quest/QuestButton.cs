using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestButton : MonoBehaviour
{
    public Button button;
    public RawImage icon;
    public Text eventName;
    public Sprite currentImage;
    public Sprite waitingImage;
    public Sprite doneImage;
    public QuestEvent thisEvent;

    // TODO: Add CompassController script. Then update the compass to next quest event when current is done.
    // https://youtu.be/CTCpkJ8PmY8

    EventStatus status;

    public void Setup(QuestEvent e, GameObject scrollList)
    {
        thisEvent = e;
        button.transform.SetParent(scrollList.transform, false);
        eventName.text = "<b>" + thisEvent.name + "</b>\n" + thisEvent.description;
        status = thisEvent.status;
        icon.texture = waitingImage.texture;
        button.interactable = false;
    }

    public void UpdateButton(EventStatus s)
    {
        status = s;

        if (status == EventStatus.DONE)
        {
            icon.texture = doneImage.texture;
            button.interactable = false;
        }
        else if (status == EventStatus.WAITING)
        {
            icon.texture = waitingImage.texture;
            button.interactable = false;
        }
        else if (status == EventStatus.CURRENT)
        {
            icon.texture = currentImage.texture;
            button.interactable = true;
            ClickHandler();
        }
    }

    public void ClickHandler()
    {
        //TODO: Set CompassController to point towards the location of this event.
    }

}
