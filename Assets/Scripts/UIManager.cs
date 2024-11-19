using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class UIManager : MonoBehaviour
{
    [System.Serializable]
    public struct ProductionUI
    {
        public Button button;
        public TMP_Text buttonText;
        public ResourceTier tier;
        public TMP_Text unitsText;
        public TMP_Text productionText;
    }

    [System.Serializable]
    public struct VoidInfrastructureUI
    {
        public Button button;
        public TMP_Text buttonText;
        public VoidInfrastructureTier tier;
        public TMP_Text unitsText;
        public TMP_Text productionText;
    }

    [Header("General UI")]
    public TMP_Text cosmicCreditsText;
    public TMP_Text voidOpalsText;
    public Image voidOpalsImage;
    public TMP_Text voidDustText;
    public Image voidDustImage;
    public TMP_Text rebirthText;
    public Button rebirthButton;
    public Button stellarProductionButton;
    public Button voidInfrastructureButton;

    [Header("Panels")]
    public GameObject stellarProductionPanel;
    public GameObject voidInfrastructurePanel;

    [Header("Stellar Production")]
    public ProductionUI starHarvester;
    public ProductionUI nebulaRefinery;
    public ProductionUI quantumMarket;
    public ProductionUI astroChamber;
    public ProductionUI celestialVault;
    public ProductionUI stellarCatalyst;

    [Header("Void Infrastructure")]
    public VoidInfrastructureUI dustCollector;
    public VoidInfrastructureUI dustSynthesizer;
    public VoidInfrastructureUI particleRefinery;
    public VoidInfrastructureUI etherReactor;
    public VoidInfrastructureUI dimensionalNexus;

    [Header("Upgrades")]
    public GameObject upgradeButtonsParent;
    public List<UpgradeButton> upgradeButtons;
    public TMP_Text sharedUpgradeCostText;
    public TMP_Text sharedUpgradeDescriptionText;

    [Header("Purchase Quantity")]
    public Button[] quantityButtons;
    public int[] purchaseQuantities = { 1, 10, 100, 1000, 10000, 100000 };
    private int currentPurchaseQuantity = 1;

    private Color defaultColor;
    private Color affordableButtonColor;
    private Color selectedPurchaseButtonColor;

    private ProductionUI[] productionUIElements;
    private VoidInfrastructureUI[] voidInfrastructureUIElements;

    [Header("Starfield")]
    public GameObject starfield;
    public Transform starfieldParent;
    public float parallaxSpeed = 0.1f;
    private RectTransform[] starfieldRectTransforms = new RectTransform[2];

    private void Start()
    {
        if (ColorManager.Instance != null)
        {
            defaultColor = ColorManager.Instance.stellarWhite;
            affordableButtonColor = ColorManager.Instance.energeticGreen;
            selectedPurchaseButtonColor = ColorManager.Instance.nebulaOrange;
        }
        else
        {
            Debug.LogError("ColorManager instance is not initialized.");
        }

        InitializeArrays();
        InitializeUI();
        SetupButtonListeners();
        HideInitialUI();

        if (starfield != null && starfieldParent != null)
        {
            starfieldRectTransforms[0] = Instantiate(starfield, starfieldParent).GetComponent<RectTransform>();
            starfieldRectTransforms[1] = Instantiate(starfield, starfieldParent).GetComponent<RectTransform>();
            starfieldRectTransforms[1].anchoredPosition = new Vector2(starfieldRectTransforms[0].rect.width, 0);
        }
    }

    private void InitializeArrays()
    {
        productionUIElements = new[] 
        { 
            starHarvester, nebulaRefinery, quantumMarket, 
            astroChamber, celestialVault, stellarCatalyst 
        };

        voidInfrastructureUIElements = new[] 
        { 
            dustCollector, dustSynthesizer, particleRefinery, 
            etherReactor, dimensionalNexus 
        };
    }

    private void InitializeUI()
    {
        upgradeButtons = upgradeButtonsParent.GetComponentsInChildren<UpgradeButton>(true).ToList();
        foreach (var button in upgradeButtons)
        {
            button.gameObject.SetActive(true);
        }
    }

    private void SetupButtonListeners()
    {
        foreach (var ui in productionUIElements)
        {
            ui.button.onClick.AddListener(() => ui.tier.PurchaseUnits(currentPurchaseQuantity));
        }

        foreach (var ui in voidInfrastructureUIElements)
        {
            ui.button.onClick.AddListener(() => ui.tier.PurchaseUnits(currentPurchaseQuantity));
        }

        for (int i = 0; i < quantityButtons.Length; i++)
        {
            int quantity = purchaseQuantities[i];
            quantityButtons[i].onClick.AddListener(() => SetPurchaseQuantity(quantity));
        }

        rebirthButton.onClick.AddListener(RebirthManager.Instance.Rebirth);
    }

    private void HideInitialUI()
    {
        // Only hide void infrastructure elements initially
        foreach (var ui in voidInfrastructureUIElements)
        {
            SetUIElementsActive(ui, false);
        }

        // Hide everything except the first production element (starHarvester)
        for (int i = 1; i < productionUIElements.Length; i++)
        {
            SetUIElementsActive(productionUIElements[i], false);
        }

        voidInfrastructurePanel.SetActive(false);
        stellarProductionPanel.SetActive(true);
        voidInfrastructureButton.gameObject.SetActive(false);
        voidOpalsText.gameObject.SetActive(false);
        voidDustText.gameObject.SetActive(false);
        rebirthButton.gameObject.SetActive(false);
        rebirthText.gameObject.SetActive(false);
        voidDustImage.gameObject.SetActive(false);
        voidOpalsImage.gameObject.SetActive(false);
    }

    private void Update()
    {
        UpdateResourceTexts();
        UpdateBuildingUI();
        UpdateRebirthUI();
        UpdateUnlockConditions();
        UpdateStarfieldParallax();
    }

    private void UpdateResourceTexts()
    {
        cosmicCreditsText.text = $"{NumberFormatter.FormatNumber(GameManager.Instance.CosmicCredits)}";
        voidOpalsText.text = $"{NumberFormatter.FormatNumber(GameManager.Instance.VoidOpals)}";
        voidDustText.text = $"{NumberFormatter.FormatNumber(GameManager.Instance.VoidDust)}";
    }

    private void UpdateBuildingUI()
    {
        foreach (var ui in productionUIElements)
        {
            if (ui.tier == stellarCatalyst.tier)
            {
                UpdateStellarCatalystUI(ui);
            }
            else
            {
                UpdateProductionUI(ui);
            }
        }

        foreach (var ui in voidInfrastructureUIElements)
        {
            UpdateVoidInfrastructureUI(ui);
        }
    }

    private void UpdateProductionUI(ProductionUI ui)
    {
        if (!ShouldShowUI(ui.tier.unlockAmount)) return;

        ui.unitsText.text = $"{ui.tier.tierName}: {ui.tier.boughtUnits} [{NumberFormatter.FormatNumber(ui.tier.generatedUnits)}]";
        ui.productionText.text = $"Cosmic Credits/Sec: {NumberFormatter.FormatNumber(ui.tier.CalculateTotalProduction())}";
        ui.buttonText.text = $"Cost: {NumberFormatter.FormatNumber(ui.tier.GetTotalCost(1))}";
        UpdateButtonColor(ui.button, ui.tier.GetTotalCost(1));
    }

    private void UpdateVoidInfrastructureUI(VoidInfrastructureUI ui)
    {
        if (!ShouldShowVoidUI(ui.tier)) return;

        ui.unitsText.text = $"{ui.tier.tierName}: {ui.tier.boughtUnits} [{NumberFormatter.FormatNumber(ui.tier.generatedUnits)}]";

        if (ui.tier.producedTier != null)
        {
            ui.productionText.text = $"Produces: {ui.tier.producedTier.tierName} per second";
        }
        else if (ui.tier.resourceType == ResourceType.VoidDust)
        {
            ui.productionText.text = $"Produces: {NumberFormatter.FormatNumber(ui.tier.CalculateTotalProduction())} Void Dust per second";
        }
        else
        {
            ui.productionText.text = $"Produces: {NumberFormatter.FormatNumber(ui.tier.CalculateTotalProduction())} per second";
        }

        ui.buttonText.text = $"Cost: {NumberFormatter.FormatNumber(ui.tier.GetNextUnitCost())} Void Opals";
        UpdateButtonColor(ui.button, ui.tier.GetNextUnitCost(), true);
    }

    private void UpdateStellarCatalystUI(ProductionUI ui)
    {
        if (!ShouldShowUI(ui.tier.unlockAmount)) return;

        ui.unitsText.text = $"{ui.tier.tierName}: {ui.tier.unitsOwned}";
        ui.productionText.text = $"Boosts all Stellar Production Assets by {NumberFormatter.FormatNumber(GameManager.Instance.GetStellarCatalystMultiplier())}x";
        ui.buttonText.text = $"Cost: {NumberFormatter.FormatNumber(ui.tier.GetTotalCost(1))}";
        UpdateButtonColor(ui.button, ui.tier.GetTotalCost(1));
    }

    private bool ShouldShowUI(float unlockAmount) => 
        GameManager.Instance.TotalCosmicCredits >= unlockAmount;

    private bool ShouldShowVoidUI(VoidInfrastructureTier tier) => 
        GameManager.Instance.TotalVoidOpals >= tier.unlockAmount;

    private void UpdateButtonColor(Button button, double cost, bool isVoidCost = false)
    {
        double currentResource = isVoidCost ? GameManager.Instance.VoidOpals : GameManager.Instance.CosmicCredits;
        button.image.color = currentResource >= cost ? affordableButtonColor : defaultColor;
    }

    private void SetUIElementsActive(ProductionUI ui, bool active)
    {
        ui.button.gameObject.SetActive(active);
        ui.unitsText.gameObject.SetActive(active);
        ui.productionText.gameObject.SetActive(active);
    }

    private void SetUIElementsActive(VoidInfrastructureUI ui, bool active)
    {
        ui.button.gameObject.SetActive(active);
        ui.unitsText.gameObject.SetActive(active);
        ui.productionText.gameObject.SetActive(active);
    }

    private void UpdateUnlockConditions()
    {
        // Check unlock conditions for each production UI element
        foreach (var ui in productionUIElements)
        {
            bool shouldShow = GameManager.Instance.TotalCosmicCredits >= ui.tier.unlockAmount;
            SetUIElementsActive(ui, shouldShow);
        }

        // Check for void infrastructure unlock
        bool voidUnlocked = GameManager.Instance.TotalVoidOpals >= 1;
        if (voidUnlocked)
        {
            voidInfrastructureButton.gameObject.SetActive(true);
            voidOpalsText.gameObject.SetActive(true);
            voidDustText.gameObject.SetActive(true);
            voidDustImage.gameObject.SetActive(true);
            voidOpalsImage.gameObject.SetActive(true);

            // Show all void infrastructure elements when void opals are unlocked
            foreach (var ui in voidInfrastructureUIElements)
            {
                SetUIElementsActive(ui, true);
            }
        }

        // Original rebirth unlock check
        if (GameManager.Instance.TotalCosmicCredits >= 1e6f)
        {
            rebirthButton.gameObject.SetActive(true);
            rebirthText.gameObject.SetActive(true);
        }

        rebirthButton.interactable = RebirthManager.Instance.CanRebirth();
    }

    private void UpdateRebirthUI()
    {
        rebirthText.text = $"Embark on a new journey by selling your company, but in return you will get Void Opals!\n" +
                          $"You need: {NumberFormatter.FormatNumber(RebirthManager.Instance.rebirthThreshold)} Cosmic Credits\n" +
                          $"You have: {NumberFormatter.FormatNumber(GameManager.Instance.CosmicCredits)} Cosmic Credits\n" +
                          $"Rebirth now to earn: {NumberFormatter.FormatNumber(RebirthManager.Instance.CalcualteVoidOpalsToEarn())} Void Opals";
    }

    private void SetPurchaseQuantity(int quantity)
    {
        currentPurchaseQuantity = quantity;
        UpdatePurchaseQuantityButtonColors();
    }

    private void UpdatePurchaseQuantityButtonColors()
    {
        for (int i = 0; i < quantityButtons.Length; i++)
        {
            quantityButtons[i].image.color = purchaseQuantities[i] == currentPurchaseQuantity 
                ? selectedPurchaseButtonColor 
                : defaultColor;
        }
    }

    public void ToggleStellarProductionPanel()
    {
        stellarProductionPanel.SetActive(true);
        voidInfrastructurePanel.SetActive(false);
    }

    public void ToggleVoidInfrastructurePanel()
    {
        voidInfrastructurePanel.SetActive(true);
        stellarProductionPanel.SetActive(false);
    }

    private void UpdateStarfieldParallax()
    {
        if (starfieldRectTransforms[0] != null && starfieldRectTransforms[1] != null)
        {
            for (int i = 0; i < starfieldRectTransforms.Length; i++)
            {
                Vector2 offset = starfieldRectTransforms[i].anchoredPosition;
                offset.x -= parallaxSpeed * Time.deltaTime;
                if (offset.x <= -starfieldRectTransforms[i].rect.width)
                {
                    offset.x = starfieldRectTransforms[(i + 1) % 2].anchoredPosition.x + starfieldRectTransforms[i].rect.width;
                }
                starfieldRectTransforms[i].anchoredPosition = offset;
            }
        }
    }
}
