
using UnityEngine;

public static class CharacterEffectCalculator
{
    private static int ApplyMultiplier(
        int amount,
        float multiplier)
    {
        // 감소량에는 배율을 적용하지 않음
        if (amount <= 0)
            return amount;

        return Mathf.CeilToInt(
            amount * Mathf.Max(0f, multiplier)
        );
    }

    // 운동 시 최대체력 증가
    public static int GetExerciseHealthGain(
        int amount,
        CharacterAbilityData ability)
    {
        float multiplier = ability != null
            ? ability.exerciseHealthMultiplier : 1f;

        return ApplyMultiplier(amount, multiplier);
    }

    // 지능 증가
    public static int GetIntelligenceGain(
        int amount,
        CharacterAbilityData ability)
    {
        float multiplier = ability != null
            ? ability.intelligenceMultiplier : 1f;

        return ApplyMultiplier(amount, multiplier);
    }

   

    // 학교생활력 증가 배율 적용
    public static int GetCampusLifeGain(
        int amount,
        CharacterAbilityData ability)
    {
        float multiplier = ability != null
            ? ability.campusLifeMultiplier
            : 1f;

        return ApplyMultiplier(amount, multiplier);
    }

}
