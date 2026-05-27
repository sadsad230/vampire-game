using System;
using UnityEngine;

public class Enemy : ManagedBehaviour
{
    public int MaxHp = 10;
    public int Damage = 0;

    public float Speed;
    
    protected override void Init()
    {
        G.vamp.Enemies.Add(this);
    }

    private void OnDestroy()
    {
        G.vamp.Enemies.Remove(this);
    }

    public int Hp()
    {
        var hp = MaxHp - Damage;
        return hp;
    }

    protected override void ManagedFixedUpdate(float dt)
    {
        base.ManagedFixedUpdate(dt);
        var target = G.vamp.GetPlayerPosition();

        if (GameMath.IsColliding(transform.position, target, 1f))
        {
            G.vamp.DealDamage(this, G.vamp.GetDamageByContact());
            G.vamp.DamagePlayer();
        }
        
        transform.position = Vector3.MoveTowards(transform.position, target, Speed * dt);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}