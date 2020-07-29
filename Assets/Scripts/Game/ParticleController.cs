using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Handles how a particle behaves. Most function should be called through
 * Animation events.
 * 
 * @author ShifatKhan
 */
public class ParticleController : MonoBehaviour
{
    /** Destroys particle effect once it is done playing.
     */
    private void FinishAnim()
    {
        Destroy(gameObject);
        //gameObject.SetActive(false);
    }
}
