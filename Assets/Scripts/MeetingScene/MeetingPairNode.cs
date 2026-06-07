using System;
using UnityEngine;
using UnityEngine.UI;

public enum MeetingNodeGender
{
    Female,
    Male
}

public class MeetingPairNode : MonoBehaviour
{
    [SerializeField] private MeetingNodeGender gender;
    [SerializeField] private int index;
    [SerializeField] private Button button;

    [Header("Heart Images")]
    [SerializeField] private Image playerHeartImage;
    [SerializeField] private Image answerHeartImage;

    public MeetingNodeGender Gender => gender;
    public int Index => index;

    public Image PlayerHeartImage => playerHeartImage;
    public Image AnswerHeartImage => answerHeartImage;
    public RectTransform AnswerHeartRect => answerHeartImage != null ? answerHeartImage.rectTransform : null;

    public Action<MeetingPairNode> OnClicked;

    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();

        button.onClick.AddListener(() => OnClicked?.Invoke(this));
    }

    public void Initialize(Color defaultPlayerColor)
    {
        SetPlayerHeartColor(defaultPlayerColor);
        HideAnswerHeart();
    }

    public void SetPlayerHeartColor(Color color)
    {
        if (playerHeartImage == null) return;

        playerHeartImage.enabled = true;
        playerHeartImage.color = color;
    }

    public void SetPlayerHeartBlack()
    {
        SetPlayerHeartColor(Color.black);
    }

    public void ShowAnswerHeart(Color color)
    {
        if (answerHeartImage == null) return;

        answerHeartImage.enabled = true;
        answerHeartImage.color = color;
    }

    public void HideAnswerHeart()
    {
        if (answerHeartImage == null) return;

        answerHeartImage.enabled = false;
    }
}