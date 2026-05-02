
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



[Serializable]
public class NPCProgressEntry
{
    public SchoolNPCType npcType;
    public int highestSuccessStage;
}
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

    [Header("NPC Progress")]
    public int friendCurrentStage = 1;
    public int friendStageCap = 99;
    public bool friendRetired = false;
    public bool friendUsedToday = false;

    public int seniorCurrentStage = 1;
    public int seniorStageCap = 99;
    public bool seniorRetired = false;
    public bool seniorUsedToday = false;

    public int professorCurrentStage = 2; // 1단계 기본 + 질문 10회 달성 시 2단계 제안이 뜨게
    public int professorStageCap = 99;
    public bool professorRetired = false;
    public bool professorUsedToday = false;
    public int professorQuestionCount = 0;

    public int romanceCurrentStage = 1;
    public int romanceStageCap = 99;
    public bool romanceRetired = false;
    public bool romanceUsedToday = false;

    [Header("Ending Unlock Flags")]
    public bool endingACutUnlocked = false;
    public bool endingGraduateSchoolUnlocked = false;
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
        ResetDailyNPCUsage();
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
        CurrentPlayer.happiness = Mathf.Clamp(CurrentPlayer.happiness + value, 0, 999);
        OnPlayerStatsRefreshed?.Invoke();
    }
    public void AddProfessorQuestionCount(int value)
    {
        professorQuestionCount = Mathf.Max(0, professorQuestionCount + value);
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
        CurrentPlayer.campusLife = Mathf.Clamp(CurrentPlayer.campusLife + value, 0, 999);
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
        CurrentPlayer.appearance = Mathf.Clamp(CurrentPlayer.appearance + value, 0, 999);
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


    public void ResetDailyNPCUsage()
    {
        friendUsedToday = false;
        seniorUsedToday = false;
        professorUsedToday = false;
        romanceUsedToday = false;
    }

    public bool IsNPCTypeUsedToday(SchoolNPCType type)
    {
        switch (type)
        {
            case SchoolNPCType.Friend: return friendUsedToday;
            case SchoolNPCType.Senior: return seniorUsedToday;
            case SchoolNPCType.Professor: return professorUsedToday;
            case SchoolNPCType.Romance: return romanceUsedToday;
        }
        return false;
    }

    public void MarkNPCTypeUsedToday(SchoolNPCType type)
    {
        switch (type)
        {
            case SchoolNPCType.Friend: friendUsedToday = true; break;
            case SchoolNPCType.Senior: seniorUsedToday = true; break;
            case SchoolNPCType.Professor: professorUsedToday = true; break;
            case SchoolNPCType.Romance: romanceUsedToday = true; break;
        }
    }

    public int GetNPCCurrentStage(SchoolNPCType type)
    {
        switch (type)
        {
            case SchoolNPCType.Friend: return friendCurrentStage;
            case SchoolNPCType.Senior: return seniorCurrentStage;
            case SchoolNPCType.Professor: return professorCurrentStage;
            case SchoolNPCType.Romance: return romanceCurrentStage;
        }
        return 1;
    }

    public int GetNPCStageCap(SchoolNPCType type)
    {
        switch (type)
        {
            case SchoolNPCType.Friend: return friendStageCap;
            case SchoolNPCType.Senior: return seniorStageCap;
            case SchoolNPCType.Professor: return professorStageCap;
            case SchoolNPCType.Romance: return romanceStageCap;
        }
        return 99;
    }

    public void SetNPCStage(SchoolNPCType type, int stage)
    {
        stage = Mathf.Max(1, stage);

        switch (type)
        {
            case SchoolNPCType.Friend: friendCurrentStage = stage; break;
            case SchoolNPCType.Senior: seniorCurrentStage = stage; break;
            case SchoolNPCType.Professor: professorCurrentStage = stage; break;
            case SchoolNPCType.Romance: romanceCurrentStage = stage; break;
        }
    }

    public void SetNPCStageCap(SchoolNPCType type, int cap)
    {
        cap = Mathf.Max(1, cap);

        switch (type)
        {
            case SchoolNPCType.Friend: friendStageCap = cap; break;
            case SchoolNPCType.Senior: seniorStageCap = cap; break;
            case SchoolNPCType.Professor: professorStageCap = cap; break;
            case SchoolNPCType.Romance: romanceStageCap = cap; break;
        }
    }

    public bool IsNPCRetired(SchoolNPCType type)
    {
        switch (type)
        {
            case SchoolNPCType.Friend: return friendRetired;
            case SchoolNPCType.Senior: return seniorRetired;
            case SchoolNPCType.Professor: return professorRetired;
            case SchoolNPCType.Romance: return romanceRetired;
        }
        return false;
    }

    public void RetireNPCType(SchoolNPCType type)
    {
        switch (type)
        {
            case SchoolNPCType.Friend: friendRetired = true; break;
            case SchoolNPCType.Senior: seniorRetired = true; break;
            case SchoolNPCType.Professor: professorRetired = true; break;
            case SchoolNPCType.Romance: romanceRetired = true; break;
        }
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
        ResetDailyNPCUsage();
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
    public void UnlockEndingFlag(string endingId)
    {
        if (string.IsNullOrEmpty(endingId)) return;

        switch (endingId)
        {
            case "A_CUT":
                endingACutUnlocked = true;
                break;

            case "GRAD_SCHOOL":
                endingGraduateSchoolUnlocked = true;
                break;
        }
    }
}