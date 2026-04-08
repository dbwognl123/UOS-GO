using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClassDodgeGameController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform arenaCenter;
    [SerializeField] private Transform player;
    [SerializeField] private DodgeProjectile projectilePrefab;
    [SerializeField] private TMP_Text timerText;

    [Header("Arena")]
    [SerializeField] private float spawnRadius = 4.2f;

    [Header("Projectile Settings")]
    [SerializeField] private float projectileSpeed = 4.5f;
    [SerializeField] private float spawnInterval = 1.0f;
    [SerializeField] private int minProjectilesPerWave = 1;
    [SerializeField] private int maxProjectilesPerWave = 5;

    [Header("Game Time")]
    [SerializeField] private float surviveTime = 20f;

    private float remainingTime;
    private int projectilesPerWave;
    private bool isFinished = false;

    private void Start()
    {
        remainingTime = surviveTime;
        SetupDifficultyFromPlayerStats();
        StartCoroutine(SpawnRoutine());
    }

    private void Update()
    {
        if (isFinished) return;

        remainingTime -= Time.deltaTime;

        if (timerText != null)
            timerText.text = Mathf.CeilToInt(remainingTime).ToString();

        if (remainingTime <= 0f)
        {
            Success();
        }
    }

    private void SetupDifficultyFromPlayerStats()
    {
        int intelligence = 0;

        if (GameManager.Instance != null && GameManager.Instance.CurrentPlayer != null)
            intelligence = GameManager.Instance.CurrentPlayer.intelligence;

        float intelligence01 = Mathf.Clamp01(intelligence / 20f);

        // 지능 높을수록 구조물 수 감소
        projectilesPerWave = Mathf.RoundToInt(
            Mathf.Lerp(maxProjectilesPerWave, minProjectilesPerWave, intelligence01)
        );
    }

    private IEnumerator SpawnRoutine()
    {
        while (!isFinished)
        {
            SpawnWave();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnWave()
    {
        if (arenaCenter == null || player == null || projectilePrefab == null) return;

        for (int i = 0; i < projectilesPerWave; i++)
        {
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            Vector2 dirFromCenter = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            Vector2 spawnPos = (Vector2)arenaCenter.position + dirFromCenter * spawnRadius;

            Vector2 targetDir = ((Vector2)player.position - spawnPos).normalized;

            DodgeProjectile projectile = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
            projectile.Init(targetDir, projectileSpeed, this);
        }
    }

    public void OnPlayerHit()
    {
        if (isFinished) return;
        Fail();
    }

    private void Success()
    {
        if (isFinished) return;
        isFinished = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddIntelligence(1);
            GameManager.Instance.AddGrade(1);

            // 성공해도 현재 수업 소모
            GameManager.Instance.FinishCurrentClass();
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene("SchoolScene");
    }

    private void Fail()
    {
        if (isFinished) return;
        isFinished = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddHealth(-1);

            // 실패해도 현재 수업 소모
            GameManager.Instance.FinishCurrentClass();
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene("SchoolScene");
    }
}