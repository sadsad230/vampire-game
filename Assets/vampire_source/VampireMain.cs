using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VampireMain : MonoBehaviour, IGameSystem
{
    [Header("Main")]
    public UpgradeList upgrades = new UpgradeList();
    public AudioSystem AudioSystem;
    
    [Header("PlayerStats")]
    public float Speed;
    public int DamageBase;
    public int DamageByContact;
    public float DashRange;
    public float DashCd;
    public float BulletSpawnDelay;
    public float BulletSpeed;
    public float BulletLifeTime;
    public int PlayerMaxHp;
    public int PlayerHits;
    public float XPCollectRange;
    
    public GameObject Player;
    public GameObject PlayerBullet;
    
    [Header("PlayerState")]
    public bool IsCanDash;
    public bool IsPerformingDash;

    [Header("Projectiles")]
    public GameObject Projectile;
    public float ProjectileSpeed;
    public GameObject OrbitalProjectile;
    public GameObject OrbitWeapon;
    
    public GameObject CurrentOrbitalWeapon;
    
    [Header("Enemies")]
    public GameObject EnemyObject;
    public GameObject Boss;
    public GameObject BossSpawnPos;
    
    public float SpawnCooldown;
    public float CooldownHitPlayer;
    public List<Enemy> Enemies = new List<Enemy>();
    
    private float enemyDamageCd = 0.2f;
    private bool _isSummon;
    
    
    [Header("XP")]
    public GameObject XPOrb;
    public List<GameObject> XPOrbs = new List<GameObject>();

    public int XPCollected;
    public int XPCollectedMax => LevelUpProgression[Mathf.Min(LevelUpProgression.Count - 1, level)];
    public int XPTotal = 0;
    public int XPTotalToSummon = 500;
    public List<int> LevelUpProgression = new List<int>();
    private int level;
    

    [Header("Other")]
    public ObjectPool DamageText;
    
    private TimeUntil cooldownHitPlayerEnds = 0;
    private TimeUntil toSpawnEnemy = 0;
    
    private void Awake()
    {
        G.Init();
        G.Add(this);
        G.Add(AudioSystem);
        G.Start();

        G.Get<AudioSystem>().SetVolume(AudioType.SFX, 0.3f);
        
        
        upgrade1Btn.onClick.AddListener(() => UpgradeBuy(0));
        upgrade2Btn.onClick.AddListener(() => UpgradeBuy(1));
        upgrade3Btn.onClick.AddListener(() => UpgradeBuy(2));
    }
    
    #region UI

    [Header("Hud")]
    public Slider summonSlider;
    public Slider HpSlider;
    
    private void UpdateUI()
    {
        summonSlider.value = XPTotal;
        summonSlider.maxValue = XPTotalToSummon;
        HpSlider.value = GetPlayerMaxHp() - PlayerHits;
        HpSlider.maxValue = GetPlayerMaxHp();
    }

    #endregion
    
    #region UPGRADE_UI

    [Header("Upgrade Ui")]
    public Canvas upgrade_ui;

    public List<RectTransform> upgrades_sine_fun = new List<RectTransform>();

    public Button upgrade1Btn;
    public Image upgrade1Image;
    public TextMeshProUGUI upgrade1Name;
    public TextMeshProUGUI upgrade1Desc;

    public Button upgrade2Btn;
    public Image upgrade2Image;
    public TextMeshProUGUI upgrade2Name;
    public TextMeshProUGUI upgrade2Desc;

    public Button upgrade3Btn;
    public Image upgrade3Image;
    public TextMeshProUGUI upgrade3Name;
    public TextMeshProUGUI upgrade3Desc;
    
    public enum UpgradeContext
    {
        XP,
    }

    private void ShowUpgrades(UpgradeContext ctx = UpgradeContext.XP)
    {
        G.IsPaused = true;
        upgrade_ui.gameObject.SetActive(true);
        
        upgrades.NewPick(ctx);
        ShowUpgrade(upgrade1Image, upgrade1Name, upgrade1Desc, upgrades.PickNextRandom());
        ShowUpgrade(upgrade2Image, upgrade2Name, upgrade2Desc, upgrades.PickNextRandom());
        ShowUpgrade(upgrade3Image, upgrade3Name, upgrade3Desc, upgrades.PickNextRandom());
    }

    private void ShowUpgrade(Image image, TextMeshProUGUI name, TextMeshProUGUI desc, VUpgrade upgrade)
    {
        name.text = upgrade.name;
        desc.text = upgrade.desc;
        //image
    }
    
    private void UpgradeBuy(int p0)
    {
        upgrade_ui.gameObject.SetActive(false);
        G.IsPaused = false;
        upgrades.AddUpgrade(p0);
        
        LevelUp();
    }

    private void LevelUp()
    {
        level++;
        SpawnCooldown -= 0.05f;
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
        
        if (Input.GetKeyDown(KeyCode.Alpha7))
            ShowUpgrades();
    }

    private void FixedUpdate()
    {
        if (G.IsPaused)
            return;
        
        UpdateXP();
        SpawnEnemy();

        if (XPTotal >= XPTotalToSummon)
        {
            if (!_isSummon)
            {
                _isSummon = true;
                
                var boss = Instantiate(Boss, BossSpawnPos.transform.position, Quaternion.identity);
                boss.GetComponent<Boss>().NumberOfPoints = 6;
            }
        }
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
        G.Get<AudioSystem>().PlaySFX("pickup");
        
        XPCollected++;
        XPTotal++;

        if (XPCollected == XPCollectedMax)
        {
            XPCollected = 0;
            ShowUpgrades();
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

    public void DealDamage(Enemy en, int damage, Vector3 position)
    {
        if (!en.EndsDamageCd)
            return;
        en.EndsDamageCd = enemyDamageCd;
        
        en.Damage += damage;
        
        AnimateDamageText(damage, position);

        if (en.Hp() <= 0)
        {
            G.Get<AudioSystem>().PlaySFX("hit");
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

    public void AnimateDamageText(int damage, Vector3 position)
    {
        var requestObject = DamageText.RequestObject();
        requestObject.GetComponent<TMP_Text>().text = damage.ToString();
        requestObject.transform.position = position + Vector3.up;
        
        requestObject.transform.localScale = Vector3.one;
        requestObject.transform.DOBlendableMoveBy(Vector3.up * 2, 1f);
        requestObject.transform.DOScale(0, 1f).OnComplete(() =>
        {
            requestObject.GetComponent<PoolItem>().ReturnToPool();
        });
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
        maxHp += upgrades.CountUpgrade("Learn Health");
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
        speed += upgrades.CountUpgrade("Learn Speed");
        return speed;
    }

    public float GetDashRange()
    {
        var range = DashRange;
        return range;
    }
    
    public int GetPlayerDamage()
    {
        var damage = DamageBase;
        damage += upgrades.CountUpgrade("Learn Damage") * 5;
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

    public List<Enemy> CloneEnemies()
    {
        return new List<Enemy>(Enemies);
    }

    public void DestroyOrbitWeapon()
    {
        if (CurrentOrbitalWeapon != null)
        {
            Destroy(CurrentOrbitalWeapon);
        }
    }

    public void CreateOrbitWeapon(int count)
    {
        var obj = Instantiate(OrbitWeapon, Player.transform);
        var weapon = obj.GetComponent<OrbitWeapon>();
        weapon.InitOrbitSystem(count, OrbitalProjectile);

        CurrentOrbitalWeapon = obj;
    }

    public void OnAdded()
    {
    }

    public void OnGameStarted()
    {
    }
}