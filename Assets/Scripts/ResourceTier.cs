using UnityEngine;

public class ResourceTier : MonoBehaviour
{
    public string tierName;
    public double baseProductionRate;
    public float baseCost;
    public double unitsOwned = 0;
    public double productionMultiplier = 0f; // Starts at 0% extra
    public float unlockAmount;
    public float costMultiplier = 1.15f;
    public bool isStellarCatalyst = false;
    public ResourceType resourceType;
    public double boughtUnits = 0;
    public double generatedUnits = 0;

    public double UnitsOwned => boughtUnits + generatedUnits;

    void Start()
    {
        TickManager.Instance.OnTick += ProduceResources;
        UpgradeManager.Instance.RecalculateUpgrades(); // Recalculate upgrades on start
    }

    public double CalculateProductionPerUnit()
    {
        double production = baseProductionRate * (1 + productionMultiplier);
        if (!isStellarCatalyst)
        {
            production *= GameManager.Instance.GetGlobalMultiplier();
        }
        return production;
    }

    public virtual double CalculateTotalProduction()
    {
        double totalUnits = UnitsOwned;
        double totalProduction = totalUnits * baseProductionRate;
        totalProduction *= (1 + productionMultiplier);

        // Apply global multiplier to appropriate tiers
        if (!isStellarCatalyst && resourceType != ResourceType.VoidDust)
        {
            totalProduction *= GameManager.Instance.GetGlobalMultiplier();
        }

        return totalProduction;
    }

    public void SetCostMultiplier(float multiplier)
    {
        costMultiplier = multiplier;
    }

    public virtual double GetNextUnitCost()
    {
        return Mathf.Ceil(baseCost * Mathf.Pow(costMultiplier, (float)boughtUnits));
    }

    public double GetTotalCost(int quantity)
    {
        double totalCost = 0f;
        double currentUnits = boughtUnits;
        for (int i = 0; i < quantity; i++)
        {
            double cost = Mathf.Ceil(baseCost * Mathf.Pow(costMultiplier, (float)currentUnits));
            totalCost += cost;
            currentUnits++;
        }
        return totalCost;
    }

    public virtual void PurchaseUnits(int quantity)
    {
        double totalCost = GetTotalCost(quantity);
        if (CanBuy(totalCost))
        {
            GameManager.Instance.CosmicCredits -= totalCost;
            boughtUnits += quantity;
            unitsOwned += quantity; // Update unitsOwned
            if (isStellarCatalyst)
            {
                GameManager.Instance.StellarCatalyst += quantity;
                GameManager.Instance.UpdateGlobalMultiplier();
            }
            UpgradeManager.Instance.RecalculateUpgrades();
        }
        else
        {
            int affordableQuantity = GetMaxAffordableUnits();
            if (affordableQuantity > 0)
            {
                totalCost = GetTotalCost(affordableQuantity);
                GameManager.Instance.CosmicCredits -= totalCost;
                boughtUnits += affordableQuantity;
                unitsOwned += affordableQuantity; // Update unitsOwned
                if (isStellarCatalyst)
                {
                    GameManager.Instance.StellarCatalyst += affordableQuantity;
                    GameManager.Instance.UpdateGlobalMultiplier();
                }
                UpgradeManager.Instance.RecalculateUpgrades();
            }
        }
    }

    private int GetMaxAffordableUnits()
    {
        int affordableUnits = 0;
        double totalCost = 0f;
        double currentUnits = boughtUnits;
        while (true)
        {
            double cost = Mathf.Ceil(baseCost * Mathf.Pow(costMultiplier, (float)currentUnits));
            totalCost += cost;
            if (GameManager.Instance.CosmicCredits >= totalCost)
            {
                affordableUnits++;
                currentUnits++;
            }
            else
            {
                break;
            }
        }
        return affordableUnits;
    }

    public void PurchaseUnit()
    {
        double cost = GetNextUnitCost();
        if (CanBuy(cost))
        {
            GameManager.Instance.CosmicCredits -= cost;
            boughtUnits++;
        }
    }

    public void IncreaseProductionMultiplier(double amount)
    {
        productionMultiplier += amount;
    }

    public virtual bool CanBuy(double cost)
    {
        return GameManager.Instance.CosmicCredits >= cost;
    }

    protected virtual void ProduceResources()
    {
        double productionPerSecond = CalculateTotalProduction();
        double productionPerTick = productionPerSecond * TickManager.Instance.tickInterval;

        switch (resourceType)
        {
            case ResourceType.CosmicCredits:
                GameManager.Instance.CosmicCredits += productionPerTick;
                GameManager.Instance.TotalCosmicCredits += productionPerTick;
                break;
            case ResourceType.VoidOpals:
                GameManager.Instance.VoidOpals += productionPerTick;
                GameManager.Instance.TotalVoidOpals += productionPerTick;
                break;
            case ResourceType.VoidDust:
                GameManager.Instance.VoidDust += productionPerTick;
                GameManager.Instance.TotalVoidDust += productionPerTick;
                break;
            case ResourceType.StellarCatalyst:
                GameManager.Instance.StellarCatalyst += productionPerTick;
                GameManager.Instance.TotalStellarCatalyst += productionPerTick;
                break;
            // Add other cases as needed
        }
    }

    public virtual void ResetTier1()
    {
        unitsOwned = 0;
        boughtUnits = 0;
        generatedUnits = 0;
        productionMultiplier = 0f; // Reset to default if necessary
    }

    private void OnDestroy()
    {
        TickManager.Instance.OnTick -= ProduceResources;
    }
}
