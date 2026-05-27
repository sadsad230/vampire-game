using UnityEngine;

public abstract class ManagedBehaviour : MonoBehaviour
{
    void Start()
    {
        Init();
    }

    protected virtual void Init()
    {
    }

    void Update()
    {
        if (!G.IsPaused)
        {
            ManagedUpdate();
        }
    }

    protected virtual void ManagedUpdate()
    {
        if (!G.IsPaused)
        {
            ManagedFixedUpdate(G.dt);
        }
    }

    protected virtual void ManagedFixedUpdate(float dt)
    {
    }
}