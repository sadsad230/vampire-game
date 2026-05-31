using UnityEngine;

public class Chest : ManagedBehaviour
{
    public float Range = 2f;

    protected override void ManagedFixedUpdate(float dt)
    {
        base.ManagedFixedUpdate(dt);

        if (G.vamp.IsCollidingWithPlayer(transform.position, Range))
        {
            G.vamp.ShowUpgrades(VampireMain.UpgradeContext.Chest);
            Destroy(gameObject);
        }
    }
}
