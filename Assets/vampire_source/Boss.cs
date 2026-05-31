using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Boss : Enemy
{
    public Slider healthBar;

    public GameObject loot;

    public int NumberOfPoints;
    public float ArcRadius;
    public float AttackCd;
    public float ShootCharge;
    public bool IsFinalBoss;

    public Transform Sprite;
    
    private float charging;
    private TimeUntil cdEnds = 0;

    protected override void ManagedFixedUpdate(float dt)
    {
        base.ManagedFixedUpdate(dt);

        if (cdEnds)
        {
            cdEnds = AttackCd;
            Shoot();
        }
        
        Sprite.right = GameMath.Direction(Sprite.position, G.vamp.GetPlayerPosition());
        
        if (healthBar != null)
            healthBar.value = GameMath.Inverse01(Damage / (float)MaxHp);
    }

    private void Shoot()
    {
        StartCoroutine(ShootRoutine());
    }
    
    private IEnumerator ShootRoutine()
    {
        _isStopped = true;

        charging = ShootCharge;
        while (charging > 0)
        {
            charging -= G.dt;
            yield return new WaitForFixedUpdate();
        }
        
        var arc = GameMath.Arc(transform.position, ArcRadius, GameMath.Direction(transform.position, G.vamp.GetPlayerPosition()), NumberOfPoints);
        foreach (var b in arc)
        {
            var instantiate = Instantiate(G.vamp.Projectile, transform.position, Quaternion.identity);
            var projectile = instantiate.GetComponent<Projectile>();
            projectile.Init();
            projectile.Direction = b - (Vector2)transform.position;
        }
        
        _isStopped = false;
    }

    protected override void OnKill()
    {
        if (IsFinalBoss)
        {
            G.IsPaused = true;
            SceneManager.LoadScene("Victory");
        }
    }
}