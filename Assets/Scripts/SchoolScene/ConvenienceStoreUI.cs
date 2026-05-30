using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConvenienceStoreUI : MonoBehaviour
{
    public static ConvenienceStoreUI Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject rootPanel;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text itemDescriptionText;
    [SerializeField] private TMP_Text itemCostText;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Button purchaseButton;

    private ShopItemType selectedItemType = ShopItemType.EnergyDrink;

    public bool IsOpen => rootPanel != null && rootPanel.activeSelf;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        if (rootPanel != null)
            rootPanel.SetActive(false);
    }

    public void OpenStore()
    {
        if (rootPanel == null) return;

        rootPanel.SetActive(true);
        messageText.text = "";
        SelectItemByInt((int)ShopItemType.EnergyDrink);
    }

    public void CloseStore()
    {
        if (rootPanel == null) return;

        rootPanel.SetActive(false);
    }

    public void SelectItemByInt(int itemTypeValue)
    {
        selectedItemType = (ShopItemType)itemTypeValue;
        RefreshSelectedItemUI();
    }

    public void OnClickPurchase()
    {
        if (GameManager.Instance == null) return;

        bool success = GameManager.Instance.TryBuyShopItem(selectedItemType, out string message);

        if (messageText != null)
            messageText.text = message;

        RefreshSelectedItemUI();
    }

    private void RefreshSelectedItemUI()
    {
        if (GameManager.Instance == null) return;

        if (itemNameText != null)
            itemNameText.text = GameManager.Instance.GetShopItemName(selectedItemType);

        if (itemDescriptionText != null)
            itemDescriptionText.text = GameManager.Instance.GetShopItemDescription(selectedItemType);

        if (itemCostText != null)
            itemCostText.text = $"가격: {GameManager.Instance.GetShopItemCost(selectedItemType)}";

        if (purchaseButton != null)
        {
            bool canBuy = GameManager.Instance.CanBuyShopItem(selectedItemType, out _);
            purchaseButton.interactable = canBuy;
        }
    }
}