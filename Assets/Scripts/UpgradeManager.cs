using UnityEngine;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;
    public List<Upgrade> upgrades = new List<Upgrade>();
    
    [SerializeField] private ResourceTier starHarvesterTier;
    [SerializeField] private ResourceTier nebulaRefineryTier;
    [SerializeField] private ResourceTier QuantumMarket;
    [SerializeField] private ResourceTier AstroChamber;
    [SerializeField] private ResourceTier CelestialVault;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
            
        InitializeUpgrades();
    }
    
    private void InitializeUpgrades()
    {
        upgrades.Clear(); // Clear the list first to avoid duplicates
        
        Upgrade nebulaBoostUpgrade = new Upgrade
        {
            upgradeName = "Nebula Synergy", // % bonus per Nebula Refinery to Star Harvester
            description = "Each Nebula Refinery owned increases Star Harvester production by 70%.",
            cost = 100000f,
            costType = ResourceType.CosmicCredits,
            targetTier = starHarvesterTier,
            sourceTier = nebulaRefineryTier,
            perUnitMultiplierIncrease = 0.35f // 10% bonus per Nebula Refinery
        };
        
        Upgrade quantumBoostUpgrade = new Upgrade
        {
            upgradeName = "Quantum Boost",
            description = "Each Quantum Core owned increases Nebula Refinery production by 25%.",
            cost = 1000000f,
            costType = ResourceType.CosmicCredits,
            targetTier = nebulaRefineryTier,
            sourceTier = QuantumMarket,
            perUnitMultiplierIncrease = 0.55f  // 25% bonus per Quantum Core
        };

        Upgrade astroQuantumBoost = new Upgrade
        {
            upgradeName = "AstroQuantum Boost",
            description = "Each Astro Chamber owned increases Quantum Market production by 40%.",
            cost = 10000000f,
            costType = ResourceType.CosmicCredits,
            targetTier = QuantumMarket,
            sourceTier = AstroChamber,
            perUnitMultiplierIncrease = 0.75f
        };

        Upgrade celestialEnhancement = new Upgrade
        {
            upgradeName = "Celestial Enhancement",
            description = "Each Celestial Vault owned increases Astro Chamber production by 50%.",
            cost = 100000000f,
            costType = ResourceType.CosmicCredits,
            targetTier = AstroChamber,
            sourceTier = CelestialVault,
            perUnitMultiplierIncrease = 1f
        };

        Upgrade universalSynergy = new Upgrade
        {
            upgradeName = "Universal Synergy",
            description = "Total owned assets boost Celestial Vault production.",
            cost = 1000000000f,
            costType = ResourceType.CosmicCredits,
            targetTier = CelestialVault,
            sourceTier = null, // Special case handled in RecalculateUpgrades
            perUnitMultiplierIncrease = 0.55f // 25% bonus per owned asset
        };

        upgrades.Add(nebulaBoostUpgrade);
        upgrades.Add(quantumBoostUpgrade);
        upgrades.Add(astroQuantumBoost);
        upgrades.Add(celestialEnhancement);
        upgrades.Add(universalSynergy);
    }

    private int CalculateTotalAssets()
    {
        // Removed the upgrade.isPurchased check to count all assets regardless of upgrade status
        return (int) starHarvesterTier.unitsOwned +
               (int) nebulaRefineryTier.unitsOwned +
               (int) QuantumMarket.unitsOwned +
               (int) AstroChamber.unitsOwned +
               (int) CelestialVault.unitsOwned;
    }

    public Upgrade GetUpgradeByName(string name)
    {
        var upgrade = upgrades.Find(u => u.upgradeName == name);
        if (upgrade == null)
        {
            Debug.LogError($"No upgrade found with name: {name}");
        }
        return upgrade;
    }

    public void RecalculateUpgrades()
    {
        foreach (var upgrade in upgrades)
        {
            if (!upgrade.isPurchased) continue; // Skip if upgrade isn't purchased

            if (upgrade.upgradeName == "Universal Synergy")
            {
                int totalAssets = CalculateTotalAssets();
                float totalBonus = totalAssets * upgrade.perUnitMultiplierIncrease;
                upgrade.targetTier.productionMultiplier = 0f;
                upgrade.targetTier.IncreaseProductionMultiplier(totalBonus);
            }
            else if (upgrade.sourceTier != null)
            {
                double totalBonus = upgrade.sourceTier.unitsOwned * upgrade.perUnitMultiplierIncrease;
                upgrade.targetTier.productionMultiplier = 0f;
                upgrade.targetTier.IncreaseProductionMultiplier(totalBonus);
            }
        }
    }
}