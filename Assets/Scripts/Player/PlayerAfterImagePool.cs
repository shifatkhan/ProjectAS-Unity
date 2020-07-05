using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterImagePool : MonoBehaviour
{
    [SerializeField]
    private GameObject afterImagePrefab;

    private Queue<GameObject> availableObjects = new Queue<GameObject>();

    // Have only instance of this pool.
    public static PlayerAfterImagePool Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        GrowPool();
    }

    private void GrowPool()
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject instanceToAdd = Instantiate(afterImagePrefab);
            instanceToAdd.transform.SetParent(transform);
            AddToPool(instanceToAdd);
        }
    }

    /** Add game object to pool.
     */
    public void AddToPool(GameObject instance)
    {
        instance.SetActive(false);
        availableObjects.Enqueue(instance);
    }

    /** Get front of pool queue.
     */
    public GameObject GetFromPool(bool flipped = false)
    {
        if(availableObjects.Count == 0)
        {
            GrowPool();
        }

        GameObject instance = availableObjects.Dequeue();
        instance.SetActive(true);

        Vector3 newTransform = instance.transform.localScale;
        newTransform.x *= flipped ? -1 : 1;
        instance.transform.localScale = newTransform;

        return instance;
    }
}
