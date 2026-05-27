using UnityEngine;

public enum ProjectileOwner
{
    Player,
    Enemy
}

public class Projectile : ManagedBehaviour
{
    public ProjectileOwner Owner;
    public Vector3 Direction;
    public float Speed;
    public float Range = 1f;

    protected override void ManagedFixedUpdate(float dt)
    {
        base.ManagedFixedUpdate(dt);

        transform.position += Direction * Speed * dt;
        
        if (Owner == ProjectileOwner.Enemy)
        {
            if (G.vamp.IsCollidingWithPlayer(transform.position, Range))
            {
                G.vamp.DamagePlayer();
                Destroy(gameObject);
            }
        }

        if (Owner == ProjectileOwner.Player)
        {
            foreach (var enemy in G.vamp.Enemies)
            {
                if (GameMath.IsColliding(enemy.transform.position, transform.position, Range))
                {
                    G.vamp.DealDamage(enemy, G.vamp.GetPlayerDamage());
                    Destroy(gameObject);
                }
            }
        }
    }
}
