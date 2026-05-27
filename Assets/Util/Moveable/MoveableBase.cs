using UnityEngine;

public class MoveableBase : ManagedBehaviour
{
    public Vector2 targetPosition;

    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(targetPosition, 0.2f);
    }
}