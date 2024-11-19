using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    // Static fields
    public static GameManager Instance;

    [Header("Resources")]
    // Resource amounts
    public double CosmicCredits = 0.0;
    public double TotalCosmicCredits = 0.0;
    public double VoidOpals = 0.0;
    public double TotalVoidOpals = 0.0;
    public double VoidDust = 0.0;
    public double TotalVoidDust = 0.0;
    public double StellarCatalyst = 0.0;
    public double TotalStellarCatalyst = 0.0;

    // Multipliers
    public float stellarCatalystMultiplier = 1f;
    public float voidDustBonusMultiplier = 1f;

    // Other public fields
    public List<ResourceTier> resourceTiers;

    // Private fields
    private float timeSinceLastRebirth;

    // Properties
    public float TimeSinceLastRebirth
    {
        get => timeSinceLastRebirth;
        set => timeSinceLastRebirth = value;
    }

    // Unity lifecycle methods
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        resourceTiers = FindObjectsByType<ResourceTier>(FindObjectsSortMode.None).ToList();
    }

    private void Start()
    {
        timeSinceLastRebirth = 0f;
    }

    private void Update()
    {
        timeSinceLastRebirth += Time.deltaTime; 

        //press g to add 1e+6 cosmic credits
        if (Input.GetKeyDown(KeyCode.G))
        {
            CosmicCredits += 1e10;
            TotalCosmicCredits += 1e10;
        }
    }

    // Public methods
    public float GetTimeSinceLastRebirth()
    {
        return timeSinceLastRebirth;
    }

    public void UpdateGlobalMultiplier()
    {
        double totalCatalystsOwned = GetTotalCatalystsOwned();
        stellarCatalystMultiplier = 1f + Mathf.Pow((float)totalCatalystsOwned, 1.5f);
    }

    public float GetStellarCatalystMultiplier()
    {
        return stellarCatalystMultiplier;
    }

    public float GetGlobalMultiplier()
    {
        return stellarCatalystMultiplier * voidDustBonusMultiplier;
    }

    public void UpdateVoidDustBonus()
    {
        float M = 0.085f; // Adjust scaling factor as needed
        float D = (float) TotalVoidDust; // Total Void Dust owned

        if (D < 0f)
        {
            D = 0f;
        }

        voidDustBonusMultiplier = 1f + M * Mathf.Pow(D, 1f / 3f);
    }

    // Private methods
    private double GetTotalCatalystsOwned()
    {
        double totalCatalysts = 0;
        foreach (ResourceTier tier in resourceTiers)
        {
            if (tier.isStellarCatalyst)
            {
                totalCatalysts += tier.unitsOwned;
            }
        }
        return totalCatalysts;
    }
}
