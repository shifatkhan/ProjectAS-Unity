using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterImagePool : MonoBehaviour
{
    [SerializeField]
    protected GameObject afterImagePrefab;

    protected Queue<GameObject> availableObjects = new Queue<GameObject>();

    // Have only instance of this pool.
    //public static PlayerAfterImagePool Instance { get; private set; }

    public virtual void Awake()
    {
        //Instance = this;
        GrowPool();
    }

    public virtual void GrowPool()
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject instanceToAdd = Instantiate(afterImagePrefab);
            instanceToAdd.GetComponent<PlayerAfterImage>().afterImagePool = this;
            instanceToAdd.transform.SetParent(transform);
            AddToPool(instanceToAdd);
        }
    }

    /** Add game object to pool.
     */
    public virtual void AddToPool(GameObject instance)
    {
        //instance.GetComponent<PlayerAfterImage>().afterImagePool = this;
        instance.SetActive(false);
        availableObjects.Enqueue(instance);
    }

    /** Get front of pool queue.
     */
    public virtual GameObject GetFromPool(bool flipped = false)
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
