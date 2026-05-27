using UnityEngine;

public class PlayerBullet : ManagedBehaviour
{
    public float smoothTime = 0.1f; 
    
    private Transform target;
    private float projectileAngle;
    private float rotVelocity;
    
    private float speed;
    
    private TimeSince timeSinceInit;
    
    public void Initialize(Transform targetTarget, float bulletSpeed)
    {
        target = targetTarget;
        speed = bulletSpeed;
        
        if (target != null)
        {
            projectileAngle = GameMath.AngleToPos2D(transform.position, target.position);
            transform.right = GameMath.AngleToVector2(projectileAngle);
        }

        timeSinceInit = 0;
    }

    protected override void ManagedFixedUpdate(float dt)
    {
        if (target != null)
        {
            projectileAngle = Mathf.SmoothDampAngle(
                projectileAngle, 
                GameMath.AngleToPos2D(transform.position, target.position), 
                ref rotVelocity, 
                smoothTime, 
                1000f
            );
            transform.right = GameMath.AngleToVector2(projectileAngle);
        }
        transform.position += (Vector3)GameMath.AngleToVector2(projectileAngle) * (speed * Time.deltaTime);
        
        foreach (var enemy in G.vamp.Enemies)
        {
            if (GameMath.IsColliding(enemy.transform.position, transform.position, 1f))
            {
                G.vamp.DealDamage(enemy, G.vamp.GetPlayerDamage());
                Destroy(gameObject);
            }
        }
        
        if (timeSinceInit >= G.vamp.BulletLifeTime)
        {
            Destroy(gameObject);
        }
    }
}