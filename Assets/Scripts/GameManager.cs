
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
public enum FestivalDatePhase
{
    None,
    Booth,
    Food,
    Concert,
    Finished
}

public enum FestivalBoothType
{
    Tarot,
    BlindDate,
    Sports,
    Food
}

public enum FestivalFoodType
{
    Steak,
    IceCream,
    ChickenGangjeong,
    ChickenSkewer,
    Pizza,
    Sushi
}
public class GameManager : MonoBehaviour
{

    private void Update()
    {
  


        
    }
    public static GameManager Instance { get; private set; }

    [Header("Ending Sequence Result")]
    public EndingSequenceResult currentEndingSequence = new EndingSequenceResult();

    public bool IsDialogueOpen { get; private set; }
    public void SetDialogueOpen(bool isOpen)
    {
        IsDialogueOpen = isOpen;
    }
    [Header("NPC Progress")]
    public int friendCurrentStage = 1;
    public int friendStageCap = 99;
    public bool friendRetired = false;
   

    public int seniorCurrentStage = 1;
    public int seniorStageCap = 99;
    public bool seniorRetired = false;
 

    [Header("Vending Buffs")]
    public bool hasEnergyDrinkToday = false;
    public bool hasMemoryBreadToday = false;

    [SerializeField] private int vendingItemCost = 20;
    [SerializeField] private float energyDrinkDrainMultiplier = 0.5f;
    [SerializeField] private int memoryBreadRequiredIntReduction = 20;
    [SerializeField] private int workoutCost = 10;

    [Header("Grade Components")]
    [SerializeField] private float regularEarned = 0f;
    [SerializeField] private float regularPossible = 0f;
    [SerializeField] private int midtermScore = 0; // 0 ~ 30
    [SerializeField] private int finalScore = 0;   // 0 ~ 30

    [Header("Attendance")]
    [SerializeField] private int absenceCount = 0;
    [SerializeField] private int lastAbsenceWeek = 0;

    public int AbsenceCount => absenceCount;
    public bool HasAcademicWarning => absenceCount >= 4;

    [Header("Meeting")]
    public bool meetingSceneScheduled = false;
    public int femaleFriendCount = 0;



    // 3:3 미팅에서 3쌍을 전부 맞힌 횟수
    public int perfectMeetingSuccessCount = 0;

    public bool romanceNpc1Unlocked = false;

    private readonly HashSet<string> usedNpcStageToday = new HashSet<string>();
    public float RegularEarned => regularEarned;
    public float RegularPossible => regularPossible;
    public int MidtermScore => midtermScore;
    public int FinalScore => finalScore;

    public int professorCurrentStage = 2; // 1단계 기본 + 질문 10회 달성 시 2단계 제안이 뜨게
    public int professorStageCap = 99;
    public bool professorRetired = false;

    public int professorQuestionCount = 0;

    public int romanceCurrentStage = 1;
    public int romanceStageCap = 99;
    public bool romanceRetired = false;

    

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

   
    [SerializeField] private int totalFestivalBoothCount = 4;

    [Header("Festival Date")]
    public bool hasFestivalDatePromise = false;
    public bool festivalDateStarted = false;
    public bool festivalDateFinished = false;
    public bool festivalDateEndedByLowHealth = false;

    public bool festivalDateEnding = false;

    public FestivalDatePhase festivalDatePhase = FestivalDatePhase.None;

    public FestivalBoothType targetFestivalBoothType;
    public FestivalFoodType targetFestivalFoodType;

    public int visitedTargetBoothCount = 0;

    private readonly HashSet<string> visitedFestivalBooths =
        new HashSet<string>();

    [Header("Romance Weekly")]
    public bool talkedToRomanceThisWeek = false;
    [Header("Final Romance")]
    public bool finalConfessionResolved = false;
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
    public void ShowSchoolMessagePopup(string message)
    {
        RaiseSchoolMessagePopup(message);
    }

    public void StartNewGame()
    {
        CurrentWeek = 1;
        CurrentSchoolEntry = SchoolEntryType.None;
        StudiedToday = false;
        WorkedToday = false;
        ResetDailyNPCUsage();
        // NPC 진행 초기화
        friendCurrentStage = 1;
        friendStageCap = 99;
        friendRetired = false;

        seniorCurrentStage = 1;
        seniorStageCap = 99;
        seniorRetired = false;

        // 교수 시작 단계는 현재 네 기획값에 맞춰 유지
        professorCurrentStage = 1;
        professorStageCap = 99;
        professorRetired = false;
        professorQuestionCount = 0;

        romanceCurrentStage = 1;
        romanceStageCap = 99;
        romanceRetired = false;

        // 미팅
        meetingSceneScheduled = false;

        // 축제 진행상태
        festivalDateEnding = false;
        festivalDatePhase = FestivalDatePhase.None;
        visitedTargetBoothCount = 0;
        visitedFestivalBooths.Clear();

        // 학교 관련 임시 상태
        usedSchoolFacilityToday = false;
        ClearSchoolHealthDrainPauses();

        regularEarned = 0f;
        regularPossible = 0f;
        midtermScore = 0;
        finalScore = 0;
        perfectMeetingSuccessCount = 0;
        femaleFriendCount = 0;
        romanceNpc1Unlocked = false;
        hasFestivalDatePromise = false;
        festivalDateStarted = false;
        festivalDateFinished = false;
        festivalDateEndedByLowHealth = false;
        talkedToRomanceThisWeek = false;
        finalConfessionResolved = false;
        endingACutUnlocked = false;

        absenceCount = 0;
        lastAbsenceWeek = 0;

        endingGraduateSchoolUnlocked = false;

        currentEndingSequence =
            new EndingSequenceResult();

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


        // 장착한 소모품의 능력치 보너스 적용
        StartBonusData bonus =
            MetaProgressSystem.ConsumeSelectedBonus();
        // 장착 캐릭터의 시작 보너스
        StartBonusData characterBonus =
            MetaProgressSystem.GetSelectedCharacterStartBonus();

        // 장착 캐릭터 능력 확정
        CurrentPlayer.activeCharacterId =
            MetaProgressSystem.GetSelectedCharacterId();

        CurrentPlayer.activeCharacterAbility =
            MetaProgressSystem.GetSelectedCharacterAbility();

        // 기존 코드에서 가져온 bonus 사용
        CurrentPlayer.schoolHealthLossReduction =
            bonus.schoolHealthLossReduction;

        CurrentPlayer.confessionBonusPercent =
            bonus.confessionBonusPercent;

        CurrentPlayer.weeklyMoneyBonus =
            bonus.weeklyMoneyBonus;



        // ========================================
        // 시작 능력치 적용
        // 소모품 보너스 + 캐릭터 보너스
        // 배율은 적용하지 않음
        // ========================================

        CurrentPlayer.money = Mathf.Max(
            0,
            CurrentPlayer.money
            + bonus.money
            + characterBonus.money
        );

        CurrentPlayer.intelligence = Mathf.Max(
            0,
            CurrentPlayer.intelligence
            + bonus.intelligence
            + characterBonus.intelligence
        );

        CurrentPlayer.maxHealth = Mathf.Max(
            1,
            CurrentPlayer.maxHealth
            + bonus.maxHealth
            + characterBonus.maxHealth
        );

        // 시작 시 현재체력은 최대체력으로 설정
        CurrentPlayer.currentHealth =
            CurrentPlayer.maxHealth;

        CurrentPlayer.happiness = Mathf.Clamp(
            CurrentPlayer.happiness
            + bonus.happiness
            + characterBonus.happiness,
            0, 999
        );

        CurrentPlayer.campusLife = Mathf.Clamp(
            CurrentPlayer.campusLife
            + bonus.campusLife
            + characterBonus.campusLife,
            0, 999
        );

        CurrentPlayer.appearance = Mathf.Clamp(
            CurrentPlayer.appearance
            + bonus.appearance
            + characterBonus.appearance,
            0, 999
        );



        CurrentPlayer.romanceAffection = 0;

        // 1주차 용돈 지급
        GiveWeeklyAllowanceIfNeeded();

        GenerateTodaySchedule();
        ClearSavedSchoolPlayerPosition();
        OnPlayerStatsRefreshed?.Invoke();
        SaveGame();
    }
    

    public bool CanUseSchoolFacility()
    {
        if (CurrentPlayer == null) return false;
        return !usedSchoolFacilityToday;
    }

    public string GetWorkoutUnavailableReason()
    {
        if (CurrentPlayer == null)
            return "플레이어 정보를 불러올 수 없습니다.";

        if (usedSchoolFacilityToday)
            return "오늘은 이미 운동했어.";

        if (!IsAllClassesFinished)
            return "수업부터 다 듣고 운동하자.";

        if (CurrentPlayer.currentHealth <= 10)
            return "아 오늘은 힘들어서 운동 못하겠다..";

        if (CurrentPlayer.money < workoutCost)
            return $"돈이 부족하네... 운동하려면 {workoutCost}원이 필요해.";

        return "";
    }
    public bool TryUseWorkoutFacility()
    {
        if (!CanUseWorkoutFacility())
            return false;

        AddMoney(-workoutCost);
        AddCurrentHealth(-10);

        // 운동부 캐릭터 효과
        int healthGain =
            CharacterEffectCalculator.GetExerciseHealthGain(
                10,
                CurrentPlayer.activeCharacterAbility
            );

        AddMaxHealth(healthGain);

        AddAppearance(5);

        usedSchoolFacilityToday = true;

        return true;
    }
    public bool CanUseWorkoutFacility()
    {
        if (CurrentPlayer == null)
            return false;

        if (usedSchoolFacilityToday)
            return false;

        if (!IsAllClassesFinished)
            return false;

        if (CurrentPlayer.currentHealth <= 10)
            return false;

        if (CurrentPlayer.money < workoutCost)
            return false;

        return true;
    }
    public bool StartStudyMinigame()
    {
        if (CurrentPlayer == null) return false;
        if (StudiedToday) return false;

        // 공부 시작 비용
        AddMaxHealth(-10);
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


    

    public float GetMaxHealthSpeedMultiplier(
    float minimumMultiplier = 0.45f,
    float healthExponent = 2f)
    {
        if (CurrentPlayer == null)
            return 1f;

        const float standardMaxHealth = 100f;

        float health01 = Mathf.Clamp01(
            CurrentPlayer.maxHealth / standardMaxHealth
        );

        float curvedHealth = Mathf.Pow(
            health01,
            healthExponent
        );

        return Mathf.Lerp(
            minimumMultiplier,
            1f,
            curvedHealth
        );
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
        if (CurrentPlayer == null)
            return;

        // 현재 캐릭터의 학교생활력 증가 배율 적용
        int finalValue =
            CharacterEffectCalculator.GetCampusLifeGain(
                value,
                CurrentPlayer.activeCharacterAbility
            );

        int before = CurrentPlayer.campusLife;

        CurrentPlayer.campusLife = Mathf.Clamp(
            CurrentPlayer.campusLife + finalValue,
            0,
            999
        );

        int actualChange =
            CurrentPlayer.campusLife - before;

        Debug.Log(
            $"[Character] 학교생활력: {value} → {actualChange}"
        );

        OnPlayerStatsRefreshed?.Invoke();
    }


    public void AddIntelligence(int value)
    {
        if (CurrentPlayer == null)
            return;

        // 캐릭터 고유 능력 적용
        int finalValue =
            CharacterEffectCalculator.GetIntelligenceGain(
                value,
                CurrentPlayer.activeCharacterAbility
            );

        int before = CurrentPlayer.intelligence;

        CurrentPlayer.intelligence = Mathf.Max(
            0,
            CurrentPlayer.intelligence + finalValue
        );

        int actualChange =
            CurrentPlayer.intelligence - before;

        OnPlayerStatChanged?.Invoke(
            PlayerStatType.Intelligence,
            actualChange
        );

        OnPlayerStatsRefreshed?.Invoke();

        Debug.Log(
            $"[Character] 지능 변화: {value} → {actualChange}"
        );
    }

    public void AddMaxHealth(int value)
    {
        if (CurrentPlayer == null)
            return;

        // 최대체력 상한 제거
        CurrentPlayer.maxHealth = Mathf.Max(
            1,
            CurrentPlayer.maxHealth + value
        );

        // 최대체력이 감소해서 현재체력보다 작아진 경우만 보정
        if (CurrentPlayer.currentHealth >
            CurrentPlayer.maxHealth)
        {
            CurrentPlayer.currentHealth =
                CurrentPlayer.maxHealth;
        }

        OnPlayerStatChanged?.Invoke(
            PlayerStatType.Health,
            value
        );

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
    public void AddRomanceAffection(int value)
    {
        if (CurrentPlayer == null)
            return;

        CurrentPlayer.romanceAffection =
            Mathf.Clamp(
                CurrentPlayer.romanceAffection + value,
                0,
                100
            );

        Debug.Log(
            $"[Romance] 호감도=" +
            $"{CurrentPlayer.romanceAffection}"
        );

        OnPlayerStatsRefreshed?.Invoke();
    }
    public float GetRomanceSpawnChance()
    {
        if (perfectMeetingSuccessCount <= 0)
            return 0f;

        float chance =
            20f +
            (perfectMeetingSuccessCount -1) * 20f;

        return Mathf.Clamp(
            chance,
            20f,
            60f
        );
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
        ClearSchoolHealthDrainPauses();

        CurrentSchoolEntry = entryType;
        ClearSavedSchoolPlayerPosition();

        SceneTransitionManager.Instance.LoadScene(
            "SchoolScene"
        );
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

    public void UnlockRomanceNpc1()
    {
        romanceNpc1Unlocked = true;
    }

    public void LockRomanceNpc1()
    {
        romanceNpc1Unlocked = false;
    }
    public void SetFestivalDatePromise()
    {
        hasFestivalDatePromise = true;

        Debug.Log(
            $"[Romance] 10주차 축제 약속 설정 완료. " +
            $"현재 주차={CurrentWeek}"
        );
    }

    public void StartFestivalDate()
    {
        if (CurrentWeek != 10)
            return;

        if (!hasFestivalDatePromise)
            return;

        festivalDateStarted = true;
        festivalDateFinished = false;
        festivalDateEnding = false;
        festivalDateEndedByLowHealth = false;

        festivalDatePhase = FestivalDatePhase.Booth;

        visitedFestivalBooths.Clear();
        visitedTargetBoothCount = 0;

        targetFestivalBoothType =
            (FestivalBoothType)UnityEngine.Random.Range(0, 4);

        FestivalDialogueController.Instance?.
            ShowStartDialogue();

        Debug.Log(
            $"[Festival] 데이트 시작 / 목표={targetFestivalBoothType}"
        );
    }
    public string GetFestivalBoothDateLine()
    {
        string boothName = GetFestivalBoothName(targetFestivalBoothType);

        return $"왔어? 우리 과가 {boothName} 하는데 한번 구경 가볼래?";
    }
    public string GetFestivalBoothName(FestivalBoothType type)
    {
        switch (type)
        {
            case FestivalBoothType.Tarot:
                return "타로 부스";

            case FestivalBoothType.BlindDate:
                return "소개팅 부스";

            case FestivalBoothType.Sports:
                return "운동 부스";

            case FestivalBoothType.Food:
                return "음식 부스";
        }

        return "부스";
    }

    public void VisitFestivalBooth(
    string boothId,
    FestivalBoothType boothType)
    {
        if (!festivalDateStarted ||
            festivalDateFinished)
            return;

        if (festivalDatePhase != FestivalDatePhase.Booth)
            return;

        if (boothType != targetFestivalBoothType)
            return;

        // 같은 장소 중복 방문 방지
        if (!visitedFestivalBooths.Add(boothId))
            return;

        visitedTargetBoothCount++;

        Debug.Log(
            $"[Festival] 목표 부스 방문 " +
            $"{visitedTargetBoothCount}/3"
        );

        // 1번째 부스
        if (visitedTargetBoothCount == 1)
        {
            FestivalDialogueController.Instance?.
                ShowFirstWrongBoothDialogue();

            return;
        }

        // 2번째 부스
        if (visitedTargetBoothCount == 2)
        {
            FestivalDialogueController.Instance?.
                ShowSecondWrongBoothDialogue();

            return;
        }

        // 3번째 부스
        if (visitedTargetBoothCount >= 3)
        {
            StartFestivalFoodPhase();
        }
    }
    private void StartFestivalFoodPhase()
    {
        festivalDatePhase =
            FestivalDatePhase.Food;

        targetFestivalFoodType =
            (FestivalFoodType)UnityEngine.Random.Range(0, 6);

        FestivalDialogueController.Instance?.
            ShowFoodRequestDialogue();

        Debug.Log(
            $"[Festival] 원하는 음식 = " +
            $"{targetFestivalFoodType}"
        );
    }
    public bool IsAllFestivalBoothsVisited()
    {
        return visitedFestivalBooths.Count
            >= totalFestivalBoothCount;
    }

    public int GetVisitedFestivalBoothCount()
    {
        return visitedFestivalBooths.Count;
    }

    public string GetFestivalFoodName(FestivalFoodType type)
    {
        switch (type)
        {
            case FestivalFoodType.Steak:
                return "스테이크";

            case FestivalFoodType.IceCream:
                return "구슬아이스크림";

            case FestivalFoodType.ChickenGangjeong:
                return "닭강정";

            case FestivalFoodType.ChickenSkewer:
                return "닭꼬치";

            case FestivalFoodType.Pizza:
                return "피자";

            case FestivalFoodType.Sushi:
                return "초밥";
        }

        return "뭐 맛있는 거";
    }
    public void StartFestivalConcertPhase()
    {
        if (!festivalDateStarted ||
            festivalDateFinished)
            return;

        festivalDatePhase =
            FestivalDatePhase.Concert;

        Debug.Log("[Festival] 공연 단계 시작");
    }
    public string GetFestivalFoodRequestLine()
    {
        string foodName =
            GetFestivalFoodName(targetFestivalFoodType);

        return $"ㅋㅋ 재밌네. 푸드트럭도 가보자. 나 {foodName} 땡긴다.";
    }

    public void CompleteFestivalDate()
    {
        if (festivalDateFinished)
            return;

        festivalDateFinished = true;
        festivalDateEnding = false;
        festivalDatePhase = FestivalDatePhase.Finished;

        AddRomanceAffection(40);

        Debug.Log(
            "[Festival] 축제 데이트 정상 완료 / 호감도 +40"
        );
    }

    public void EndFestivalDateByLowHealth()
    {
        if (festivalDateFinished)
            return;

        festivalDateFinished = true;
        festivalDateEnding = false;
        festivalDateEndedByLowHealth = true;
        festivalDatePhase = FestivalDatePhase.Finished;

        AddRomanceAffection(20);

        Debug.Log(
            "[Festival] 체력 부족 종료 / 호감도 +20"
        );
    }

    public void ResolveFinalConfession(bool success)
    {
        if (finalConfessionResolved)
            return;

        finalConfessionResolved = true;

        if (success && CurrentPlayer != null)
        {
            CurrentPlayer.hasGirlfriend = true;

            Debug.Log(
                $"[Romance] 고백 성공 / 호감도={CurrentPlayer.romanceAffection}"
            );
        }
        else
        {
            Debug.Log(
                $"[Romance] 고백 종료 / 성공={success}"
            );
        }

        OnPlayerStatsRefreshed?.Invoke();
    }
    public bool CanAttemptFinalConfession()
    {
        if (CurrentPlayer == null)
            return false;

        return CurrentPlayer.romanceAffection >= 50;
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
    public void RegisterRomanceConversation()
    {
        // 11~15주차에서만 적용
        if (CurrentWeek < 11 || CurrentWeek > 15)
            return;

        // 이번 주에 이미 호감도를 받았으면 중복 지급 X
        if (talkedToRomanceThisWeek)
            return;

        talkedToRomanceThisWeek = true;

        AddRomanceAffection(10);

        Debug.Log(
            $"[Romance] {CurrentWeek}주차 Romance NPC 대화 / " +
            $"호감도 +10 / 현재={CurrentPlayer.romanceAffection}"
        );
    }
    private void ApplyWeeklyRomanceResult()
    {
        // 11~15주차에서만
        if (CurrentWeek < 11 || CurrentWeek > 15)
            return;

        if (!talkedToRomanceThisWeek)
        {
            AddRomanceAffection(-10);

            Debug.Log(
                $"[Romance] {CurrentWeek}주차 Romance NPC와 대화하지 않음 / " +
                $"호감도 -10 / 현재={CurrentPlayer.romanceAffection}"
            );
        }
        else
        {
            Debug.Log(
                $"[Romance] {CurrentWeek}주차 Romance NPC 대화 완료 / " +
                $"호감도 감소 없음"
            );
        }
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
    private string GetNpcStageUsageKey(SchoolNPCType type, int stage)
    {
        return $"{type}_{stage}";
    }

    public void ResetDailyNPCUsage()
    {
        usedNpcStageToday.Clear();
    }

    public bool IsNPCStageUsedToday(SchoolNPCType type, int stage)
    {
        return usedNpcStageToday.Contains(GetNpcStageUsageKey(type, stage));
    }

    public void MarkNPCStageUsedToday(SchoolNPCType type, int stage)
    {
        usedNpcStageToday.Add(GetNpcStageUsageKey(type, stage));
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



    private readonly HashSet<string> schoolDrainPauseReasons =
    new HashSet<string>();

    public bool IsSchoolHealthDrainPaused
    {
        get
        {
            return IsDialogueOpen ||
                   schoolDrainPauseReasons.Count > 0;
        }
    }

    public void PauseSchoolHealthDrain(string reason)
    {
        if (string.IsNullOrEmpty(reason))
            return;

        schoolDrainPauseReasons.Add(reason);
    }

    public void ResumeSchoolHealthDrain(string reason)
    {
        if (string.IsNullOrEmpty(reason))
            return;

        schoolDrainPauseReasons.Remove(reason);
    }

    public void ClearSchoolHealthDrainPauses()
    {
        schoolDrainPauseReasons.Clear();
        IsDialogueOpen = false;
    }


    public float GetSchoolHealthDrainMultiplier()
    {
        return hasEnergyDrinkToday
            ? energyDrinkDrainMultiplier
            : 1f;
    }

    public int GetRequiredIntelligenceReduction()
    {
        return hasMemoryBreadToday ? memoryBreadRequiredIntReduction : 0;
    }

    public int GetCurrentRequiredIntelligence()
    {
        int requiredIntelligence =
            CurrentWeek * 20;

        requiredIntelligence -=
            GetRequiredIntelligenceReduction();

        return Mathf.Max(
            0,
            requiredIntelligence
        );
    }

    public void GenerateTodaySchedule()
    {
        List<int> pool = new List<int>(allClassrooms);
        todaySchedule.Clear();

        for (int i = 0; i < 4; i++)
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

        float ratio = Mathf.Clamp01(regularEarned / 200);
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

    public void ScheduleMeetingScene()
    {
        meetingSceneScheduled = true;
    }

    public void ApplyMeetingSceneResult(int correctCount)
    {
        correctCount = Mathf.Clamp(correctCount, 0, 3);

        femaleFriendCount += correctCount;
        AddCampusLife(correctCount * 10);

        // 한 번의 3:3 미팅에서 3쌍 모두 정확히 매칭하면
        // "완벽 성공" 1회 누적
        if (correctCount == 3)
        {
            perfectMeetingSuccessCount++;

            Debug.Log(
                $"[미팅] 완벽 성공! " +
                $"누적 성공 횟수: {perfectMeetingSuccessCount}"
            );
        }

        meetingSceneScheduled = false;
        EndDay();
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
        if (CurrentPlayer == null)
            return;

        // 한 주차에 결석은 한 번만 기록
        if (lastAbsenceWeek != CurrentWeek)
        {
            lastAbsenceWeek = CurrentWeek;
            absenceCount++;

            // 휴식 보상
            AddMaxHealth(20);
            AddHappiness(10);

            // 결석 패널티
            AddCampusLife(-30);

            Debug.Log(
                $"[Attendance] 누적 결석: {absenceCount}회 / " +
                $"최대체력: {CurrentPlayer.maxHealth} / " +
                $"행복: {CurrentPlayer.happiness} / " +
                $"학교생활력: {CurrentPlayer.campusLife}"
            );

            if (HasAcademicWarning)
            {
                Debug.Log(
                    "[Ending] 학사경고 히든 엔딩 조건 달성!"
                );
            }

            // 결석 기록 및 스탯 저장
            SaveGame();
        }

        // 기존 저녁 이동
        SceneTransitionManager.Instance.LoadScene(
            "EveningScene"
        );
    }


    public void GoToEvening()
    {
        if (meetingSceneScheduled)
        {
            SceneTransitionManager.Instance.LoadScene("MeetingScene");
            return;
        }

        SceneTransitionManager.Instance.LoadScene("EveningScene");
    }

    public void SleepInEvening()
    {
        if (CurrentPlayer == null) return;

        EndDay();
    }
    public void EndDay()
    {
        // 현재 주차의 Romance 결과를 먼저 정산
        ApplyWeeklyRomanceResult();

        // 다음 주차를 위해 초기화
        talkedToRomanceThisWeek = false;

        // 그 다음 주차 증가
        CurrentWeek++;

        usedSchoolFacilityToday = false;

        if (CurrentWeek > MAX_WEEK)
        {
            // 이번 플레이 결과 계산
            BuildEndingSequenceResult();

            // 실제 플레이로 획득한 엔딩만
            // 영구 컬렉션에 저장
            UnlockCurrentEndingCollection();

            // 16주 완주 보상
            MetaProgressSystem.AddCoin(20);

            SaveSystem.DeleteSave();//진행중인 세이브 제거

            SceneTransitionManager.Instance.LoadScene(
                "EndingScene"
            );

            return;
        }
        GiveWeeklyAllowanceIfNeeded();

        StudiedToday = false;
        WorkedToday = false;
        StudiedToday = false;
        WorkedToday = false;
        hasEnergyDrinkToday = false;
        hasMemoryBreadToday = false;

        ResetDailyNPCUsage();

        if (CurrentPlayer != null)
            CurrentPlayer.currentHealth =
                CurrentPlayer.maxHealth;

        GenerateTodaySchedule();
        OnPlayerStatsRefreshed?.Invoke();

        SaveGame();

        SceneTransitionManager.Instance.
            LoadScene("MorningScene");
    }
    public void UnlockEndingFlag(string endingId)
    {
        if (string.IsNullOrEmpty(endingId))
            return;

        switch (endingId)
        {
            case "A_CUT":
            case "BEST_FRIEND":
                endingACutUnlocked = true;

                Debug.Log(
                    "[Ending] 찐친 히든 조건 달성"
                );
                break;

            case "GRAD_SCHOOL":
                endingGraduateSchoolUnlocked = true;

                Debug.Log(
                    "[Ending] 대학원 히든 조건 달성"
                );
                break;
        }
    }

    private void BuildEndingSequenceResult()
    {
        if (CurrentPlayer == null)
            return;

        currentEndingSequence =
            new EndingSequenceResult();

        // =========================================
        // A : 성적
        // =========================================

        if (CurrentPlayer.grade >= 70)
        {
            currentEndingSequence.sceneA =
                EndingSceneAType.GoodGrade;
        }
        else if (CurrentPlayer.grade >= 50)
        {
            currentEndingSequence.sceneA =
                EndingSceneAType.NormalGrade;
        }
        else
        {
            currentEndingSequence.sceneA =
                EndingSceneAType.BadGrade;
        }

        // =========================================
        // B : 학교생활
        // =========================================

        if (CurrentPlayer.campusLife >= 250)
        {
            currentEndingSequence.sceneB =
                EndingSceneBType.GoodCampusLife;
        }
        else if (CurrentPlayer.campusLife >= 100)
        {
            currentEndingSequence.sceneB =
                EndingSceneBType.NormalCampusLife;
        }
        else
        {
            currentEndingSequence.sceneB =
                EndingSceneBType.BadCampusLife;
        }

        // =========================================
        // C : 여자친구
        // =========================================

        currentEndingSequence.sceneC =
            CurrentPlayer.hasGirlfriend
                ? EndingSceneCType.HasGirlfriend
                : EndingSceneCType.NoGirlfriend;

        // =========================================
        // D : 행복
        // =========================================

        if (CurrentPlayer.happiness >= 40)
        {
            currentEndingSequence.sceneD =
                EndingSceneDType.Happy;
        }
        else
        {
            currentEndingSequence.sceneD =
                EndingSceneDType.Unhappy;
        }

        // =========================================
        // E : 히든 - 찐친
        // =========================================

        currentEndingSequence.sceneE =
            endingACutUnlocked
                ? EndingSceneEType.BestFriend
                : EndingSceneEType.None;

        // =========================================
        // F : 히든 - 대학원
        // =========================================

        currentEndingSequence.sceneF =
            endingGraduateSchoolUnlocked
                ? EndingSceneFType.GraduateSchool
                : EndingSceneFType.None;

        Debug.Log(
            $"[Ending] " +
            $"A={currentEndingSequence.sceneA}, " +
            $"B={currentEndingSequence.sceneB}, " +
            $"C={currentEndingSequence.sceneC}, " +
            $"D={currentEndingSequence.sceneD}, " +
            $"E={currentEndingSequence.sceneE}, " +
            $"F={currentEndingSequence.sceneF}"
        );
    }
    public bool StartPartTimeQTEMinigame()
    {
        if (CurrentPlayer == null) return false;
        if (WorkedToday) return false;

        AddMaxHealth(-10);
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
    public void DebugSetWeek9WithFestivalPromise()
    {
        CurrentWeek = 9;

        hasFestivalDatePromise = true;

        // 테스트용: 미팅 완벽 성공 1회
        perfectMeetingSuccessCount = 1;

        festivalDateStarted = false;
        festivalDateFinished = false;
        festivalDateEndedByLowHealth = false;
        festivalDateEnding = false;

        festivalDatePhase = FestivalDatePhase.None;

        Debug.Log(
            $"[Debug] 9주차 / 축제 약속 완료 / " +
            $"미팅 완벽 성공={perfectMeetingSuccessCount}회 / " +
            $"Romance 등장확률={GetRomanceSpawnChance()}%"
        );
    }


    public void GoToEveningSceneAfterFestival()
    {
        SceneManager.LoadScene("EveningScene");
    
    }


    private void UnlockCollectionWithReward(
    string endingKey)
    {
        bool isNew =
            EndingCollectionSave.Unlock(
                endingKey
            );

        if (!isNew)
            return;

        const int rewardCoin = 20;

        MetaProgressSystem.AddCoin(
            rewardCoin
        );
    }
    private void UnlockCurrentEndingCollection()
    {
        if (currentEndingSequence == null)
            return;

        EndingSequenceResult result =
            currentEndingSequence;

        // A : 성적
        UnlockCollectionWithReward(
            $"A_{result.sceneA}"
        );

        // B : 학교생활
        UnlockCollectionWithReward(
            $"B_{result.sceneB}"
        );

        // C : 여자친구
        UnlockCollectionWithReward(
            $"C_{result.sceneC}"
        );

        // E : 히든 - 찐친
        if (result.sceneE != EndingSceneEType.None)
        {
            UnlockCollectionWithReward(
                $"E_{result.sceneE}"
            );
        }

        // F : 히든 - 대학원
        if (result.sceneF != EndingSceneFType.None)
        {
            UnlockCollectionWithReward(
                $"F_{result.sceneF}"
            );
        }

        if (HasAcademicWarning)
        {
            UnlockCollectionWithReward(
                "H_AcademicWarning"
            );
        }
        // D : 행복
        UnlockCollectionWithReward(
            $"D_{result.sceneD}"
        );
    }

    //세이브
    private SaveGameData CreateSaveData()
    {
        if (CurrentPlayer == null)
            return null;

        SaveGameData data =
            new SaveGameData();

        // =========================
        // 진행
        // =========================

        data.currentWeek =
            CurrentWeek;

        // =========================
        // 플레이어
        // =========================

        data.absenceCount = absenceCount;
        data.lastAbsenceWeek = lastAbsenceWeek;

        data.happiness =
            CurrentPlayer.happiness;

        data.campusLife =
            CurrentPlayer.campusLife;

        data.intelligence =
            CurrentPlayer.intelligence;

        data.maxHealth =
            CurrentPlayer.maxHealth;

        data.currentHealth =
            CurrentPlayer.currentHealth;

        data.money =
            CurrentPlayer.money;

        data.appearance =
            CurrentPlayer.appearance;

        data.grade =
            CurrentPlayer.grade;

        data.hasGirlfriend =
            CurrentPlayer.hasGirlfriend;

        data.romanceAffection =
            CurrentPlayer.romanceAffection;


        // 장착 캐릭터 및 고유 능력
        data.activeCharacterId =
            CurrentPlayer.activeCharacterId;

        data.activeCharacterAbility =
            CurrentPlayer.activeCharacterAbility;

        // 이번 회차 지속 아이템
        data.schoolHealthLossReduction =
            CurrentPlayer.schoolHealthLossReduction;

        data.confessionBonusPercent =
            CurrentPlayer.confessionBonusPercent;

        data.weeklyMoneyBonus =
            CurrentPlayer.weeklyMoneyBonus;

        data.lastAllowanceWeek =
            CurrentPlayer.lastAllowanceWeek;


        // =========================
        // 학점
        // =========================

        data.regularEarned =
            regularEarned;

        data.regularPossible =
            regularPossible;

        data.midtermScore =
            midtermScore;

        data.finalScore =
            finalScore;

        // =========================
        // NPC
        // =========================

        data.friendCurrentStage =
            friendCurrentStage;

        data.friendStageCap =
            friendStageCap;

        data.friendRetired =
            friendRetired;

        data.seniorCurrentStage =
            seniorCurrentStage;

        data.seniorStageCap =
            seniorStageCap;

        data.seniorRetired =
            seniorRetired;

        data.professorCurrentStage =
            professorCurrentStage;

        data.professorStageCap =
            professorStageCap;

        data.professorRetired =
            professorRetired;

        data.professorQuestionCount =
            professorQuestionCount;

        data.romanceCurrentStage =
            romanceCurrentStage;

        data.romanceStageCap =
            romanceStageCap;

        data.romanceRetired =
            romanceRetired;

        // =========================
        // 미팅 / 연애
        // =========================

        data.femaleFriendCount =
            femaleFriendCount;

        data.perfectMeetingSuccessCount =
            perfectMeetingSuccessCount;

        data.romanceNpc1Unlocked =
            romanceNpc1Unlocked;

        // =========================
        // 축제
        // =========================

        data.hasFestivalDatePromise =
            hasFestivalDatePromise;

        data.festivalDateStarted =
            festivalDateStarted;

        data.festivalDateFinished =
            festivalDateFinished;

        data.festivalDateEndedByLowHealth =
            festivalDateEndedByLowHealth;

        // =========================
        // 히든 엔딩
        // =========================

        data.endingACutUnlocked =
            endingACutUnlocked;

        data.endingGraduateSchoolUnlocked =
            endingGraduateSchoolUnlocked;

        // =========================
        // 시간표
        // =========================

        data.todaySchedule =
            new List<int>(
                todaySchedule
            );

        return data;
    }
    public void SaveGame()
    {
        if (CurrentPlayer == null)
        {
            Debug.LogWarning(
                "[Save] 플레이어가 없어서 저장할 수 없습니다."
            );

            return;
        }

        SaveGameData data =
            CreateSaveData();

        SaveSystem.Save(data);
    }
    private void ApplySaveData(
    SaveGameData data)
    {
        if (data == null)
            return;

        // =========================
        // 진행
        // =========================

        CurrentWeek =
            Mathf.Clamp(
                data.currentWeek,
                1,
                MAX_WEEK
            );

        // 이어하기는 항상 Morning에서 시작
        CurrentSchoolEntry =
            SchoolEntryType.None;

        // =========================
        // 플레이어
        // =========================

        CurrentPlayer =
            new PlayerRunData
            {
                happiness =
                    data.happiness,

                campusLife =
                    data.campusLife,

                intelligence =
                    data.intelligence,

                maxHealth =
                    data.maxHealth,

                currentHealth =
                    data.currentHealth,

                money =
                    data.money,

                appearance =
                    data.appearance,

                grade =
                    data.grade,

                hasGirlfriend =
                    data.hasGirlfriend
            };

        CurrentPlayer.romanceAffection =
            data.romanceAffection;


        // 해당 회차에 선택했던 캐릭터 복구
        CurrentPlayer.activeCharacterId =
            string.IsNullOrEmpty(data.activeCharacterId)
                ? "DEFAULT"
                : data.activeCharacterId;

        CurrentPlayer.activeCharacterAbility =
            data.activeCharacterAbility
            ?? new CharacterAbilityData();

        // 지속 아이템 효과 복구
        CurrentPlayer.schoolHealthLossReduction =
            data.schoolHealthLossReduction;

        CurrentPlayer.confessionBonusPercent =
            data.confessionBonusPercent;

        CurrentPlayer.weeklyMoneyBonus =
            data.weeklyMoneyBonus;

        CurrentPlayer.lastAllowanceWeek =
            data.lastAllowanceWeek;


        // =========================
        // 학점
        // =========================

        regularEarned =
            data.regularEarned;

        regularPossible =
            data.regularPossible;

        midtermScore =
            data.midtermScore;

        finalScore =
            data.finalScore;

        // =========================
        // NPC
        // =========================

        friendCurrentStage =
            data.friendCurrentStage;

        friendStageCap =
            data.friendStageCap;

        friendRetired =
            data.friendRetired;

        seniorCurrentStage =
            data.seniorCurrentStage;

        seniorStageCap =
            data.seniorStageCap;

        seniorRetired =
            data.seniorRetired;

        professorCurrentStage =
            data.professorCurrentStage;

        professorStageCap =
            data.professorStageCap;

        professorRetired =
            data.professorRetired;

        professorQuestionCount =
            data.professorQuestionCount;

        romanceCurrentStage =
            data.romanceCurrentStage;

        romanceStageCap =
            data.romanceStageCap;

        romanceRetired =
            data.romanceRetired;

        // =========================
        // 미팅 / 연애
        // =========================

        femaleFriendCount =
            data.femaleFriendCount;

        perfectMeetingSuccessCount =
            data.perfectMeetingSuccessCount;

        romanceNpc1Unlocked =
            data.romanceNpc1Unlocked;

        // =========================
        // 축제
        // =========================

        hasFestivalDatePromise =
            data.hasFestivalDatePromise;

        festivalDateStarted =
            data.festivalDateStarted;

        festivalDateFinished =
            data.festivalDateFinished;

        festivalDateEndedByLowHealth =
            data.festivalDateEndedByLowHealth;

        // =========================
        // 엔딩
        // =========================

        endingACutUnlocked =
            data.endingACutUnlocked;

        endingGraduateSchoolUnlocked =
            data.endingGraduateSchoolUnlocked;

        currentEndingSequence =
            new EndingSequenceResult();

        // =========================
        // 일일 상태 초기화
        // =========================

        StudiedToday = false;
        WorkedToday = false;

        usedSchoolFacilityToday = false;

        hasEnergyDrinkToday = false;
        hasMemoryBreadToday = false;

        talkedToRomanceThisWeek = false;

        ResetDailyNPCUsage();

        // =========================
        // 축제 런타임 상태 정리
        // =========================

        festivalDateEnding = false;

        visitedTargetBoothCount = 0;
        visitedFestivalBooths.Clear();

        // =========================
        // 시간표 복원
        // =========================

        todaySchedule.Clear();

        if (data.todaySchedule != null &&
            data.todaySchedule.Count > 0)
        {
            todaySchedule.AddRange(
                data.todaySchedule
            );
        }
        else
        {
            GenerateTodaySchedule();
        }

        currentClassIndex = 0;
        CurrentEnteredClassroom = -1;

        // =========================
        // 씬 일시상태 초기화
        // =========================

        ClearSavedSchoolPlayerPosition();
        ClearSchoolHealthDrainPauses();

        finalConfessionResolved = false;
        meetingSceneScheduled = false;

        OnPlayerStatsRefreshed?.Invoke();

        Debug.Log(
            $"[Save] 게임 데이터 복원 완료 / " +
            $"Week={CurrentWeek} / " +
            $"Money={CurrentPlayer.money} / " +
            $"Int={CurrentPlayer.intelligence}"
        );
    }

    public void ApplySchoolHealthLoss(int baseLoss)
    {
        if (CurrentPlayer == null || baseLoss <= 0)
            return;

        int reduction = Mathf.Max(
            0,
            CurrentPlayer.schoolHealthLossReduction
        );

        int finalLoss = Mathf.Max(
            0,
            baseLoss - reduction
        );

        if (finalLoss > 0)
            AddCurrentHealth(-finalLoss);

        Debug.Log(
            $"[Item] 학교 체력 감소: {baseLoss} → {finalLoss}"
        );
    }

    public float GetConfessionChanceWithBonus(
    float baseChance)
    {
        if (CurrentPlayer == null)
            return Mathf.Clamp01(baseChance);

        float bonus =
            CurrentPlayer.confessionBonusPercent / 100f;

        return Mathf.Clamp01(baseChance + bonus);
    }
    public void LoadGame()
    {
        SaveGameData data =
            SaveSystem.Load();

        if (data == null)
        {
            Debug.LogWarning(
                "[Save] 저장 데이터를 불러올 수 없습니다."
            );

            return;
        }

        ApplySaveData(data);

        SceneTransitionManager.Instance.LoadScene(
            "MorningScene"
        );
    }
    public bool HasSavedGame()
    {
        return SaveSystem.HasSave();
    }


    public void AbandonCurrentRun()
    {
        // 일시정지 해제
        Time.timeScale = 1f;

        // 현재 회차 데이터 초기화
        CurrentPlayer = null;
        CurrentWeek = 1;
        CurrentSchoolEntry = SchoolEntryType.None;

        StudiedToday = false;
        WorkedToday = false;

        ClearSavedSchoolPlayerPosition();
        ClearSchoolHealthDrainPauses();
        ResetDailyNPCUsage();

        // 기존 이어하기 저장 삭제
        SaveSystem.DeleteSave();

        // 메인 메뉴로 이동
        SceneTransitionManager.Instance.LoadScene("MainScene");

        Debug.Log("[Game] 현재 회차를 포기했습니다.");
    }


    private void GiveWeeklyAllowanceIfNeeded()
    {
        if (CurrentPlayer == null)
            return;

        if (CurrentWeek < 1 || CurrentWeek > 16)
            return;

        int bonus = CurrentPlayer.weeklyMoneyBonus;

        if (bonus <= 0)
            return;

        // 같은 주차 중복 지급 방지
        if (CurrentPlayer.lastAllowanceWeek >= CurrentWeek)
            return;

        // 지급 주차를 먼저 기록
        CurrentPlayer.lastAllowanceWeek = CurrentWeek;

        AddMoney(bonus);

        Debug.Log(
            $"[Item] {CurrentWeek}주차 엄마의 용돈 +{bonus}"
        );
    }



}


