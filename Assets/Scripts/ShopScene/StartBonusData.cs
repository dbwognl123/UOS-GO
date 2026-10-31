
using System;
using UnityEngine;

[Serializable]
public class StartBonusData
{
    [Header("게임 시작 능력치")]
    public int money = 0;
    public int intelligence = 0;
    public int maxHealth = 0;
    public int happiness = 0;
    public int campusLife = 0;
    public int appearance = 0;

    [Header("학교씬 체력 감소 완화")]
    [Min(0)]
    public int schoolHealthLossReduction = 0;

    [Header("고백 성공 확률 보너스 (%p)")]
    [Range(0, 100)]
    public int confessionBonusPercent = 0;

    [Header("주차마다 추가 지급할 돈")]
    [Min(0)]
    public int weeklyMoneyBonus = 0;
}
