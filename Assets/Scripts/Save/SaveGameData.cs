using System;
using System.Collections.Generic;

[Serializable]
public class SaveGameData
{
    public int saveVersion = 1;

    // =========================
    // 기본 진행
    // =========================
    public int currentWeek = 1;

    // =========================
    // 플레이어
    // =========================
    public int happiness;
    public int campusLife;
    public int intelligence;

    public int maxHealth;
    public int currentHealth;

    public int money;
    public int appearance;
    public int grade;

    public bool hasGirlfriend;
    public int romanceAffection;

    // =========================
    // 학점
    // =========================
    public float regularEarned;
    public float regularPossible;

    public int midtermScore;
    public int finalScore;

    public int absenceCount;
    public int lastAbsenceWeek;

    // =========================
    // NPC 진행
    // =========================
    public int friendCurrentStage;
    public int friendStageCap;
    public bool friendRetired;

    public int seniorCurrentStage;
    public int seniorStageCap;
    public bool seniorRetired;

    public int professorCurrentStage;
    public int professorStageCap;
    public bool professorRetired;
    public int professorQuestionCount;

    public int romanceCurrentStage;
    public int romanceStageCap;
    public bool romanceRetired;

    // =========================
    // 미팅 / 연애
    // =========================
    public int femaleFriendCount;
    public int perfectMeetingSuccessCount;

    public bool romanceNpc1Unlocked;

    // =========================
    // 축제
    // =========================
    public bool hasFestivalDatePromise;

    public bool festivalDateStarted;
    public bool festivalDateFinished;
    public bool festivalDateEndedByLowHealth;

    // =========================
    // 엔딩 조건
    // =========================
    public bool endingACutUnlocked;
    public bool endingGraduateSchoolUnlocked;

    // =========================
    // 오늘의 시간표
    // =========================
    public List<int> todaySchedule =
        new List<int>();


    // 이번 회차의 캐릭터
    public string activeCharacterId = "DEFAULT";

    public CharacterAbilityData activeCharacterAbility =
        new CharacterAbilityData();

    // 이번 회차의 지속 아이템 효과
    public int schoolHealthLossReduction = 0;
    public int confessionBonusPercent = 0;
    public int weeklyMoneyBonus = 0;

    // 마지막 용돈 지급 주차
    public int lastAllowanceWeek = 0;

}