using System.Collections.Generic;
using UnityEngine;

public class OrbitalProjectile : ManagedBehaviour
{
    public float Range;
    
    protected override void ManagedFixedUpdate(float dt)
    {
        base.ManagedFixedUpdate(dt);
        
        foreach (var enemy in G.vamp.Enemies)
        {
            if (GameMath.IsColliding(enemy.transform.position, transform.position, Range))
            {
                G.vamp.DealDamage(enemy, G.vamp.GetPlayerDamage(), enemy.transform.position);
            }
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, Range);
    }
}