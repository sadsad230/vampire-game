using System;
using UnityEngine;

public class Enemy : ManagedBehaviour
{
    public int MaxHp = 10;
    public int Damage = 0;
    public float Range = 1f;

    public TimeUntil EndsDamageCd;
    
    public float Speed;
    
    protected bool _isStopped = false;

    public override void Init()
    {
        G.vamp.Enemies.Add(this);
    }

    private void OnDestroy()
    {
        G.vamp.Enemies.Remove(this);
        OnKill();
    }

    protected virtual void OnKill()
    {
        
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

        if (GameMath.IsColliding(transform.position, target, Range))
        {
            if (!G.vamp.IsPerformingDash)
            {
                G.vamp.DealDamage(this, G.vamp.GetDamageByContact(), transform.position);
                G.vamp.DamagePlayer();
            }
            else if (G.vamp.upgrades.HasUpgrade("Dashcut"))
            {
                G.vamp.DealDamage(this, G.vamp.GetDamageByContact(), transform.position);
            }
        }
        
        if (!_isStopped)
            transform.position = Vector3.MoveTowards(transform.position, target, Speed * dt);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, Range);
    }
}