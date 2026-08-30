using UnityEngine;

public class FestivalWeekController : MonoBehaviour
{
    [Header("Festival Objects")]
    [SerializeField] private GameObject festivalRoot;

    private void Start()
    {
        RefreshFestivalObjects();
    }

    private void RefreshFestivalObjects()
    {
        if (festivalRoot == null)
            return;

        bool isFestivalWeek =
            GameManager.Instance != null &&
            GameManager.Instance.CurrentWeek == 10;

        festivalRoot.SetActive(isFestivalWeek);

        Debug.Log(
            $"[Festival] Week=" +
            $"{GameManager.Instance?.CurrentWeek}, " +
            $"FestivalObjects={isFestivalWeek}"
        );
    }
}