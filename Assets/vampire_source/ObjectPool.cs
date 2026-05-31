using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject PoolObject;
    public int PoolSize = 20;

    private Queue<GameObject> Pool = new Queue<GameObject>();

    private void Start()
    {
        for (int i = 0; i < PoolSize; i++)
        {
            CreateNewObjectInPool();
        }
    }
    
    private GameObject CreateNewObjectInPool()
    {
        GameObject obj = Instantiate(PoolObject, this.transform);
        obj.SetActive(false);
        
        if (obj.TryGetComponent<PoolItem>(out var poolItem))
        {
            poolItem.SetPool(this);
        }
        
        Pool.Enqueue(obj);
        return obj;
    }

    public GameObject RequestObject(float customLifeTime = 0)
    {
        GameObject obj;

        if (Pool.Count > 0)
        {
            obj = Pool.Dequeue();
        }
        else
        {
            obj = CreateNewObjectInPool();
            Pool.Dequeue(); 
        }
        
        if (obj.TryGetComponent<PoolItem>(out var poolItem))
        {
            poolItem.ResetState(customLifeTime);
        }

        obj.SetActive(true);
        return obj;
    }
    
    public void ReleaseObject(GameObject obj)
    {
        if (Pool.Contains(obj)) return; 

        obj.SetActive(false);
        Pool.Enqueue(obj);
    }
}