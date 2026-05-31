using UnityEngine;

public enum ProjectileOwner
{
    Player,
    Enemy
}

public class Projectile : ManagedBehaviour
{
    public ProjectileOwner Owner = ProjectileOwner.Enemy;
    public Vector3 Direction;
    public float Speed;
    public float Range = 1f;
    public float LifeTime = 3f;

    private TimeUntil die;

    public void Init()
    {
        die = LifeTime;
    }

    protected override void ManagedFixedUpdate(float dt)
    {
        base.ManagedFixedUpdate(dt);

        
        transform.position += Direction.normalized * Speed * dt;
        
        if (Owner == ProjectileOwner.Enemy)
        {
            if (!G.vamp.IsPerformingDash)
            {
                if (G.vamp.IsCollidingWithPlayer(transform.position, Range))
                {
                    G.vamp.DamagePlayer();
                    Destroy(gameObject);
                }
            }
        }

        if (Owner == ProjectileOwner.Player)
        {
            foreach (var enemy in G.vamp.Enemies)
            {
                if (GameMath.IsColliding(enemy.transform.position, transform.position, Range))
                {
                    G.vamp.DealDamage(enemy, G.vamp.GetPlayerDamage(), enemy.transform.position);
                    Destroy(gameObject);
                }
            }
        }

        if (die)
        {
            Destroy(gameObject);
        }
    }
}
