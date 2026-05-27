using System.Collections.Generic;
using UnityEngine;

public class VUpgrade
{
    public string name;
    public string desc;

    public bool xpComplete;
    public bool unique;
    public bool priority;
}

public class UpgradeList
{
    public List<VUpgrade> upgrades = new List<VUpgrade>()
    {
        new VUpgrade() { name = "Dash", desc = "You gain ability to perform dash", xpComplete = true, unique = true },

        new VUpgrade() { name = "Learn Damage", desc = "Increases your base damage by 5", xpComplete = true },
        new VUpgrade() { name = "Learn Speed", desc = "Increases your move speed", xpComplete = true },
        new VUpgrade() { name = "Learn Health", desc = "Increases your max health", xpComplete = true },

        new VUpgrade() { name = "Collect", desc = "Collect all xp orbs", xpComplete = true },
        new VUpgrade() { name = "Hit", desc = "Hit every enemy for 50 damage", xpComplete = true },
    };

    public List<VUpgrade> purchasedUpgrades = new List<VUpgrade>();
    
    public List<VUpgrade> upgradesInPick = new List<VUpgrade>();
    
    private List<VUpgrade> suggestedPriority =  new List<VUpgrade>();

    private VampireMain.UpgradeContext ctx;

    public void NewPick(VampireMain.UpgradeContext ctx)
    {
        this.ctx = ctx;
        upgradesInPick = new List<VUpgrade>();
    }

    public VUpgrade PickNextRandom()
    {
        while (true)
        {
            var gupgrades = upgrades.FindAll(x => !IsTaken(x) && IsInContext(x));

            if (upgradesInPick.Count == 0)
            {
                foreach (var gu in gupgrades)
                {
                    if (!upgrades.Contains(gu) && !suggestedPriority.Contains(gu) && gu.priority)
                    {
                        suggestedPriority.Add(gu);
                        upgradesInPick.Add(gu);
                        return gu;
                    }
                }
            }

            var randomUpgrade = gupgrades.GetRandom();
            if (upgradesInPick.Contains(randomUpgrade)) continue;
            upgradesInPick.Add(randomUpgrade);
            return randomUpgrade;
        }
    }
    
    public void AddUpgrade(int p0)
    {
        AddUpgrade(upgradesInPick[p0].name);
    }

    public void AddUpgrade(string name)
    {
        if (name == "Collect")
        {
            var xpOrbs = new List<GameObject>(G.vamp.XPOrbs);
            foreach (var xp in xpOrbs)
                G.vamp.XpCollect(xp);
            G.vamp.XPOrbs.Clear();
            return;
        }
        
        var vUpgrade = upgrades.Find(m => m.name == name);
        if (vUpgrade == null)
            Debug.Log("failed to add " + name);
        purchasedUpgrades.Add(vUpgrade);
    }

    private bool IsInContext(VUpgrade upgrade)
    {
        if (ctx == VampireMain.UpgradeContext.XP)
            return upgrade.xpComplete;
        
        return false;
    }
    
    private bool IsTaken(VUpgrade upgrade)
    {
        if (upgrade.unique && HasUpgrade(upgrade.name))
            return true;
        return false;
    }

    public int CountUpgrade(string name)
    {
        var n = 0;

        foreach (var upg in purchasedUpgrades)
        {
            if (upg.name == name)
                n++;
        }
        return n;
    }

    public bool HasUpgrade(string name)
    {
        return CountUpgrade(name) > 0;
    }
}