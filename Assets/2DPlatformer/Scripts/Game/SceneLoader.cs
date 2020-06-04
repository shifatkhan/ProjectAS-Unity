using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/** Class that loads new scene asynchronously. This script can be attached to 
 * a box collider gameobject, so if the player collides with it, it will load
 * the new scene (must set triggerEnabled to TRUE).
 * If triggerEnabled = FALSE, then this script can be used for UI buttons.
 * @author ShifatKhan
 */
public class SceneLoader : MonoBehaviour
{
    [HideInInspector]
    public string sceneName;

    private float loadingProgress = 0;
    public bool triggerEnabled = true;

    public void LoadScene()
    {
        StartCoroutine(LoadSceneCo());
    }

    private IEnumerator LoadSceneCo()
    {
        // Load scene asynchronously.
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            // Make the progress go from 0 to 1, instead of 0 to 0.9
            // Thanks to Brackeys: https://youtu.be/YMj2qPq9CP8
            loadingProgress = Mathf.Clamp01(operation.progress / 0.9f);

            Debug.Log(loadingProgress);
            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggerEnabled && other.gameObject.CompareTag("Player"))
        {
            LoadScene();
        }
    }

    /** Debug: Draw box collider.
     */
    void OnDrawGizmos()
    {
        if (triggerEnabled && GetComponent<BoxCollider2D>() != null)
        {
            Gizmos.color = new Color(0, 1, 0, .5f);
            Gizmos.DrawCube(transform.position, GetComponent<BoxCollider2D>().size);
        }
        
    }
}
