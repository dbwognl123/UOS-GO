
using System;
using UnityEngine;

[Serializable]
public class CharacterAbilityData
{
    [Header("운동 최대체력 증가 배율")]
    [Min(0f)]
    public float exerciseHealthMultiplier = 1f;

    [Header("지능 증가 배율")]
    [Min(0f)]
    public float intelligenceMultiplier = 1f;

    [Header("학교생활력 증가 배율")]
    [Min(0f)]
    public float campusLifeMultiplier = 1f;
}
