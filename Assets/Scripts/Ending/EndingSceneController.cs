using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndingSceneController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image sceneImage;
    [SerializeField] private TMP_Text sceneText;
    [SerializeField] private GameObject finishButtons;

    [Header("Fade")]
    [SerializeField] private CanvasGroup fadeOverlay;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float blackHoldTime = 0.15f;

    [Header("Slide Data")]
    [SerializeField] private EndingSlideData[] slideDatabase;

    private readonly List<EndingSlideData> runtimeSlides = new List<EndingSlideData>();

    private void Start()
    {
        if (finishButtons != null)
            finishButtons.SetActive(false);

        BuildRuntimeSlides();
        StartCoroutine(PlayEndingSequence());
    }

    private void BuildRuntimeSlides()
    {
        runtimeSlides.Clear();

        if (GameManager.Instance == null ||
            GameManager.Instance.currentEndingSequence == null)
        {
            return;
        }

        EndingSequenceResult result =
            GameManager.Instance.currentEndingSequence;

        // A : 성적
        AddSlide($"A_{result.sceneA}");

        // B : 학교생활
        AddSlide($"B_{result.sceneB}");

        // C : 여자친구
        AddSlide($"C_{result.sceneC}");

        // E : 히든 - 찐친
        if (result.sceneE != EndingSceneEType.None)
        {
            AddSlide($"E_{result.sceneE}");
        }

        // F : 히든 - 대학원
        if (result.sceneF != EndingSceneFType.None)
        {
            AddSlide($"F_{result.sceneF}");
        }

        // D : 행복
        // 항상 마지막에 재생
        AddSlide($"D_{result.sceneD}");
    }
    private void AddSlide(string key)
    {
        for (int i = 0; i < slideDatabase.Length; i++)
        {
            if (slideDatabase[i] != null && slideDatabase[i].key == key)
            {
                runtimeSlides.Add(slideDatabase[i]);
                return;
            }
        }

        Debug.LogWarning($"EndingSceneController: slide key not found -> {key}");
    }

    private IEnumerator PlayEndingSequence()
    {
        if (runtimeSlides.Count == 0)
        {
            if (finishButtons != null)
                finishButtons.SetActive(true);
            yield break;
        }

        if (fadeOverlay != null)
            fadeOverlay.alpha = 1f;

        for (int i = 0; i < runtimeSlides.Count; i++)
        {
            EndingSlideData slide = runtimeSlides[i];
            yield return StartCoroutine(PlaySlide(slide));

            if (i < runtimeSlides.Count - 1)
            {
                yield return StartCoroutine(Fade(0f, 1f));
                yield return new WaitForSeconds(blackHoldTime);
            }
        }

        yield return StartCoroutine(Fade(0f, 0.35f));

        if (finishButtons != null)
            finishButtons.SetActive(true);
    }

    private IEnumerator PlaySlide(EndingSlideData slide)
    {
        if (slide == null || sceneImage == null)
            yield break;

        if (sceneText != null)
            sceneText.text = slide.text;

        RectTransform rt = sceneImage.rectTransform;

        float elapsed = 0f;
        float swapTimer = 0f;
        bool showA = true;

        // 시작 직전 검은 화면에서 첫 이미지 세팅
        sceneImage.sprite = slide.imageA != null ? slide.imageA : slide.imageB;
        rt.anchoredPosition = slide.startAnchoredPos;
        rt.localScale = slide.startScale;

        yield return StartCoroutine(Fade(1f, 0f));

        while (elapsed < slide.totalDuration)
        {
            elapsed += Time.deltaTime;
            swapTimer += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / slide.totalDuration);

            // 천천히 이동 / 줌
            rt.anchoredPosition = Vector2.Lerp(slide.startAnchoredPos, slide.endAnchoredPos, t);
            rt.localScale = Vector3.Lerp(slide.startScale, slide.endScale, t);

            // ABAB 이미지 전환
            if (slide.imageA != null && slide.imageB != null && swapTimer >= slide.swapInterval)
            {
                swapTimer = 0f;
                showA = !showA;
                sceneImage.sprite = showA ? slide.imageA : slide.imageB;
            }

            yield return null;
        }

        // 마지막 프레임 정리
        rt.anchoredPosition = slide.endAnchoredPos;
        rt.localScale = slide.endScale;
    }

    private IEnumerator Fade(float from, float to)
    {
        if (fadeOverlay == null)
            yield break;

        float time = 0f;
        fadeOverlay.alpha = from;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / fadeDuration);
            fadeOverlay.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }

        fadeOverlay.alpha = to;
    }

    public void OnClickGoTitle()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void OnClickCollection()
    {
        SceneManager.LoadScene("CollectionScene");
    }
}