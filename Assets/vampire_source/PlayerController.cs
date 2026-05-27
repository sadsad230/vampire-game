using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : ManagedBehaviour
{
    public float TurnSpeed;
    public Vector2 Direction;

    private TimeUntil delayEnds;

    protected override void ManagedFixedUpdate(float dt)
    {
        Walk(dt);
        UpdateAttack();
    }

    private void Walk(float dt)
    {
        Direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Direction = Vector2.ClampMagnitude(Direction, 1f);
        transform.position += (Vector3)(Direction * G.vamp.GetPlayerSpeed() * dt);
        
        if (Direction != Vector2.zero)
        {
            float rotationAngle = Mathf.Atan2(Direction.y, Direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, rotationAngle);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, dt * TurnSpeed);
        }
        
        transform.position = Vector3.ClampMagnitude(transform.position, 70f);
    }
    
    #region AutoAttack

    private void UpdateAttack()
    {
        var target = FindTarget();
        if (target == null)
            return;

        if (delayEnds)
        {
            var bulletGO = Instantiate(G.vamp.PlayerBullet, G.vamp.GetPlayerPosition(), Quaternion.identity);

            var bulletScript = bulletGO.GetComponent<PlayerBullet>();
            if (bulletScript != null)
            {
                bulletScript.Initialize(target, G.vamp.BulletSpeed);
            }
            
            delayEnds = G.vamp.BulletSpawnDelay;
        }
    }
    
    private Transform FindTarget()
    {
        var eos = G.vamp.GetEnemiesOnScreen();
        if (eos == null || eos.Count == 0) return null;

        var closest = GameMath.FindClosest(eos, G.vamp.GetPlayerPosition());
        return closest.Item1;
    }
    
    #endregion
}