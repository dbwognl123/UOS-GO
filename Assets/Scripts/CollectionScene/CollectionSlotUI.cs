using UnityEngine;
using UnityEngine.UI;

public class CollectionSlotUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image endingImage;
    [SerializeField] private GameObject lockImage;
    [SerializeField] private GameObject newBadge;
    [SerializeField] private Button button;

    private CollectionEndingData data;
    private CollectionSceneController controller;

    public void Setup(
        CollectionEndingData endingData,
        CollectionSceneController sceneController)
    {
        data = endingData;
        controller = sceneController;

        Refresh();
    }

    public void Refresh()
    {
        if (data == null)
            return;

        bool unlocked =
            EndingCollectionSave.IsUnlocked(
                data.endingKey
            );

        if (endingImage != null)
        {
            endingImage.gameObject.SetActive(
                unlocked
            );

            if (unlocked)
                endingImage.sprite = data.thumbnail;
        }

        if (lockImage != null)
        {
            lockImage.SetActive(
                !unlocked
            );
        }

        if (newBadge != null)
        {
            newBadge.SetActive(
                unlocked &&
                EndingCollectionSave.IsNew(
                    data.endingKey
                )
            );
        }

        if (button != null)
        {
            button.interactable =
                unlocked;
        }
    }

    public void OnClick()
    {
        if (data == null)
            return;

        if (!EndingCollectionSave.IsUnlocked(
                data.endingKey))
        {
            return;
        }

        EndingCollectionSave.MarkAsSeen(
            data.endingKey
        );

        if (newBadge != null)
            newBadge.SetActive(false);

        controller?.OpenDetail(data);
    }
}