using UnityEngine;

public class VoidInfrastructureTier : ResourceTier
{
    public ResourceTier producedTier; // The tier this building produces
    private double accumulatedProduction = 0f;

    void Start()
    {
        TickManager.Instance.OnTick += ProduceResources;
        UpgradeManager.Instance.RecalculateUpgrades();
    }

    protected override void ProduceResources()
    {
        double productionPerSecond = CalculateTotalProduction();
        double productionPerTick = productionPerSecond * TickManager.Instance.tickInterval;

        if (resourceType == ResourceType.VoidDust)
        {
            GameManager.Instance.VoidDust += productionPerTick;
            GameManager.Instance.TotalVoidDust += productionPerTick;
            GameManager.Instance.UpdateVoidDustBonus();
        }
        else if (producedTier != null)
        {
            accumulatedProduction += productionPerTick;
            int unitsToAdd = (int)accumulatedProduction;
            if (unitsToAdd > 0)
            {
                producedTier.generatedUnits += unitsToAdd;
                accumulatedProduction -= unitsToAdd;
            }
        }
    }

    public override double GetNextUnitCost()
    {
        return Mathf.Ceil(baseCost * Mathf.Pow(costMultiplier, (float)boughtUnits));
    }

    public override bool CanBuy(double cost)
    {
        return GameManager.Instance.VoidOpals >= cost;
    }

    public override void PurchaseUnits(int quantity)
    {
        double unitCost = GetNextUnitCost();
        double totalCost = GetTotalCost(quantity);
        if (quantity > 1 && !CanBuy(totalCost))
        {
            // Prevent purchase if quantity is higher than 1 and not enough resources
            int affordableQuantity = GetMaxAffordableUnits(quantity);
            if (affordableQuantity > 0)
            {
                totalCost = GetTotalCost(affordableQuantity);
                GameManager.Instance.VoidOpals -= totalCost;
                boughtUnits += affordableQuantity;
                UpgradeManager.Instance.RecalculateUpgrades();
            }
            return;
        }

        if (CanBuy(totalCost))
        {
            GameManager.Instance.VoidOpals -= totalCost;
            boughtUnits += quantity;
            UpgradeManager.Instance.RecalculateUpgrades();
        }
        else
        {
            // Attempt to purchase as many as possible
            int affordableQuantity = GetMaxAffordableUnits(quantity);
            if (affordableQuantity > 0)
            {
                totalCost = GetTotalCost(affordableQuantity);
                GameManager.Instance.VoidOpals -= totalCost;
                boughtUnits += affordableQuantity;
                UpgradeManager.Instance.RecalculateUpgrades();
            }
        }
    }

    private int GetMaxAffordableUnits(int maxQuantity)
    {
        int affordableUnits = 0;
        double totalCost = 0f;
        double currentUnits = boughtUnits;
        for (int i = 0; i < maxQuantity; i++)
        {
            double cost = Mathf.Ceil(baseCost * Mathf.Pow(costMultiplier, (float)currentUnits));
            totalCost += cost;
            if (GameManager.Instance.VoidOpals >= totalCost)
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

    public override void ResetTier1()
    {
        generatedUnits = 0;
        productionMultiplier = 0f; // Reset to default if necessary
    }
}