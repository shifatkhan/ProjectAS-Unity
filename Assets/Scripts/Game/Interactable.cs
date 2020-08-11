using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    protected bool playerInRange = false;

    public bool interacted { get; protected set; }
    [SerializeField] protected GameEvent interactEvent;

    void Update()
    {
        if (playerInRange && Input.GetButtonDown("Interact"))
        {
            OnInteract();
        }
    }

    public virtual void OnInteract()
    {
        if(interactEvent)
            interactEvent.Raise();

        interacted = true;
    }

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
            interacted = false;
        }
    }
}
