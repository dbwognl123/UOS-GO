using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CollectionSceneController : MonoBehaviour
{
    [Header("Grid")]
    [SerializeField] private Transform gridRoot;
    [SerializeField] private CollectionSlotUI slotPrefab;

    [Header("Ending Data")]
    [SerializeField] private CollectionEndingData[] endings;

    [Header("Detail Panel")]
    [SerializeField] private GameObject detailPanel;
    [SerializeField] private Image detailImage;
    [SerializeField] private TMP_Text detailTitle;
    [SerializeField] private TMP_Text conditionText;

    private void Start()
    {
        BuildCollection();

        if (detailPanel != null)
            detailPanel.SetActive(false);
    }

    private void BuildCollection()
    {
        if (gridRoot == null ||
            slotPrefab == null)
        {
            return;
        }

        foreach (CollectionEndingData data in endings)
        {
            CollectionSlotUI slot =
                Instantiate(
                    slotPrefab,
                    gridRoot
                );

            slot.Setup(
                data,
                this
            );
        }
    }

    public void OpenDetail(
        CollectionEndingData data)
    {
        if (data == null)
            return;

        if (detailPanel != null)
            detailPanel.SetActive(true);

        if (detailImage != null)
            detailImage.sprite =
                data.detailImage != null
                    ? data.detailImage
                    : data.thumbnail;

        if (detailTitle != null)
            detailTitle.text =
                data.title;

        if (conditionText != null)
            conditionText.text =
                data.unlockCondition;
    }

    public void CloseDetail()
    {
        if (detailPanel != null)
            detailPanel.SetActive(false);
    }

    public void BackToMain()
    {
        SceneManager.LoadScene(
            "MainScene"
        );
    }
#if UNITY_EDITOR
    [ContextMenu("Debug Unlock GoodGrade")]
    private void DebugUnlockGoodGrade()
    {
        EndingCollectionSave.Unlock("A_GoodGrade");

        Debug.Log("[Collection Test] A_GoodGrade 해금");
    }
#endif
}