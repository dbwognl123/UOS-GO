using System;

[Serializable]
public class PlayerRunData
{
    public int happiness;      // 행복도
    public int campusLife;     // 학교생활참여도
    public int intelligence;   // 지능
    public int maxHealth;      // 최대체력
    public int currentHealth;  // 현재체력(학교 활동용)
    public int money;          // 돈
    public bool hasGirlfriend; // 여자친구 유무
    public int appearance;     // 외모
    public int grade;          // 학점

    public int romanceAffection;


    public string activeCharacterId = "DEFAULT";

    public CharacterAbilityData activeCharacterAbility =
        new CharacterAbilityData();

    public int schoolHealthLossReduction = 0;
    public int confessionBonusPercent = 0;
    public int weeklyMoneyBonus = 0;

    // 중복 지급 방지용
    public int lastAllowanceWeek = 0;

}