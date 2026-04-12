
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum SchoolEntryType
{
    None,
    FrontGate,
    SideGate,
    BackGate
}

public enum PlayerStatType
{
    Health,
    Intelligence,
    Money,
    Appearance,
    CampusLife,
    Grade,
    Happiness,
    Girlfriend
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int CurrentWeek { get; private set; } = 1;
    public SchoolEntryType CurrentSchoolEntry { get; private set; } = SchoolEntryType.None;
    public PlayerRunData CurrentPlayer { get; private set; }
    public bool StudiedToday { get; private set; } = false;
    public bool WorkedToday { get; private set; } = false;
    private const int MAX_WEEK = 16;

    private readonly int[] allClassrooms =
    {
        01, 03, 04, 05, 06, 07,
        08, 10, 11, 14, 15,
        16, 18, 19, 20, 27,
        29 ,33, 35, 39
    };


    private List<int> todaySchedule = new List<int>();
    private int currentClassIndex = 0;

    public IReadOnlyList<int> TodaySchedule => todaySchedule;
    public int CurrentClassIndex => currentClassIndex;
    public bool IsAllClassesFinished => currentClassIndex >= todaySchedule.Count;

    public int CurrentEnteredClassroom { get; private set; } = -1;

    // 추가
    public Vector3 SavedSchoolPlayerPosition { get; private set; }
    public bool HasSavedSchoolPlayerPosition { get; private set; } = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public event Action<PlayerStatType, int> OnPlayerStatChanged;
    public event Action OnPlayerStatsRefreshed;
   
    public void StartNewGame()
    {
        CurrentWeek = 1;
        CurrentSchoolEntry = SchoolEntryType.None;
        StudiedToday = false;
        WorkedToday = false;

        CurrentPlayer = new PlayerRunData
        {
            happiness = 5,
            campusLife = 0,
            intelligence = 10,
            maxHealth = 100,
            currentHealth = 100,
            money = 5,
            hasGirlfriend = false,
            appearance = 5,
            grade = 0
        };

        GenerateTodaySchedule();
        ClearSavedSchoolPlayerPosition();
        OnPlayerStatsRefreshed?.Invoke();
        SceneManager.LoadScene("MorningScene");
    }
    public bool StudyInEvening()
    {
        if (CurrentPlayer == null) return false;
        if (StudiedToday) return false;

        AddMaxHealth(-5);
        AddIntelligence(5);
        StudiedToday = true;

        return true;
    }


    public bool WorkPartTimeInEvening()
    {
        if (CurrentPlayer == null) return false;
        if (WorkedToday) return false;

        AddMaxHealth(-3);
        AddMoney(3);
        WorkedToday = true;

        return true;
    }
    public void AddHappiness(int value)
    {
        if (CurrentPlayer == null) return;

        CurrentPlayer.happiness = Mathf.Clamp(CurrentPlayer.happiness + value, 0, 100);
        OnPlayerStatChanged?.Invoke(PlayerStatType.Happiness, value);
        OnPlayerStatsRefreshed?.Invoke();
    }

    public void AddGrade(int value)
    {
        if (CurrentPlayer == null) return;

        CurrentPlayer.grade += value;
        OnPlayerStatChanged?.Invoke(PlayerStatType.Grade, value);
        OnPlayerStatsRefreshed?.Invoke();
    }
    public void AddCampusLife(int value)
    {
        if (CurrentPlayer == null) return;

        CurrentPlayer.campusLife += value;
        OnPlayerStatChanged?.Invoke(PlayerStatType.CampusLife, value);
        OnPlayerStatsRefreshed?.Invoke();
    }

    public void AddIntelligence(int value)
    {
        if (CurrentPlayer == null) return;

        CurrentPlayer.intelligence = Mathf.Clamp(CurrentPlayer.intelligence + value, 0, 200);
        OnPlayerStatChanged?.Invoke(PlayerStatType.Intelligence, value);
        OnPlayerStatsRefreshed?.Invoke();
    }

    public void AddMaxHealth(int value)
    {
        if (CurrentPlayer == null) return;

        CurrentPlayer.maxHealth = Mathf.Clamp(CurrentPlayer.maxHealth + value, 1, 100);

        if (CurrentPlayer.currentHealth > CurrentPlayer.maxHealth)
            CurrentPlayer.currentHealth = CurrentPlayer.maxHealth;

        OnPlayerStatChanged?.Invoke(PlayerStatType.Health, value);
        OnPlayerStatsRefreshed?.Invoke();
    }

    public void AddCurrentHealth(int value)
    {
        if (CurrentPlayer == null) return;

        CurrentPlayer.currentHealth = Mathf.Clamp(
            CurrentPlayer.currentHealth + value,
            0,
            CurrentPlayer.maxHealth
        );

        OnPlayerStatChanged?.Invoke(PlayerStatType.Health, value);
        OnPlayerStatsRefreshed?.Invoke();
    }

    public void AddMoney(int value)
    {
        if (CurrentPlayer == null) return;

        CurrentPlayer.money = Mathf.Max(0, CurrentPlayer.money + value);
        OnPlayerStatChanged?.Invoke(PlayerStatType.Money, value);
        OnPlayerStatsRefreshed?.Invoke();
    }

    public void AddAppearance(int value)
    {
        if (CurrentPlayer == null) return;

        CurrentPlayer.appearance = Mathf.Clamp(CurrentPlayer.appearance + value, 0, 10);
        OnPlayerStatChanged?.Invoke(PlayerStatType.Appearance, value);
        OnPlayerStatsRefreshed?.Invoke();
    }

    public void SetGirlfriend(bool value)
    {
        if (CurrentPlayer == null) return;

        bool before = CurrentPlayer.hasGirlfriend;
        CurrentPlayer.hasGirlfriend = value;

        if (before != value)
        {
            OnPlayerStatChanged?.Invoke(PlayerStatType.Girlfriend, value ? 1 : -1);
            OnPlayerStatsRefreshed?.Invoke();
        }
    }
    public void EnterSchool(SchoolEntryType entryType)
    {
        CurrentSchoolEntry = entryType;
        ClearSavedSchoolPlayerPosition();
        SceneManager.LoadScene("SchoolScene");
    }


    public void GenerateTodaySchedule()
    {
        List<int> pool = new List<int>(allClassrooms);
        todaySchedule.Clear();

        for (int i = 0; i < 5; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, pool.Count);
            int classroomNumber = pool[randomIndex];

            todaySchedule.Add(classroomNumber);
            pool.RemoveAt(randomIndex);
        }

        currentClassIndex = 0;
        CurrentEnteredClassroom = -1;
        ClearSavedSchoolPlayerPosition();
    }

    public int GetCurrentTargetClassroom()
    {
        if (IsAllClassesFinished)
            return -1;

        return todaySchedule[currentClassIndex];
    }

    public void FinishCurrentClass()
    {
        if (IsAllClassesFinished) return;
        if (CurrentEnteredClassroom == -1) return;

        currentClassIndex++;
        CurrentEnteredClassroom = -1;
    }

    public bool CanEnterClassroom(int classroomNumber)
    {
        if (IsAllClassesFinished)
            return false;

        return classroomNumber == GetCurrentTargetClassroom();
    }

    public void SaveSchoolPlayerPosition(Vector3 position)
    {
        SavedSchoolPlayerPosition = position;
        HasSavedSchoolPlayerPosition = true;
    }

    public void ClearSavedSchoolPlayerPosition()
    {
        HasSavedSchoolPlayerPosition = false;
        SavedSchoolPlayerPosition = Vector3.zero;
    }

    public void EnterClassScene(int classroomNumber, Vector3 schoolPlayerPosition)
    {
        if (!CanEnterClassroom(classroomNumber))
            return;

        CurrentEnteredClassroom = classroomNumber;
        SaveSchoolPlayerPosition(schoolPlayerPosition);
        SceneManager.LoadScene("ClassScene");
    }

    public void CompleteCurrentClass()
    {
        if (IsAllClassesFinished)
            return;

        if (CurrentEnteredClassroom != GetCurrentTargetClassroom())
            return;

        currentClassIndex++;
        CurrentEnteredClassroom = -1;
    }
    public void GoToShop()
    {
        SceneManager.LoadScene("ShopScene");
    }

    public void GoToCollection()
    {
        SceneManager.LoadScene("CollectionScene");
    }
    public void ReturnToMain()
    {
        SceneManager.LoadScene("MainScene");
    }
    public void SkipSchool()
    {
        SceneManager.LoadScene("EveningScene");
    }

    public void GoToEvening()
    {
        SceneManager.LoadScene("EveningScene");
    }

    public void SleepInEvening()
    {
        if (CurrentPlayer == null) return;

        EndDay();
    }
    public void EndDay()
    {
        CurrentWeek++;

        if (CurrentWeek > MAX_WEEK)
        {
            SceneManager.LoadScene("EndingScene");
        }
        else
        {
            StudiedToday = false;
            WorkedToday = false;

            // 다음날 시작할 때 현재체력은 최대체력으로 리셋
            if (CurrentPlayer != null)
                CurrentPlayer.currentHealth = CurrentPlayer.maxHealth;

            GenerateTodaySchedule();
            OnPlayerStatsRefreshed?.Invoke();
            SceneManager.LoadScene("MorningScene");
        }
    }
}