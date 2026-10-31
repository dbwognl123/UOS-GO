using UnityEngine;
using UnityEngine.UI;

public class ShopProductButtonUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image productImage;

    private MetaShopProductData data;
    private MetaShopController controller;

    public void Setup(
        MetaShopProductData productData,
        MetaShopController shopController)
    {
        data = productData;
        controller = shopController;

        if (productImage != null)
        {
            productImage.sprite =
                data.productImage;
        }
    }

    public void OnClick()
    {
        if (data == null)
            return;

        if (controller == null)
            return;

        controller.OpenDetail(data);
    }
}