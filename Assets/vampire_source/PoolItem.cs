using UnityEngine;

public class PoolItem : ManagedBehaviour
{
    public float DefaultLifeTime = 1f; 
    private float currentLifeTime;
    private ObjectPool originPool;
    
    public void SetPool(ObjectPool pool)
    {
        originPool = pool;
    }
    
    public void ResetState(float customLifeTime = 0)
    {
        currentLifeTime = customLifeTime > 0 ? customLifeTime : DefaultLifeTime;
    }

    protected override void ManagedFixedUpdate(float dt)
    {
        if (currentLifeTime > 0)
        {
            currentLifeTime -= dt;
            if (currentLifeTime <= 0)
            {
                ReturnToPool();
            }
        }
    }

    public void ReturnToPool()
    {
        if (originPool != null)
        {
            originPool.ReleaseObject(this.gameObject);
        }
        else
        {
            gameObject.SetActive(false); 
        }
    }
}