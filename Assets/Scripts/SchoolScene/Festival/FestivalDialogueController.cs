using UnityEngine;

public class FestivalDialogueController : MonoBehaviour
{
    public static FestivalDialogueController Instance { get; private set; }

    [Header("Female NPC")]
    [SerializeField] private string npcName = "여자";
    [SerializeField] private Sprite npcPortrait;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowStartDialogue()
    {
        GameManager gm = GameManager.Instance;

        if (gm == null)
            return;

        string boothName =
            gm.GetFestivalBoothName(
                gm.targetFestivalBoothType
            );

        Show(
            $"왔어? 우리 과 {boothName} 하는데 놀러갈래?"
        );
    }
    public void ShowFoodRequestDialogue()
    {
        GameManager gm = GameManager.Instance;

        if (gm == null)
            return;

        string foodName =
            gm.GetFestivalFoodName(
                gm.targetFestivalFoodType
            );

        Show(
            $"아 여기네 ㅋㅋ " +
            $"{foodName} 땡기는데 푸드트럭 가볼까?"

        );
    }
    public void ShowWrongFoodDialogue(
        FestivalFoodType selectedFood)
    {
        GameManager gm = GameManager.Instance;

        if (gm == null)
            return;

        string foodName =
            gm.GetFestivalFoodName(selectedFood);

        Show(
            $"흠.. {foodName}는 별로 안 땡기는데"
        );
    }

    public void ShowCorrectFoodDialogue(
        FestivalFoodType selectedFood)
    {
        GameManager gm = GameManager.Instance;

        if (gm == null)
            return;

        string foodName =
            gm.GetFestivalFoodName(selectedFood);

        Show(
            $"여기 {foodName} 맛있다 ㅎㅎ 공연 보러가자!"
        );
    }

    private void Show(
    string line,
    System.Action onClosed = null)
    {
        if (SchoolNPCUI.Instance == null)
            return;

        SchoolNPCUI.Instance.OpenSimpleDialogue(
            npcName,
            npcPortrait,
            line,
            onClosed
        );
    }
    public void ShowBoothProgressDialogue(
    int visitedCount,
    System.Action onClosed = null)
    {
        GameManager gm = GameManager.Instance;

        if (gm == null)
            return;

        string boothName =
            gm.GetFestivalBoothName(
                gm.targetFestivalBoothType
            );

        Show(
            $"{visitedCount}/3 {boothName}",
            onClosed
        );
    }
    public void ShowFirstWrongBoothDialogue()
    {
        Show(
            "어? 여기는 우리 과가 하는 데가 아닌 것 같아."
        );
    }

    public void ShowSecondWrongBoothDialogue()
    {
        Show(
            "어? 여기도 아닌 것 같은데."
        );
    }
    public void ShowLowHealthEndDialogue(
        System.Action onClosed = null)
    {
        Show(
            "시간이 벌써 이렇게 됐네 아쉽다",
            onClosed
        );
    }

    public void ShowConcertSuccessDialogue(
        System.Action onClosed = null)
    {
        Show(
            "오늘 너무 재밌었어 또 보자 ㅎㅎ",
            onClosed
        );
    }
}