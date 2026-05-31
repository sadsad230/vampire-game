using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : ManagedBehaviour
{
    public float smoothTime = 0.1f;

    private Transform target;
    private float projectileAngle;
    private float rotVelocity;

    private float speed;

    private int _remainRicochets;

    private HashSet<Transform> _alreadyDamaged = new HashSet<Transform>();

    private TimeSince timeSinceInit;

    public void Initialize(Transform targetTarget, float bulletSpeed, int ricochets)
    {
        target = targetTarget;
        speed = bulletSpeed;
        _remainRicochets = ricochets;
        
        _alreadyDamaged.Clear();

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

        transform.position += (Vector3)GameMath.AngleToVector2(projectileAngle) * (speed * dt);

        foreach (var enemy in G.vamp.Enemies)
        {
            if (!_alreadyDamaged.Contains(enemy.transform) && GameMath.IsColliding(enemy.transform.position, transform.position, 1f))
            {
                G.vamp.DealDamage(enemy, G.vamp.GetPlayerDamage(), enemy.transform.position);
                _alreadyDamaged.Add(enemy.transform);

                if (_remainRicochets > 0)
                {
                    _remainRicochets--;

                    Transform closest = FindTargetForRicochet();

                    if (closest != null)
                    {
                        target = closest;
                        rotVelocity = 0;
                        timeSinceInit = 0;
                        break;
                    }
                }
                
                Destroy(gameObject);
                break;
            }
        }


        if (timeSinceInit >= G.vamp.BulletLifeTime)
        {
            Destroy(gameObject);
        }
    }

    private Transform FindTargetForRicochet()
    {
        var eos = G.vamp.GetEnemiesOnScreen();
        if (eos == null || eos.Count == 0) return null;

        var validTargets = new List<Transform>();

        foreach (var e in eos)
        {
            if (e != null && !_alreadyDamaged.Contains(e.transform))
            {
                validTargets.Add(e.transform);
            }
        }

        var closest = GameMath.FindClosest(validTargets, transform.position);
        return closest.Item1;
    }
}