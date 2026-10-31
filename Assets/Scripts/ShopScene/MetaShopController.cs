
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MetaShopController : MonoBehaviour
{
    [Header("Coin")]
    [SerializeField] private TMP_Text coinText;

    [Header("Product List")]
    [SerializeField] private Transform productContent;
    [SerializeField] private ShopProductButtonUI productButtonPrefab;
    [SerializeField] private MetaShopProductData[] products;

    [Header("Detail Panel")]
    [SerializeField] private GameObject detailPanel;
    [SerializeField] private Image detailProductImage;
    [SerializeField] private TMP_Text detailProductName;
    [SerializeField] private TMP_Text detailDescription;
    [SerializeField] private TMP_Text detailPrice;
    [SerializeField] private TMP_Text detailOwnedText;
    [SerializeField] private TMP_Text messageText;

    [Header("Action Button")]
    [SerializeField] private Button buyButton;
    [SerializeField] private Image buyButtonImage;

    [Header("Button Sprites")]
    [SerializeField] private Sprite buyButtonSprite;
    [SerializeField] private Sprite characterEquipSprite;
    [SerializeField] private Sprite characterEquippedSprite;
    [SerializeField] private Sprite consumableUseSprite;
    [SerializeField] private Sprite consumableUsingSprite;

    private MetaShopProductData selectedProduct;

    private void Start()
    {
        if (detailPanel != null)
            detailPanel.SetActive(false);

        RefreshCoin();
        ShowCategory(ShopCategory.Character);
    }

    // =========================
    // 카테고리
    // =========================

    public void OnClickCharacterCategory()
    {
        ShowCategory(ShopCategory.Character);
    }

    public void OnClickConsumableCategory()
    {
        ShowCategory(ShopCategory.Consumable);
    }

    public void OnClickExtraCategory()
    {
        ShowCategory(ShopCategory.Extra);
    }

    private void ShowCategory(ShopCategory category)
    {
        if (productContent == null ||
            productButtonPrefab == null)
            return;

        for (int i = productContent.childCount - 1;
             i >= 0; i--)
        {
            GameObject child =
                productContent.GetChild(i).gameObject;

            child.SetActive(false);
            Destroy(child);
        }

        if (products == null)
            return;

        foreach (MetaShopProductData product in products)
        {
            if (product == null || product.category != category)
                continue;

            ShopProductButtonUI slot =
                Instantiate(productButtonPrefab, productContent);

            slot.Setup(product, this);
        }
    }

    // =========================
    // 상세창
    // =========================

    public void OpenDetail(MetaShopProductData product)
    {
        if (product == null)
            return;

        selectedProduct = product;

        if (detailPanel != null)
            detailPanel.SetActive(true);

        if (detailProductImage != null)
            detailProductImage.sprite = product.productImage;

        if (detailProductName != null)
            detailProductName.text = product.productName;

        if (detailDescription != null)
            detailDescription.text = product.description;

        if (detailPrice != null)
            detailPrice.text = $"{product.price} Coin";

        if (messageText != null)
            messageText.text = "";

        RefreshDetail();
    }

    public void CloseDetail()
    {
        selectedProduct = null;

        if (detailPanel != null)
            detailPanel.SetActive(false);
    }

    // =========================
    // 구매 / 장착
    // =========================

    public void OnClickBuy()
    {
        if (selectedProduct == null)
            return;

        string id = selectedProduct.productId;

        bool success = false;
        string message = "";

        if (selectedProduct.category == ShopCategory.Character)
        {
            if (MetaProgressSystem.IsCharacterUnlocked(id))
            {
                success = MetaProgressSystem.SelectCharacter(
    selectedProduct
);

                message = success
                    ? "캐릭터를 착용했습니다."
                    : "착용할 수 없습니다.";
            }
            else
            {
                success = MetaProgressSystem.BuyCharacter(
                    id, selectedProduct.price);

                message = success
                    ? "캐릭터를 구매했습니다."
                    : "Coin이 부족합니다.";
            }
        }
        else if (selectedProduct.category == ShopCategory.Consumable)
        {
            if (MetaProgressSystem.HasConsumable(id))
            {
                bool wasSelected =
                    MetaProgressSystem.IsConsumableSelected(id);

                success = MetaProgressSystem.SelectConsumable(
                    selectedProduct);

                if (success)
                {
                    message = wasSelected
                        ? "소모품 장착을 해제했습니다."
                        : "다음 게임에 사용할 소모품을 장착했습니다.";
                }
                else
                {
                    message = "사용할 수 없습니다.";
                }
            }
            else
            {
                success = MetaProgressSystem.BuyConsumable(
                    id, selectedProduct.price);

                message = success
                    ? "소모품을 구매했습니다."
                    : "Coin이 부족합니다.";
            }
        }
        else
        {
            message = "아직 구매할 수 없는 상품입니다.";
        }

        if (messageText != null)
            messageText.text = message;

        RefreshCoin();
        RefreshDetail();
    }

    // =========================
    // 버튼 이미지 및 상태
    // =========================

    private void RefreshDetail()
    {
        if (selectedProduct == null)
            return;

        string id = selectedProduct.productId;

        Sprite targetSprite = buyButtonSprite;
        bool canClick = true;

        string ownedMessage = "미보유";

        if (selectedProduct.category == ShopCategory.Character)
        {
            bool unlocked =
                MetaProgressSystem.IsCharacterUnlocked(id);

            bool selected =
                MetaProgressSystem.GetSelectedCharacterId() == id;

            if (selected)
            {
                targetSprite = characterEquippedSprite;
                ownedMessage = "착용 중 (영구 보유)";
                canClick = false;
            }
            else if (unlocked)
            {
                targetSprite = characterEquipSprite;
                ownedMessage = "보유 중";
            }
        }
        else if (selectedProduct.category == ShopCategory.Consumable)
        {
            int count =
                MetaProgressSystem.GetConsumableCount(id);

            bool selected =
                MetaProgressSystem.IsConsumableSelected(id);

            ownedMessage = $"보유: {count}개";

            if (selected)
            {
                targetSprite = consumableUsingSprite;
                ownedMessage += " / 사용 중";
            }
            else if (count > 0)
            {
                targetSprite = consumableUseSprite;
            }
        }
        else
        {
            canClick = false;
            ownedMessage = "준비 중";
        }

        // 이미지 변경
        if (buyButtonImage != null && targetSprite != null)
        {
            buyButtonImage.sprite = targetSprite;
        }

        // 캐릭터 착용 중에는 클릭 불가능
        // 소모품 사용 중에는 다시 눌러 해제 가능
        if (buyButton != null)
        {
            buyButton.interactable = canClick;
        }

        if (detailOwnedText != null)
        {
            detailOwnedText.text = ownedMessage;
        }
    }

    private void RefreshCoin()
    {
        if (coinText != null)
        {
            coinText.text =
                $"Coin : {MetaProgressSystem.GetCoin()}";
        }
    }

    // =========================
    // 돌아가기
    // =========================

    public void OnClickBack()
    {
        SceneTransitionManager.Instance.LoadScene("MainScene");
    }
}
