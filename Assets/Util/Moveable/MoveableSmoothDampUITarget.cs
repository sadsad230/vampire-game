using UnityEngine;

public class MoveableSmoothDampUITarget : MoveableSmoothDamp
{
    public Transform target;
    public RectTransform uiTarget;
    public Vector3 uiOffset;

    protected override void ManagedUpdate()
    {
        if (target != null)
            targetPosition = target.position + uiOffset;
        else
            targetPosition = GameMath.CanvasToWorld(uiTarget) + uiOffset;

        base.ManagedUpdate();
    }
}