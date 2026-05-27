using System;
using UnityEngine;

public class MoveableBalatro : MoveableBase
{
    private Vector2 velocity; 
    private float maxVelocity; 

    private void Update()
    {
        float realDt = Mathf.Clamp(Time.smoothDeltaTime, 1 / 50f, 1 / 100f);
        
        float expTimeXY = Mathf.Exp(-50 * realDt);
        maxVelocity = 70 * realDt;

        MoveXY(realDt, expTimeXY);
    }

    private void MoveXY(float dt, float expTimeXY)
    {
        Vector2 T = targetPosition; 
        Vector2 currentPos = new Vector2(transform.position.x, transform.position.y); 
        
        velocity = expTimeXY * velocity + (1 - expTimeXY) * (T - currentPos) * 35 * dt;
        
        if (velocity.sqrMagnitude > maxVelocity * maxVelocity)
        {
            velocity = velocity.normalized * maxVelocity;
        }
        
        transform.position += new Vector3(velocity.x, velocity.y, 0) * dt * 100f;
    }
}