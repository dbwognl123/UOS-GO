using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MeetingPairController : MonoBehaviour
{
    [Header("Nodes")]
    [SerializeField] private MeetingPairNode[] femaleNodes;
    [SerializeField] private MeetingPairNode[] maleNodes;

    [Header("UI")]
    [SerializeField] private TMP_Text infoText;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private Button submitButton;

    [Header("Pair Colors")]
    [SerializeField] private Color firstPairColor = new Color(1f, 0.2f, 0.2f, 1f);   // 빨강
    [SerializeField] private Color secondPairColor = new Color(1f, 0.4f, 0.7f, 1f);  // 핑크
    [SerializeField] private Color thirdPairColor = new Color(0.6f, 0.3f, 1f, 1f);   // 보라

    [Header("Answer Arrow UI")]
    [SerializeField] private RectTransform answerArrowRoot;
    [SerializeField] private Image answerArrowPrefab;

    private readonly List<Image> spawnedAnswerArrows = new List<Image>();

    private class PairData
    {
        public int femaleIndex;
        public int maleIndex;
        public int colorIndex;
    }

    private readonly List<PairData> pairs = new List<PairData>();

    private MeetingPairNode pendingNode = null;
    private int pendingColorIndex = -1;
    private bool submitted = false;

    // 정답: 여자 i의 정답 남자 index
    private int[] correctMaleForFemale;

    private Color[] PairColors => new Color[]
    {
        firstPairColor,
        secondPairColor,
        thirdPairColor
    };

    private void Start()
    {
        SetupNodes();
        GenerateRandomAnswer();
        RefreshUI();
    }

    private void SetupNodes()
    {
        foreach (var node in femaleNodes)
        {
            node.OnClicked += OnNodeClicked;
            node.Initialize(Color.black);
        }

        foreach (var node in maleNodes)
        {
            node.OnClicked += OnNodeClicked;
            node.Initialize(Color.black);
        }

        if (resultText != null)
            resultText.text = "";

        if (infoText != null)
            infoText.text = "남자/여자를 눌러 3쌍을 만드세요.";
    }

    private void GenerateRandomAnswer()
    {
        correctMaleForFemale = new int[femaleNodes.Length];

        List<int> pool = new List<int>();
        for (int i = 0; i < maleNodes.Length; i++)
            pool.Add(i);

        for (int femaleIndex = 0; femaleIndex < femaleNodes.Length; femaleIndex++)
        {
            int rand = Random.Range(0, pool.Count);
            correctMaleForFemale[femaleIndex] = pool[rand];
            pool.RemoveAt(rand);
        }
    }

    private void OnNodeClicked(MeetingPairNode node)
    {
        if (submitted)
            return;

        // 이미 연결된 노드를 누르면 그 쌍 전체 해제
        if (TryRemoveExistingPair(node))
        {
            ClearPending();
            RefreshUI();
            return;
        }

        // 첫 클릭
        if (pendingNode == null)
        {
            pendingColorIndex = GetNextAvailableColorIndex();
            if (pendingColorIndex < 0)
            {
                if (infoText != null)
                    infoText.text = "이미 3쌍이 모두 연결되었습니다.";
                return;
            }

            pendingNode = node;
            pendingNode.SetPlayerHeartColor(PairColors[pendingColorIndex]);

            if (infoText != null)
                infoText.text = $"{GetGenderText(node.Gender)} 선택됨. 반대 성별을 눌러 짝을 만드세요.";
            return;
        }

        // 같은 노드 다시 누르면 취소
        if (node == pendingNode)
        {
            pendingNode.SetPlayerHeartBlack();
            ClearPending();

            if (infoText != null)
                infoText.text = "선택이 취소되었습니다.";
            return;
        }

        // 같은 성별 연속 클릭 → 현재 선택 취소, 둘 다 검정 유지
        if (node.Gender == pendingNode.Gender)
        {
            pendingNode.SetPlayerHeartBlack();

            // 새로 누른 같은 성별 노드도 혹시 색이 있다면 검정으로
            node.SetPlayerHeartBlack();

            ClearPending();

            if (infoText != null)
                infoText.text = "같은 성별끼리는 연결할 수 없습니다.";
            return;
        }

        // 반대 성별이면 쌍 확정
        CreatePair(pendingNode, node, pendingColorIndex);

        ClearPending();
        RefreshUI();

        if (infoText != null)
            infoText.text = "짝이 연결되었습니다.";
    }

    private void CreatePair(MeetingPairNode a, MeetingPairNode b, int colorIndex)
    {
        MeetingPairNode female = a.Gender == MeetingNodeGender.Female ? a : b;
        MeetingPairNode male = a.Gender == MeetingNodeGender.Male ? a : b;

        Color color = PairColors[colorIndex];

        female.SetPlayerHeartColor(color);
        male.SetPlayerHeartColor(color);

        pairs.Add(new PairData
        {
            femaleIndex = female.Index,
            maleIndex = male.Index,
            colorIndex = colorIndex
        });
    }

    private bool TryRemoveExistingPair(MeetingPairNode node)
    {
        for (int i = 0; i < pairs.Count; i++)
        {
            bool matched =
                (node.Gender == MeetingNodeGender.Female && pairs[i].femaleIndex == node.Index) ||
                (node.Gender == MeetingNodeGender.Male && pairs[i].maleIndex == node.Index);

            if (matched)
            {
                int femaleIndex = pairs[i].femaleIndex;
                int maleIndex = pairs[i].maleIndex;

                femaleNodes[femaleIndex].SetPlayerHeartBlack();
                maleNodes[maleIndex].SetPlayerHeartBlack();

                pairs.RemoveAt(i);

                if (infoText != null)
                    infoText.text = "짝이 해제되었습니다.";

                return true;
            }
        }

        return false;
    }

    private int GetNextAvailableColorIndex()
    {
        bool[] used = new bool[3];

        foreach (var pair in pairs)
        {
            if (pair.colorIndex >= 0 && pair.colorIndex < used.Length)
                used[pair.colorIndex] = true;
        }

        for (int i = 0; i < used.Length; i++)
        {
            if (!used[i])
                return i;
        }

        return -1;
    }

    private void ClearPending()
    {
        pendingNode = null;
        pendingColorIndex = -1;
    }

    private void RefreshUI()
    {
        if (submitButton != null)
            submitButton.interactable = pairs.Count == 3;
    }

    public void OnClickSubmit()
    {
        if (submitted)
            return;

        if (pairs.Count != 3)
        {
            if (infoText != null)
                infoText.text = "남녀 3쌍을 모두 연결해야 제출할 수 있습니다.";
            return;
        }

        submitted = true;

        int correctCount = 0;

        foreach (var pair in pairs)
        {
            if (correctMaleForFemale[pair.femaleIndex] == pair.maleIndex)
                correctCount++;
        }

        RevealCorrectAnswer();

        if (resultText != null)
        {
            resultText.text = correctCount == 3
                ? "정답 3/3\n여자NPC1 해금!"
                : $"정답 {correctCount}/3\n정답 하트를 확인하세요.";
        }

        StartCoroutine(FinishMeetingRoutine(correctCount));
    }

    private System.Collections.IEnumerator FinishMeetingRoutine(int correctCount)
    {
        yield return new WaitForSeconds(2f);

        if (GameManager.Instance != null)
            GameManager.Instance.ApplyMeetingSceneResult(correctCount);
    }
    private void RevealCorrectAnswer()
    {
        for (int femaleIndex = 0; femaleIndex < femaleNodes.Length; femaleIndex++)
        {
            Color answerColor = PairColors[femaleIndex];
            int correctMaleIndex = correctMaleForFemale[femaleIndex];

            femaleNodes[femaleIndex].ShowAnswerHeart(answerColor);
            maleNodes[correctMaleIndex].ShowAnswerHeart(answerColor);
        }

        ShowAnswerArrows();
    }


    private void ShowAnswerArrows()
    {
        ClearAnswerArrows();

        for (int femaleIndex = 0; femaleIndex < femaleNodes.Length; femaleIndex++)
        {
            int maleIndex = correctMaleForFemale[femaleIndex];

            RectTransform from = femaleNodes[femaleIndex].AnswerHeartRect;
            RectTransform to = maleNodes[maleIndex].AnswerHeartRect;

            if (from == null || to == null)
                continue;

            Color arrowColor = femaleNodes[femaleIndex].AnswerHeartImage.color;
            CreateArrow(from, to, arrowColor);
        }
    }

    private void CreateArrow(RectTransform from, RectTransform to, Color color)
    {
        if (answerArrowPrefab == null || answerArrowRoot == null)
            return;

        Image arrow = Instantiate(answerArrowPrefab, answerArrowRoot);
        arrow.gameObject.SetActive(true);
        arrow.color = color;
        arrow.raycastTarget = false;

        RectTransform arrowRect = arrow.rectTransform;

        Vector2 startLocal = WorldToLocalPoint(answerArrowRoot, from.position);
        Vector2 endLocal = WorldToLocalPoint(answerArrowRoot, to.position);

        Vector2 dir = endLocal - startLocal;
        float length = dir.magnitude;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        arrowRect.anchorMin = new Vector2(0.5f, 0.5f);
        arrowRect.anchorMax = new Vector2(0.5f, 0.5f);
        arrowRect.pivot = new Vector2(0f, 0.5f);

        arrowRect.anchoredPosition = startLocal;
        arrowRect.localRotation = Quaternion.Euler(0f, 0f, angle);
        arrowRect.sizeDelta = new Vector2(length, 2f);

        spawnedAnswerArrows.Add(arrow);
    }

    private Vector2 WorldToLocalPoint(RectTransform root, Vector3 worldPos)
    {
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, worldPos);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            root,
            screenPoint,
            null,
            out Vector2 localPoint
        );

        return localPoint;
    }

    private void ClearAnswerArrows()
    {
        for (int i = 0; i < spawnedAnswerArrows.Count; i++)
        {
            if (spawnedAnswerArrows[i] != null)
                Destroy(spawnedAnswerArrows[i].gameObject);
        }

        spawnedAnswerArrows.Clear();
    }
    private string GetGenderText(MeetingNodeGender gender)
    {
        return gender == MeetingNodeGender.Female ? "여자" : "남자";
    }
}