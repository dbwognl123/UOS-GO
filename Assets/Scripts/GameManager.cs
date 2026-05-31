
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

public enum SchoolFacilityType
{
    Playground,
    WellnessCenter
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
public enum ClassEnterResult
{
    WrongClass,
    AttendanceOnly,
    StartFinalMinigame,
    AlreadyFinished
}

public enum ShopItemType
{
    EnergyDrink,
    MemoryBread
}
public class GameManager : MonoBehaviour
{

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F8))
        {
            SceneTransitionManager.Instance.LoadScene("EndingScene");
        }
#endif

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F6))
        {
            CurrentWeek = 8;
            Debug.Log("테스트용: 현재 주차를 8주차로 변경");
        }

        if (Input.GetKeyDown(KeyCode.F7))
        {
            CurrentWeek = 16;
            Debug.Log("테스트용: 현재 주차를 16주차로 변경");
        }
#endif
    }
    public static GameManager Instance { get; private set; }

    [Header("Ending Sequence Result")]
    public EndingSequenceResult currentEndingSequence = new EndingSequenceResult();

    
    [Header("NPC Progress")]
    public int friendCurrentStage = 1;
    public int friendStageCap = 99;
    public bool friendRetired = false;
    public bool friendUsedToday = false;

    public int seniorCurrentStage = 1;
    public int seniorStageCap = 99;
    public bool seniorRetired = false;
    public bool seniorUsedToday = false;

    [Header("Vending Buffs")]
    public bool hasEnergyDrinkToday = false;
    public bool hasMemoryBreadToday = false;

    [SerializeField] private int vendingItemCost = 10;
    [SerializeField] private float energyDrinkDrainMultiplier = 0.5f;
    [SerializeField] private int memoryBreadRequiredIntReduction = 20;

    [Header("Grade Components")]
    [SerializeField] private float regularEarned = 0f;
    [SerializeField] private float regularPossible = 0f;
    [SerializeField] private int midtermScore = 0; // 0 ~ 30
    [SerializeField] private int finalScore = 0;   // 0 ~ 30

    public float RegularEarned => regularEarned;
    public float RegularPossible => regularPossible;
    public int MidtermScore => midtermScore;
    public int FinalScore => finalScore;

    public int professorCurrentStage = 2; // 1단계 기본 + 질문 10회 달성 시 2단계 제안이 뜨게
    public int professorStageCap = 99;
    public bool professorRetired = false;
    public bool professorUsedToday = false;
    public int professorQuestionCount = 0;

    public int romanceCurrentStage = 1;
    public int romanceStageCap = 99;
    public bool romanceRetired = false;
    public bool romanceUsedToday = false;

    [Header("School Facility")]
    public bool usedSchoolFacilityToday = false;

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
        08, 10, 11, 15,
        16, 18, 19, 20, 27,
        29 ,33, 35, 37, 39
    };
    private static readonly Dictionary<int, string> classroomNameMap = new Dictionary<int, string>
{
    { 1, "전농관" },
    { 3, "건설공학관" },
    { 4, "창공관" },
    { 5, "인문학관" },
    { 6, "배봉관" },
    { 7, "대학본부" },
    { 8, "자연과학관" },
    { 10, "경농관" },
    { 11, "창의혁신관" },
    { 12, "학생회관" },
    { 13, "시대인재관" },
    { 14, "과학기술관" },
    { 15, "21세기관" },
    { 16, "조형관" },
    { 18, "자작마루" },
    { 19, "정보기술관" },
    { 20, "법학관" },
    { 21, "중앙도서관" },
    { 22, "생활관" },
    { 23, "건축구조실험동" },
    { 24, "토목구조실험동" },
    { 25, "미디어관" },
    { 27, "대강당" },
    { 28, "운동장" },
    { 29, "박물관" },
    { 32, "웰니스센터" },
    { 33, "미래관" },
    { 34, "국제학사" },
    { 35, "음악관" },
    { 36, "어린이집" },
    { 37, "100주년 기념관" },
    { 38, "스마트연구동" },
    { 39, "시대융합관" },
    { 41, "실외테니스장" },
    { 81, "자동화온실" }
};
    public string GetClassroomName(int classroomNumber)
    {
        if (classroomNameMap.TryGetValue(classroomNumber, out string name))
            return name;

        return $"건물 {classroomNumber}";
    }

    public string GetClassroomDisplayMultiline(int classroomNumber)
    {
        return $"{classroomNumber}\n{GetClassroomName(classroomNumber)}";
    }

    public string GetClassroomDisplayInline(int classroomNumber)
    {
        return $"{classroomNumber} {GetClassroomName(classroomNumber)}";
    }


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
    public event Action<string> OnSchoolMessagePopupRequested;
    private void RaiseSchoolMessagePopup(string message)
    {
        OnSchoolMessagePopupRequested?.Invoke(message);
    }


    public void StartNewGame()
    {
        usedSchoolFacilityToday = false;
        CurrentWeek = 1;
        CurrentSchoolEntry = SchoolEntryType.None;
        StudiedToday = false;
        WorkedToday = false;
        ResetDailyNPCUsage();
        regularEarned = 0f;
        regularPossible = 0f;
        midtermScore = 0;
        finalScore = 0;

        hasEnergyDrinkToday = false;
        hasMemoryBreadToday = false;
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
        
        SceneTransitionManager.Instance.LoadScene("MorningScene");
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

    public bool CanUseSchoolFacility()
    {
        if (CurrentPlayer == null) return false;
        return !usedSchoolFacilityToday;
    }

    public bool TryUseWorkoutFacility()
    {
        if (!CanUseWorkoutFacility())
            return false;

        AddCurrentHealth(-10);
        AddMaxHealth(2);
        AddAppearance(1);

        usedSchoolFacilityToday = true;
        OnPlayerStatsRefreshed?.Invoke();

        return true;
    }

    public bool CanUseWorkoutFacility()
    {
        if (CurrentPlayer == null) return false;

        if (usedSchoolFacilityToday) return false;
        if (!IsAllClassesFinished) return false;
        if (CurrentPlayer.currentHealth < 10) return false;

        return true;
    }
    public bool StartStudyMinigame()
    {
        if (CurrentPlayer == null) return false;
        if (StudiedToday) return false;

        // 공부 시작 비용
        AddMaxHealth(-5);
        StudiedToday = true;
        SceneTransitionManager.Instance.LoadScene("StudyMemoryScene");
        
        return true;
    }

    
    public void FinishStudyMinigame(int intelligenceGain)
    {
        if (CurrentPlayer == null) return;

        AddIntelligence(intelligenceGain);
        OnPlayerStatsRefreshed?.Invoke();

        SceneTransitionManager.Instance.LoadScene("EveningScene");

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

        if (professorQuestionCount >= 10 && !professorRetired)
        {
            professorCurrentStage = Mathf.Max(professorCurrentStage, 2);
        }
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
    public int GetMidtermScore()
    {
        return midtermScore;
    }

    public int GetFinalExamScore()
    {
        return finalScore;
    }
    public void ApplyClassResult(float survivedSeconds, float maxSeconds)
    {
        if (CurrentPlayer == null) return;

        float ratio = 0f;
        if (maxSeconds > 0f)
            ratio = Mathf.Clamp01(survivedSeconds / maxSeconds);

        if (IsMidtermWeek())
        {
            midtermScore = Mathf.RoundToInt(ratio * 30f);
        }
        else if (IsFinalWeek())
        {
            finalScore = Mathf.RoundToInt(ratio * 30f);
        }
        else
        {
            regularEarned += survivedSeconds;
            regularPossible += maxSeconds;
        }

        RefreshComputedGrade();
    }
    public void EnterSchool(SchoolEntryType entryType)
    {
        CurrentSchoolEntry = entryType;
        ClearSavedSchoolPlayerPosition();
        SceneTransitionManager.Instance.LoadScene("SchoolScene");
    }

    public int GetShopItemCost(ShopItemType itemType)
    {
        switch (itemType)
        {
            case ShopItemType.EnergyDrink:
                return 10;

            case ShopItemType.MemoryBread:
                return 10;
        }

        return 9999;
    }

    public string GetShopItemName(ShopItemType itemType)
    {
        switch (itemType)
        {
            case ShopItemType.EnergyDrink:
                return "에너지드링크";

            case ShopItemType.MemoryBread:
                return "암기빵";
        }

        return "알 수 없음";
    }

    public string GetShopItemDescription(ShopItemType itemType)
    {
        switch (itemType)
        {
            case ShopItemType.EnergyDrink:
                return "오늘 학교에서 현재체력 감소 속도가 1/2이 됩니다.";

            case ShopItemType.MemoryBread:
                return "오늘 수업의 요구 지능이 감소합니다.";
        }

        return "";
    }

    public bool HasBoughtShopItemToday(ShopItemType itemType)
    {
        switch (itemType)
        {
            case ShopItemType.EnergyDrink:
                return hasEnergyDrinkToday;

            case ShopItemType.MemoryBread:
                return hasMemoryBreadToday;
        }

        return false;
    }

    public bool CanBuyShopItem(ShopItemType itemType, out string reason)
    {
        reason = "";

        if (CurrentPlayer == null)
        {
            reason = "플레이어 정보가 없습니다.";
            return false;
        }

        int cost = GetShopItemCost(itemType);

        if (CurrentPlayer.money < cost)
        {
            reason = "돈이 부족합니다.";
            return false;
        }

        if (HasBoughtShopItemToday(itemType))
        {
            reason = "오늘은 이미 구매했습니다.";
            return false;
        }

        return true;
    }

    public bool TryBuyShopItem(ShopItemType itemType, out string message)
    {
        if (!CanBuyShopItem(itemType, out message))
            return false;

        AddMoney(-GetShopItemCost(itemType));

        switch (itemType)
        {
            case ShopItemType.EnergyDrink:
                hasEnergyDrinkToday = true;
                message = "에너지드링크를 구매했습니다.";
                break;

            case ShopItemType.MemoryBread:
                hasMemoryBreadToday = true;
                message = "암기빵을 구매했습니다.";
                break;
        }

        OnPlayerStatsRefreshed?.Invoke();
        return true;
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

   

    


    public float GetSchoolHealthDrainMultiplier()
    {
        return hasEnergyDrinkToday ? 0.5f : 1f;
    }

    public int GetRequiredIntelligenceReduction()
    {
        return hasMemoryBreadToday ? memoryBreadRequiredIntReduction : 0;
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

    public bool IsLastClassOfToday
    {
        get
        {
            if (todaySchedule == null || todaySchedule.Count == 0) return false;
            return currentClassIndex == todaySchedule.Count - 1;
        }
    }
    public int GetCurrentTargetClassroom()
    {
        if (IsAllClassesFinished)
            return -1;

        return todaySchedule[currentClassIndex];
    }

    public ClassEnterResult TryEnterClassroom(int classroomNumber, Vector3 schoolPlayerPosition)
    {
        if (IsAllClassesFinished)
            return ClassEnterResult.AlreadyFinished;

        if (classroomNumber != GetCurrentTargetClassroom())
            return ClassEnterResult.WrongClass;

        // 1~4교시는 출석만 처리
        if (!IsLastClassOfToday)
        {
            currentClassIndex++;
            CurrentEnteredClassroom = -1;
            SaveSchoolPlayerPosition(schoolPlayerPosition);

            int nextClassroom = GetCurrentTargetClassroom();
            RaiseSchoolMessagePopup(
    $"출석 완료!\n다음 장소: {GetClassroomDisplayInline(nextClassroom)}"
);
            return ClassEnterResult.AttendanceOnly;
        }

        // 마지막 수업이면 미니게임 시작
        CurrentEnteredClassroom = classroomNumber;
        SaveSchoolPlayerPosition(schoolPlayerPosition);
        return ClassEnterResult.StartFinalMinigame;
    }
    public void FinishCurrentClass()
    {
        if (IsAllClassesFinished)
            return;

        // 마지막 수업 미니게임이 끝났으므로 그 수업까지 완료 처리
        currentClassIndex++;

        // 혹시 현재 진입한 교실 저장값이 있으면 초기화
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
        SceneTransitionManager.Instance.LoadScene("ClassScene");
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

    public bool IsMidtermWeek()
    {
        return CurrentWeek == 8;
    }

    public bool IsFinalWeek()
    {
        return CurrentWeek == 16;
    }

    public bool IsExamWeek()
    {
        return IsMidtermWeek() || IsFinalWeek();
    }

    public int GetRegularWeightedScore()
    {
        if (regularPossible <= 0f)
            return 0;

        float ratio = Mathf.Clamp01(regularEarned / regularPossible);
        return Mathf.RoundToInt(ratio * 40f);
    }

    public int GetTotalGradeScore()
    {
        return GetRegularWeightedScore() + midtermScore + finalScore;
    }

    private void RefreshComputedGrade()
    {
        if (CurrentPlayer == null) return;

        CurrentPlayer.grade = GetTotalGradeScore();
        OnPlayerStatsRefreshed?.Invoke();
    }
    public void GoToShop()
    {
        SceneTransitionManager.Instance.LoadScene("ShopScene");



    }

    public void GoToCollection()
    {
        SceneTransitionManager.Instance.LoadScene("CollectionScene");
    }
    public void ReturnToMain()
    {
        SceneTransitionManager.Instance.LoadScene("MainScene");
    }
    public void SkipSchool()
    {
        SceneTransitionManager.Instance.LoadScene("EveningScene");
    }

    public void GoToEvening()
    {
        SceneTransitionManager.Instance.LoadScene("EveningScene");
    }

    public void SleepInEvening()
    {
        if (CurrentPlayer == null) return;

        EndDay();
    }
    public void EndDay()
    {
        CurrentWeek++;
        usedSchoolFacilityToday = false;
        if (CurrentWeek > MAX_WEEK)
        {
            BuildEndingSequenceResult();
            SceneTransitionManager.Instance.LoadScene("EndingScene");
            return;
        }

        StudiedToday = false;
        WorkedToday = false;
        hasEnergyDrinkToday = false;
        hasMemoryBreadToday = false;
        ResetDailyNPCUsage();

        if (CurrentPlayer != null)
            CurrentPlayer.currentHealth = CurrentPlayer.maxHealth;

        GenerateTodaySchedule();
        OnPlayerStatsRefreshed?.Invoke();
        SceneTransitionManager.Instance.LoadScene("MorningScene");
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
    private void BuildEndingSequenceResult()
    {
        if (CurrentPlayer == null)
            return;

        currentEndingSequence = new EndingSequenceResult();

        // A: 학교생활력
        if (CurrentPlayer.campusLife >= 200)
            currentEndingSequence.sceneA = EndingSceneAType.GoodCampusLife;
        else if (CurrentPlayer.campusLife >= 80)
            currentEndingSequence.sceneA = EndingSceneAType.NormalCampusLife;
        else
            currentEndingSequence.sceneA = EndingSceneAType.LonerCampusLife;

        // B: 학점
        if (CurrentPlayer.grade >= 150)
            currentEndingSequence.sceneB = EndingSceneBType.GoodGrade;
        else if (CurrentPlayer.grade >= 70)
            currentEndingSequence.sceneB = EndingSceneBType.NormalGrade;
        else
            currentEndingSequence.sceneB = EndingSceneBType.BadGrade;

        // C: 여자친구 유무
        currentEndingSequence.sceneC = CurrentPlayer.hasGirlfriend
            ? EndingSceneCType.HasGirlfriend
            : EndingSceneCType.NoGirlfriend;

        // D: 특수 이벤트
        if (endingACutUnlocked)
            currentEndingSequence.sceneD = EndingSceneDType.BestFriend;
        else if (endingGraduateSchoolUnlocked)
            currentEndingSequence.sceneD = EndingSceneDType.GraduateSchool;
        else
            currentEndingSequence.sceneD = EndingSceneDType.None;

        // E: 최대체력
        if (CurrentPlayer.maxHealth >= 60)
            currentEndingSequence.sceneE = EndingSceneEType.Healthy;
        else
            currentEndingSequence.sceneE = EndingSceneEType.Tired;

        // F: 행복
        if (CurrentPlayer.happiness >= 50)
            currentEndingSequence.sceneF = EndingSceneFType.Happy;
        else
            currentEndingSequence.sceneF = EndingSceneFType.Unsatisfied;
    }


    public bool StartPartTimeQTEMinigame()
    {
        if (CurrentPlayer == null) return false;
        if (WorkedToday) return false;

        AddMaxHealth(-5);
        WorkedToday = true;

        SceneTransitionManager.Instance.LoadScene("PartTimeQTEScene");
        return true;
    }

    public void FinishPartTimeQTEMinigame(int moneyGain)
    {
        if (CurrentPlayer == null) return;

        AddMoney(moneyGain);
        OnPlayerStatsRefreshed?.Invoke();

        SceneTransitionManager.Instance.LoadScene("EveningScene");
    }
}