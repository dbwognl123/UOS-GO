using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StudyMemoryGameController : MonoBehaviour
{
    [Header("Board")]
    [SerializeField] private int gridSize = 10;
    [SerializeField] private Transform boardRoot;
    [SerializeField] private StudyGridCell cellPrefab;

    [Header("UI")]
    [SerializeField] private TMP_Text infoText;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private CanvasGroup fadeOverlay;

    [Header("Timing")]
    [SerializeField] private float memorizeDuration = 2f;
    [SerializeField] private float resultDuration = 1f;
    [SerializeField] private float fadeDuration = 0.5f;

    [Header("Colors")]
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color memorizedPathColor = Color.yellow;
    [SerializeField] private Color playerColor = Color.cyan;
    [SerializeField] private Color goalColor = Color.red;
    [SerializeField] private Color failColor = new Color(1f, 0.3f, 0.3f);
    [SerializeField] private Color successTrailColor = new Color(1f, 0.9f, 0.4f);

    private StudyGridCell[,] cells;
    private List<Vector2Int> path = new List<Vector2Int>();

    private Vector2Int startPos = new Vector2Int(0, 0);
    private Vector2Int goalPos = new Vector2Int(9, 9);
    private Vector2Int currentPos;

    // path 안에서 현재까지 맞게 밟은 인덱스
    // path[0]은 시작칸 (0,0)
    private int currentPathIndex = 0;

    private bool inputEnabled = false;
    private bool finished = false;

    private void Start()
    {
        BuildBoard();
        GenerateRandomPath();

        currentPos = startPos;
        currentPathIndex = 0;

        if (resultText != null)
            resultText.text = string.Empty;

        if (fadeOverlay != null)
            fadeOverlay.alpha = 0f;

        StartCoroutine(GameRoutine());
    }

    private void BuildBoard()
    {
        cells = new StudyGridCell[gridSize, gridSize];

        // UI GridLayoutGroup은 위에서 아래로 배치하니까
        // y를 큰 값부터 내려오면서 생성해서 (0,0)이 좌하단처럼 보이게 함
        for (int y = gridSize - 1; y >= 0; y--)
        {
            for (int x = 0; x < gridSize; x++)
            {
                StudyGridCell cell = Instantiate(cellPrefab, boardRoot);
                cell.Setup(new Vector2Int(x, y));
                cell.SetColor(defaultColor);
                cells[x, y] = cell;
            }
        }
    }

    private void GenerateRandomPath()
    {
        path.Clear();

        Vector2Int current = startPos;
        path.Add(current);

        // (0,0) -> (9,9) 까지
        // 오른쪽 9번 + 위로 9번 = 총 18번 이동
        List<Vector2Int> moves = new List<Vector2Int>();

        for (int i = 0; i < gridSize - 1; i++)
            moves.Add(Vector2Int.right);

        for (int i = 0; i < gridSize - 1; i++)
            moves.Add(Vector2Int.up);

        Shuffle(moves);

        for (int i = 0; i < moves.Count; i++)
        {
            current += moves[i];
            path.Add(current);
        }
    }

    private void Shuffle(List<Vector2Int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }

    private IEnumerator GameRoutine()
    {
        if (infoText != null)
            infoText.text = "길을 기억하세요";

        ShowMemorizeBoard();
        yield return new WaitForSeconds(memorizeDuration);

        if (infoText != null)
            infoText.text = "길을 따라 (9,9)까지 이동하세요";

        ShowPlayBoard();
        inputEnabled = true;
    }

    private void Update()
    {
        if (!inputEnabled || finished)
            return;

        Vector2Int move = Vector2Int.zero;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            move = Vector2Int.up;
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            move = Vector2Int.down;
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            move = Vector2Int.left;
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            move = Vector2Int.right;

        if (move != Vector2Int.zero)
            TryMove(move);
    }

    private void TryMove(Vector2Int move)
    {
        Vector2Int next = currentPos + move;

        if (!IsInside(next))
            return;

        int nextIndex = currentPathIndex + 1;

        // 정답 경로의 다음 칸이면 성공 이동
        if (nextIndex < path.Count && next == path[nextIndex])
        {
            currentPos = next;
            currentPathIndex = nextIndex;

            ShowPlayBoard();

            if (currentPos == goalPos)
            {
                StartCoroutine(FinishRoutine(true));
            }
        }
        else
        {
            currentPos = next;
            ShowFailBoard(next);
            StartCoroutine(FinishRoutine(false));
        }
    }

    private bool IsInside(Vector2Int p)
    {
        return p.x >= 0 && p.x < gridSize && p.y >= 0 && p.y < gridSize;
    }

    private void ShowMemorizeBoard()
    {
        ClearBoard();

        for (int i = 0; i < path.Count; i++)
        {
            Vector2Int p = path[i];
            cells[p.x, p.y].SetColor(memorizedPathColor);
        }

        // 시작점 / 목표점 강조
        cells[startPos.x, startPos.y].SetColor(playerColor);
        cells[goalPos.x, goalPos.y].SetColor(goalColor);
    }

    private void ShowPlayBoard()
    {
        ClearBoard();

        // 지금까지 맞게 밟은 칸 표시
        for (int i = 0; i <= currentPathIndex; i++)
        {
            Vector2Int p = path[i];
            cells[p.x, p.y].SetColor(successTrailColor);
        }

        // 현재 위치 / 목표 표시
        cells[currentPos.x, currentPos.y].SetColor(playerColor);
        cells[goalPos.x, goalPos.y].SetColor(goalColor);
    }

    private void ShowFailBoard(Vector2Int failedPos)
    {
        ShowPlayBoard();
        cells[failedPos.x, failedPos.y].SetColor(failColor);
    }

    private void ClearBoard()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                cells[x, y].SetColor(defaultColor);
            }
        }
    }

    private IEnumerator FinishRoutine(bool success)
    {
        if (finished) yield break;

        finished = true;
        inputEnabled = false;

        // 시작칸(path[0]) 제외하고 "실제로 맞게 밟은 칸 수"
        int correctStepCount = Mathf.Max(0, currentPathIndex);

        if (resultText != null)
        {
            resultText.text = success
                ? $"성공!\n맞게 밟은 블록 수: {correctStepCount}\n지능 +{correctStepCount}"
                : $"실패!\n맞게 밟은 블록 수: {correctStepCount}\n지능 +{correctStepCount}";
        }

        yield return new WaitForSeconds(resultDuration);
        yield return StartCoroutine(FadeToBlack());

        if (GameManager.Instance != null)
            GameManager.Instance.FinishStudyMinigame(correctStepCount);
    }

    private IEnumerator FadeToBlack()
    {
        if (fadeOverlay == null)
            yield break;

        float time = 0f;
        fadeOverlay.alpha = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / fadeDuration);
            fadeOverlay.alpha = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }

        fadeOverlay.alpha = 1f;
    }
}