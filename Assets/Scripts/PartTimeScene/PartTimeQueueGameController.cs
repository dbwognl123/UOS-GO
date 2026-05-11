using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartTimeQueueGameController : MonoBehaviour
{
    [SerializeField] private AudioClip failClip;
    [SerializeField] private float failLockDuration = 1f;

    private bool isInputLocked = false;

    [Header("Game")]
    [SerializeField] private float totalGameTime = 10f;
    [SerializeField] private int visibleTileCount = 7;
    [SerializeField] private int moneyPerSuccess = 1;

    [Header("UI")]
    [SerializeField] private TMP_Text infoText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private CanvasGroup fadeOverlay;
    [SerializeField] private PartTimeTileSlot[] slots;

    [Header("Sprites")]
    [SerializeField] private Sprite spriteW;
    [SerializeField] private Sprite spriteA;
    [SerializeField] private Sprite spriteS;
    [SerializeField] private Sprite spriteD;

    [Header("Audio")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip clipW;
    [SerializeField] private AudioClip clipA;
    [SerializeField] private AudioClip clipS;
    [SerializeField] private AudioClip clipD;

    [Header("Finish")]
    [SerializeField] private float resultShowTime = 1.2f;
    [SerializeField] private float fadeDuration = 0.5f;

    private readonly List<PartTimeTileType> tileQueue = new List<PartTimeTileType>();

    private float remainingTime;
    private bool isFinished = false;

    private int successCount = 0;
    private int failCount = 0;

    private void Start()
    {
        remainingTime = totalGameTime;

        if (infoText != null)
            infoText.text = "맨 왼쪽 타일과 같은 키를 빠르게 누르세요";

        if (resultText != null)
            resultText.text = string.Empty;

        if (fadeOverlay != null)
            fadeOverlay.alpha = 0f;

        InitializeQueue();
        RefreshUI();
    }

    private void Update()
    {
        if (isFinished || isInputLocked) return;

        remainingTime -= Time.deltaTime;

        if (timerText != null)
            timerText.text = $"남은 시간 : {Mathf.CeilToInt(remainingTime)}";

        if (remainingTime <= 0f)
        {
            StartCoroutine(FinishRoutine());
            return;
        }

        if (Input.GetKeyDown(KeyCode.W))
            HandleInput(PartTimeTileType.W);

        if (Input.GetKeyDown(KeyCode.A))
            HandleInput(PartTimeTileType.A);

        if (Input.GetKeyDown(KeyCode.S))
            HandleInput(PartTimeTileType.S);

        if (Input.GetKeyDown(KeyCode.D))
            HandleInput(PartTimeTileType.D);
    }

    private void InitializeQueue()
    {
        tileQueue.Clear();

        for (int i = 0; i < visibleTileCount; i++)
        {
            tileQueue.Add(GetRandomTileType());
        }
    }

    private void HandleInput(PartTimeTileType inputType)
    {
        if (tileQueue.Count == 0) return;
        if (isInputLocked) return;

        PartTimeTileType leftmost = tileQueue[0];

        if (inputType == leftmost)
        {
            successCount++;
            PlayClip(inputType);
            StartCoroutine(PopAndShiftRoutine());
        }
        else
        {
            failCount++;
            StartCoroutine(FailRoutine());
        }
    }

    private IEnumerator FailRoutine()
    {
        isInputLocked = true;

        if (sfxSource != null && failClip != null)
            sfxSource.PlayOneShot(failClip);

        yield return new WaitForSeconds(failLockDuration);

        isInputLocked = false;
    }
    private IEnumerator PopAndShiftRoutine()
    {
        isInputLocked = true;

        if (slots != null && slots.Length > 0)
            slots[0].Clear();

        yield return new WaitForSeconds(0.05f);

        tileQueue.RemoveAt(0);
        tileQueue.Add(GetRandomTileType());
        RefreshUI();

        isInputLocked = false;
    }

    private void RefreshUI()
    {
        if (slots == null) return;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null) continue;

            if (i < tileQueue.Count)
            {
                PartTimeTileType type = tileQueue[i];
                slots[i].SetTile(type, GetSprite(type));
            }
            else
            {
                slots[i].Clear();
            }
        }
    }

    private PartTimeTileType GetRandomTileType()
    {
        return (PartTimeTileType)Random.Range(0, 4);
    }

    private Sprite GetSprite(PartTimeTileType type)
    {
        switch (type)
        {
            case PartTimeTileType.W: return spriteW;
            case PartTimeTileType.A: return spriteA;
            case PartTimeTileType.S: return spriteS;
            case PartTimeTileType.D: return spriteD;
        }

        return null;
    }

    private AudioClip GetClip(PartTimeTileType type)
    {
        switch (type)
        {
            case PartTimeTileType.W: return clipW;
            case PartTimeTileType.A: return clipA;
            case PartTimeTileType.S: return clipS;
            case PartTimeTileType.D: return clipD;
        }

        return null;
    }

    private void PlayClip(PartTimeTileType type)
    {
        if (sfxSource == null) return;

        AudioClip clip = GetClip(type);
        if (clip != null)
            sfxSource.PlayOneShot(clip);
    }

    private IEnumerator FinishRoutine()
    {
        if (remainingTime > -9999f)
        {
            // 종료 진입 표시
        }

        isFinished = true;

        int moneyGain = successCount * moneyPerSuccess;

        if (resultText != null)
        {
            resultText.text =
                $"알바 종료!\n" +
                $"성공: {successCount}\n" +
                $"실패: {failCount}\n" +
                $"알바비 +{moneyGain}";
        }

        yield return new WaitForSeconds(resultShowTime);
        yield return StartCoroutine(FadeToBlack());

        if (GameManager.Instance != null)
            GameManager.Instance.FinishPartTimeQTEMinigame(moneyGain);
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