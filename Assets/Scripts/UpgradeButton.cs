using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UpgradeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string upgradeName; // Set this in inspector
    private Upgrade upgrade;
    private Button button;
    private UIManager uiManager;

    private Color purchasedColor;
    private Color canBuyColor;
    private Color cantBuyColor;

    void Start()
    {
        button = GetComponent<Button>();
        upgrade = UpgradeManager.Instance.GetUpgradeByName(upgradeName);
        uiManager = FindFirstObjectByType<UIManager>();
        purchasedColor = ColorManager.Instance.energeticGreen;
        canBuyColor = ColorManager.Instance.stellarWhite;
        cantBuyColor = ColorManager.Instance.warningRed;
        button.onClick.AddListener(OnClick);
        HideDescription();
    }

    void OnClick()
    {
        upgrade.Purchase();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowDescription();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideDescription();
    }

    private void ShowDescription()
    {
        uiManager.sharedUpgradeCostText.text = $"Cost: {NumberFormatter.FormatNumber(upgrade.cost)} {GetCurrencySymbol(upgrade.costType)}";
        uiManager.sharedUpgradeDescriptionText.text = $"{upgrade.upgradeName}: {upgrade.description}\nCurrent Bonus: {NumberFormatter.FormatNumber(upgrade.targetTier.productionMultiplier * 100)}%";
    }

    private void HideDescription()
    {
        uiManager.sharedUpgradeCostText.text = "";
        uiManager.sharedUpgradeDescriptionText.text = "";
    }

    void Update()
    {
        UpdateButtonAppearance();
    }

    private void UpdateButtonAppearance()
    {
        Image buttonImage = button.GetComponent<Image>();
        if (upgrade.isPurchased)
        {
            buttonImage.color = purchasedColor;
        }
        else if (upgrade.CanAfford())
        {
            buttonImage.color = canBuyColor;
        }
        else
        {
            buttonImage.color = cantBuyColor;
        }
    }

    private string GetCurrencySymbol(ResourceType costType)
    {
        return costType switch
        {
            ResourceType.CosmicCredits => "CC",
            ResourceType.VoidOpals => "VO",
            _ => "??"
        };
    }
}