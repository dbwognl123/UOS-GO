using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EveningSceneUI : MonoBehaviour
{
    [Header("Week Transition")]
    [SerializeField] private TMP_Text weekTransitionText;
    [SerializeField] private float weekTextShowTime = 1.0f;

    [Header("Popup")]
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private TMP_Text popupText;

    [Header("Fade")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 0.4f;
    [SerializeField] private float blackScreenStayTime = 0.2f;

    private EveningInteractionType currentInteractionType = EveningInteractionType.None;
    private bool isBusy = false;

    private void Start()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);

        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
            fadeImage.gameObject.SetActive(false);
        }
        if (weekTransitionText != null)
            weekTransitionText.gameObject.SetActive(false);
    }

    public void OpenPopup(EveningInteractionType type)
    {
        if (isBusy) return;
        if (popupPanel == null || popupText == null) return;

        // 이미 오늘 한 행동이면 팝업 자체를 안 띄움
        if (IsInteractionBlocked(type))
        {
            currentInteractionType = EveningInteractionType.None;
            popupPanel.SetActive(false);
            return;
        }

        currentInteractionType = type;
        popupPanel.SetActive(true);

        switch (type)
        {
            case EveningInteractionType.Door:
                popupText.text = "알바하러 갈까?";
                break;

            case EveningInteractionType.Computer:
                popupText.text = "공부할까?";
                break;

            case EveningInteractionType.Bed:
                popupText.text = "자러 갈까?";
                break;

            default:
                popupText.text = "";
                break;
        }
    }

    public void ClosePopup()
    {
        if (isBusy) return;

        currentInteractionType = EveningInteractionType.None;

        if (popupPanel != null)
            popupPanel.SetActive(false);
    }

    public void OnClickYes()
    {
        if (GameManager.Instance == null || isBusy) return;

        if (IsInteractionBlocked(currentInteractionType))
        {
            ClosePopupForce();
            return;
        }

        switch (currentInteractionType)
        {
            case EveningInteractionType.Door:
                StartCoroutine(DoPartTimeRoutine());
                break;

            case EveningInteractionType.Computer:
                StartCoroutine(DoStudyRoutine());
                break;

            case EveningInteractionType.Bed:
                StartCoroutine(DoSleepRoutine());
                break;
        }
    }
    private IEnumerator DoSleepRoutine()
    {
        isBusy = true;
        ClosePopupForce();

        int nextWeek = GameManager.Instance.CurrentWeek + 1;

        yield return StartCoroutine(Fade(0f, 1f));

        if (weekTransitionText != null)
        {
            weekTransitionText.gameObject.SetActive(true);
            weekTransitionText.text = $"{nextWeek}주차";
        }

        yield return new WaitForSeconds(weekTextShowTime);

        if (weekTransitionText != null)
            weekTransitionText.gameObject.SetActive(false);

        GameManager.Instance.EndDay();
    }

    public void OnClickNo()
    {
        ClosePopup();
    }

    private IEnumerator DoPartTimeRoutine()
    {
        isBusy = true;
        ClosePopupForce();

        GameManager.Instance.WorkPartTimeInEvening();

        yield return StartCoroutine(Fade(0f, 1f));
        yield return new WaitForSeconds(blackScreenStayTime);
        yield return StartCoroutine(Fade(1f, 0f));

        isBusy = false;
    }

    private IEnumerator DoStudyRoutine()
    {
        isBusy = true;
        ClosePopupForce();

        GameManager.Instance.StudyInEvening();

        yield return StartCoroutine(Fade(0f, 1f));
        yield return new WaitForSeconds(blackScreenStayTime);
        yield return StartCoroutine(Fade(1f, 0f));

        isBusy = false;
    }

    private IEnumerator Fade(float from, float to)
    {
        if (fadeImage == null) yield break;

        fadeImage.gameObject.SetActive(true);

        float time = 0f;
        Color c = fadeImage.color;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / fadeDuration);
            c.a = Mathf.Lerp(from, to, t);
            fadeImage.color = c;
            yield return null;
        }

        c.a = to;
        fadeImage.color = c;

        if (Mathf.Approximately(to, 0f))
            fadeImage.gameObject.SetActive(false);
    }

    private void ClosePopupForce()
    {
        currentInteractionType = EveningInteractionType.None;

        if (popupPanel != null)
            popupPanel.SetActive(false);
    }
    private bool IsInteractionBlocked(EveningInteractionType type)
    {
        if (GameManager.Instance == null) return false;

        switch (type)
        {
            case EveningInteractionType.Computer:
                return GameManager.Instance.StudiedToday;

            case EveningInteractionType.Door:
                return GameManager.Instance.WorkedToday;

            default:
                return false;
        }
    }
}