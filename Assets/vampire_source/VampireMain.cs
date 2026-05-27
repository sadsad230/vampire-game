using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VampireMain : MonoBehaviour, IGameSystem
{
    public UpgradeList upgrades = new UpgradeList();
    
    public float Speed;
    public int Damage;
    public int DamageByContact;
    public float BulletSpawnDelay;
    public float BulletSpeed;
    public float BulletLifeTime;
    public int PlayerMaxHp;
    public int PlayerHits;
    public float XPCollectRange;

    public float ProjectileSpeed;
    
    public GameObject Player;
    public GameObject XPOrb;
    public GameObject Projectile;
    public GameObject PlayerBullet;
    public GameObject EnemyObject;
    
    public List<GameObject> XPOrbs = new List<GameObject>();

    public int XPCollected;
    public int XPCollectedMax;
    public int XPTotal = 0;
    public int XPTotalToSummon = 500;

    public float SpawnCooldown;
    public List<Enemy> Enemies = new List<Enemy>();
    public float CooldownHitPlayer;
    
    private TimeUntil cooldownHitPlayerEnds = 0;
    private TimeUntil toSpawnEnemy = 0;
    

    

    private void Awake()
    {
        G.Init();
        G.Add(this);
        G.Start();
        
        
    }
    
    #region UI

    public Slider summonSlider;
    
    private void UpdateUI()
    {
        summonSlider.value = XPTotal;
        summonSlider.maxValue = XPTotalToSummon;
    }

    #endregion
    
    #region UPGRADE_UI

    public CanvasGroup upgrade_ui;

    public List<RectTransform> upgrades_sine_fun = new List<RectTransform>();

    public Button upgrade1Btn;
    public Image upgrade1Image;
    public TextMeshProUGUI upgrade1Text;
    public TextMeshProUGUI upgrade1Desc;

    public Button upgrade2Btn;
    public Image upgrade2Image;
    public TextMeshProUGUI upgrade2Text;
    public TextMeshProUGUI upgrade2Desc;

    public Button upgrade3Btn;
    public Image upgrade3Image;
    public TextMeshProUGUI upgrade3Text;
    public TextMeshProUGUI upgrade3Desc;
    
    public enum UpgradeContext
    {
        XP,
    }

    #endregion
    
    #region GameLoop

    private void Update()
    {
        UpdateUI();
        
        if (G.IsPaused)
            return;

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            XpCollect(Instantiate(XPOrb, GetPlayerPosition(),  Quaternion.identity));
            XpCollect(Instantiate(XPOrb, GetPlayerPosition(),  Quaternion.identity));
        }

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            for (int i = 0; i < 100; i++)
            {
                XpCollect(Instantiate(XPOrb, GetPlayerPosition(),  Quaternion.identity));
            }
        }
    }

    private void FixedUpdate()
    {
        if (G.IsPaused)
            return;
        
        UpdateXP();
        SpawnEnemy();
        
        if (XPTotal >= XPTotalToSummon)
            SceneManager.LoadScene("Victory");
    }
    
    #endregion
    
    #region XP

    public void AddXP(Vector3 position)
    {
        var xp = Instantiate(XPOrb, position + (Vector3)Random.insideUnitCircle, Quaternion.identity);
        XPOrbs.Add(xp);
    }
    
    public void OnXPRemoved(GameObject XP)
    {
        XPCollected++;
        XPTotal++;

        if (XPCollected == XPCollectedMax)
        {
            XPCollected = 0;
            //DoUpgrade
        }
        
        Destroy(XP);
    }

    public void XpCollect(GameObject XP)
    {
        var moveableSmoothDamp = XP.AddComponent<MoveableSmoothDamp>();
        moveableSmoothDamp.targetPosition = GetPlayerPosition();
        moveableSmoothDamp.selfDestruct = true;
        moveableSmoothDamp.smoothTime = 0.15f;
        moveableSmoothDamp.reachRange = 1.5f;
        moveableSmoothDamp.maxVelocity = 50f;
        moveableSmoothDamp.onReach += () => OnXPRemoved(XP);
        
    }

    public void UpdateXP()
    {
        for (var index = XPOrbs.Count - 1; index >= 0; index--)
        {
            var orb = XPOrbs[index];

            if (GameMath.IsColliding(GetPlayerPosition(), orb.transform.position, XPCollectRange))
            {
                XpCollect(orb);
                XPOrbs.RemoveAt(index);
            }
        }
    }
    
    #endregion

    #region Spawner

    public void SpawnEnemy()
    {
        if (!toSpawnEnemy)
            return;

        var rand = Random.Range(10, 100);

        var pos = (Vector2)GetPlayerPosition() + Random.insideUnitCircle.normalized * rand;
        
        Instantiate(EnemyObject, pos, Quaternion.identity);

        toSpawnEnemy = SpawnCooldown;
    }

    #endregion

    #region Enemy
    
    private void GameOver()
    {
        G.IsPaused = true;
        //show game over screen
        StopAllCoroutines();
    }

    public void DealDamage(Enemy en, int damage)
    {
        en.Damage += damage;

        if (en.Hp() <= 0)
        {
            RemoveEnemy(en);
        }
    }

    public void RemoveEnemy(Enemy en)
    {
        var rnd = Random.Range(1, 3);

        for (var i = 1; i < 1 + rnd; i++)
        {
            AddXP(en.transform.position);
        }
        
        Destroy(en.gameObject);
    }

    #endregion
    
    List<Enemy> enemiesOnScreen = new List<Enemy>();
    public List<Enemy> GetEnemiesOnScreen()
    {
        enemiesOnScreen.Clear();
        foreach (var e in Enemies)
            if (GameMath.IsOnScreen(e.transform.position))
                enemiesOnScreen.Add(e);
        return enemiesOnScreen;
    }
    

    public void DamagePlayer()
    {
        if (!cooldownHitPlayerEnds)
            return;
        
        cooldownHitPlayerEnds = CooldownHitPlayer;

        PlayerHits++;

        if (PlayerHits >= GetPlayerMaxHp())
            GameOver();
    }

    private int GetPlayerMaxHp()
    {
        var maxHp = PlayerMaxHp;
        return maxHp;
    }

    public float GetProjectileSpeed()
    {
        var speed = ProjectileSpeed;
        return speed;
    }

    public float GetPlayerSpeed()
    {
        var speed = Speed;
        return speed;
    }

    public int GetPlayerDamage()
    {
        var damage = Damage;
        return damage;
    }

    public int GetDamageByContact()
    {
        var damage = DamageByContact;
        return damage;
    }

    public Vector3 GetPlayerPosition()
    {
        return Player.transform.position;
    }
    
    public bool IsCollidingWithPlayer(Vector3 position, float range)
    {
        return GameMath.IsColliding(Player.transform.position, position, range);
    }

    public void OnAdded()
    {
    }

    public void OnGameStarted()
    {
    }
}