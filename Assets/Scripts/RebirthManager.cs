using UnityEngine;

public class RebirthManager : MonoBehaviour
{
    public static RebirthManager Instance;

    public float rebirthThreshold = 1e10f;
    public float A = 100f;
    public float B = 0.75f;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public bool CanRebirth()
    {
        return GameManager.Instance.CosmicCredits >= rebirthThreshold;
    }

    public void Rebirth()
    {
        if (CanRebirth())
        {
            double totalCosmicCredits = GameManager.Instance.CosmicCredits;
            int voidOpalsEarned = Mathf.FloorToInt(A * Mathf.Pow((float)(totalCosmicCredits / rebirthThreshold), B));
            GameManager.Instance.VoidOpals += voidOpalsEarned;
            GameManager.Instance.TotalVoidOpals += voidOpalsEarned;

            HandleRebirth();
        }
    }

    public void HandleRebirth()
    {
        ResetTier1();
        GameManager.Instance.TimeSinceLastRebirth = 0f;
    }

    public void ResetTier1()
    {
        GameManager.Instance.CosmicCredits = 90;
        foreach (var tier in GameManager.Instance.resourceTiers)
        {
            tier.ResetTier1();
        }

        foreach (var tier in GameManager.Instance.resourceTiers)
        {
            tier.productionMultiplier = 0f; // Reset production multiplier
        }

        foreach (var upgrade in UpgradeManager.Instance.upgrades)
        {
            upgrade.isPurchased = false; // Reset upgrade purchase status
        }

        // Recalculate upgrades to ensure all multipliers are reset
        UpgradeManager.Instance.RecalculateUpgrades();
        GameManager.Instance.stellarCatalystMultiplier = 1f; // Reset stellar catalyst multiplier
        GameManager.Instance.voidDustBonusMultiplier = 1f; // Reset void dust bonus multiplier
        GameManager.Instance.VoidDust = 0; // Reset void dust
    }

    public int CalcualteVoidOpalsToEarn()
    {
        double CosmicCredits = GameManager.Instance.CosmicCredits;
        float rebirthThreshold = this.rebirthThreshold;

        return Mathf.FloorToInt(A * Mathf.Pow((float)(CosmicCredits / rebirthThreshold), B));
    }
}