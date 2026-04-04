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
        301, 302, 303, 304, 305, 306,
        308, 309, 310, 311, 314, 315,
        316, 319, 320, 333
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
            intelligence = 0,
            health = 10,
            money = 5,
            hasGirlfriend = false,
            appearance = 5
        };
        GenerateTodaySchedule();
        ClearSavedSchoolPlayerPosition();
        SceneManager.LoadScene("MorningScene");
    }

    public bool StudyInEvening() //공부
    {
        if (CurrentPlayer == null) return false;
        if (StudiedToday) return false;

        CurrentPlayer.health = Mathf.Max(0, CurrentPlayer.health - 1);
        CurrentPlayer.intelligence += 1;
        StudiedToday = true;

        return true;
    }
    public bool WorkPartTimeInEvening()
    {
        if (CurrentPlayer == null) return false;
        if (WorkedToday) return false;

        CurrentPlayer.health = Mathf.Max(0, CurrentPlayer.health - 1);
        CurrentPlayer.money += 3;
        WorkedToday = true;

        return true;
    }
    public void AddHappiness(int value)
    {
        if (CurrentPlayer == null) return;
        CurrentPlayer.happiness += value;
    }

    public void AddCampusLife(int value)
    {
        if (CurrentPlayer == null) return;
        CurrentPlayer.campusLife += value;
    }

    public void AddIntelligence(int value)
    {
        if (CurrentPlayer == null) return;
        CurrentPlayer.intelligence += value;
    }

    public void AddHealth(int value)
    {
        if (CurrentPlayer == null) return;
        CurrentPlayer.health += value;
    }

    public void AddMoney(int value)
    {
        if (CurrentPlayer == null) return;
        CurrentPlayer.money += value;
    }

    public void AddAppearance(int value)
    {
        if (CurrentPlayer == null) return;
        CurrentPlayer.appearance += value;
    }

    public void SetGirlfriend(bool value)
    {
        if (CurrentPlayer == null) return;
        CurrentPlayer.hasGirlfriend = value;
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
            int randomIndex = Random.Range(0, pool.Count);
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

        CurrentPlayer.health += 1;
        
    }
    public void EndDay()
    {
        CurrentWeek++;
        WorkedToday = false;
        StudiedToday = false;
        if (CurrentWeek > MAX_WEEK)
        {
            SceneManager.LoadScene("EndingScene");
        }
        else
        {
            GenerateTodaySchedule();
            SceneManager.LoadScene("MorningScene");
        }
    }
}