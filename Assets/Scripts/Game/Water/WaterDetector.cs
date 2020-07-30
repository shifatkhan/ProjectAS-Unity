using UnityEngine;
using System.Collections;

public class WaterDetector : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D Hit)
    {
        // TODO: Change this to accomodate enemies, NPCs and player. (Entity?)
        if (Hit.GetComponent<Rigidbody2D>() != null && Hit.CompareTag("Player"))
        {
            transform.parent.GetComponent<WaterSimulation>().Splash(transform.position.x, Hit.GetComponent<Player>().GetVelocity().y / 40f);
        }
    }

}
