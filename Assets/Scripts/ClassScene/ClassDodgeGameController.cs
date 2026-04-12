using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClassDodgeGameController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform arenaCenter;
    [SerializeField] private DodgePlayerController player;
    [SerializeField] private DodgeProjectile projectilePrefab;
    [SerializeField] private TMP_Text timerText;

    [Header("Arena")]
    [SerializeField] private float arenaRadius = 3.5f;
    [SerializeField] private float spawnPadding = 0.1f;

    [Header("Applied Runtime Difficulty")]
    [SerializeField] private float projectileSpeed = 1f;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private int projectilesPerWave = 5;

    [Header("Debug")]
    [SerializeField] private int debugWeek;
    [SerializeField] private int debugIntelligence;
    [SerializeField] private int debugRequiredIntelligence;
    [SerializeField] private int debugGap;
    [SerializeField] private int debugDifficultyTier;

    [Header("Game Time")]
    [SerializeField] private float surviveTime = 20f;

    private float remainingTime;
    private bool isFinished = false;

    public Transform ArenaCenter => arenaCenter;
    public float ArenaRadius => arenaRadius;

    private void Start()
    {
        remainingTime = surviveTime;
        SetupDifficultyFromPlayerStats();

        if (player != null)
            player.SetupArena(arenaCenter, arenaRadius);

        StartCoroutine(SpawnRoutine());
    }

    private void Update()
    {
        if (isFinished) return;

        remainingTime -= Time.deltaTime;

        if (timerText != null)
            timerText.text = Mathf.CeilToInt(remainingTime).ToString();

        if (remainingTime <= 0f)
            Success();
    }

    private void SetupDifficultyFromPlayerStats()
    {
        int week = 1;
        int intelligence = 0;

        if (GameManager.Instance != null && GameManager.Instance.CurrentPlayer != null)
        {
            week = Mathf.Clamp(GameManager.Instance.CurrentWeek, 1, 16);
            intelligence = GameManager.Instance.CurrentPlayer.intelligence;
        }

        // 1주차  15  30 45  60  75  90 105 120 135 150 165 180 195 210 225
        // 플레   10  20 30 40 50 60 70 80 90 100 110 120 130 140 150
        int requiredIntelligence = week * 15;
        int gap = Mathf.Max(0, requiredIntelligence - intelligence);

        int difficultyTier;

        if (gap <= 10)
            difficultyTier = 1;
        else if (gap <= 25)
            difficultyTier = 2;
        else if (gap <= 45)
            difficultyTier = 3;
        else if (gap <= 60)
            difficultyTier = 4;
        else if (gap <= 85)
            difficultyTier = 5;
        else if (gap <= 120)
            difficultyTier = 6;
        else
            difficultyTier = 7;

        switch (difficultyTier)
        {
            case 1:
                projectilesPerWave = 5;
                spawnInterval = 2.0f;
                projectileSpeed = 1.0f;
                break;

            case 2:
                projectilesPerWave = 6;
                spawnInterval = 1.7f;
                projectileSpeed = 1.3f;
                break;

            case 3:
                projectilesPerWave = 7;
                spawnInterval = 1.4f;
                projectileSpeed = 1.6f;
                break;

            case 4:
                projectilesPerWave = 9;
                spawnInterval = 1.1f;
                projectileSpeed = 2.0f;
                break;

            case 5:
                projectilesPerWave = 10;
                spawnInterval = 0.9f;
                projectileSpeed = 2.3f;
                break;

            case 6:
                projectilesPerWave = 11;
                spawnInterval = 0.7f;
                projectileSpeed = 2.6f;
                break;

            default: // 7
                projectilesPerWave = 13;
                spawnInterval = 0.5f;
                projectileSpeed = 3.0f;
                break;
        }
    

        debugWeek = week;
        debugIntelligence = intelligence;
        debugRequiredIntelligence = requiredIntelligence;
        debugGap = gap;
        debugDifficultyTier = difficultyTier;

        Debug.Log(
            $"[ClassDifficulty] week={week}, intelligence={intelligence}, required={requiredIntelligence}, " +
            $"gap={gap}, tier={difficultyTier}, count={projectilesPerWave}, interval={spawnInterval}, speed={projectileSpeed}"
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

            Vector2 spawnPos = (Vector2)arenaCenter.position + dirFromCenter * (arenaRadius - spawnPadding);
            Vector2 targetDir = ((Vector2)player.transform.position - spawnPos).normalized;

            DodgeProjectile projectile = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
            projectile.Init(targetDir, projectileSpeed, this, arenaCenter, arenaRadius);
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
            GameManager.Instance.FinishCurrentClass();
        }

        SceneManager.LoadScene("SchoolScene");
    }

    private void Fail()
    {
        if (isFinished) return;
        isFinished = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.FinishCurrentClass();
        }

        SceneManager.LoadScene("SchoolScene");
    }

    private void OnDrawGizmosSelected()
    {
        if (arenaCenter == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(arenaCenter.position, arenaRadius);
    }
}