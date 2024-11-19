using UnityEngine;
using System;

public class Upgrade
{
    public string upgradeName;
    public float cost;
    public bool isPurchased = false;
    public Action effect;
    public ResourceTier targetTier;
    public ResourceType costType;
    public string description;
    public ResourceTier sourceTier;
    public float perUnitMultiplierIncrease;
    public float multiplierIncrease;

    public bool CanAfford()
    {
        return costType switch
        {
            ResourceType.CosmicCredits => GameManager.Instance.CosmicCredits >= cost,
            ResourceType.VoidOpals => GameManager.Instance.VoidOpals >= cost,
            _ => false
        };
    }

    public void Purchase()
    {
        if (!isPurchased && CanAfford())
        {
            switch (costType)
            {
                case ResourceType.CosmicCredits:
                    GameManager.Instance.CosmicCredits -= cost;
                    break;
                case ResourceType.VoidOpals:
                    GameManager.Instance.VoidOpals -= (int)cost;
                    break;
            }
            isPurchased = true;
            ApplyEffect();
            
            // Force recalculation of all upgrades after purchase
            UpgradeManager.Instance.RecalculateUpgrades();
        }
    }

    private void ApplyEffect()
    {
        effect?.Invoke();
        if (upgradeName == "Universal Synergy")
        {
            // Universal Synergy is handled by RecalculateUpgrades
            return;
        }
        
        if (perUnitMultiplierIncrease > 0f && sourceTier != null)
        {
            float totalMultiplier = (float)sourceTier.unitsOwned * perUnitMultiplierIncrease;
            targetTier.IncreaseProductionMultiplier(totalMultiplier);
        }
        else
        {
            targetTier.IncreaseProductionMultiplier(multiplierIncrease);
        }
    }
}