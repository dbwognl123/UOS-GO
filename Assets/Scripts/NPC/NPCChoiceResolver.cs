using UnityEngine;

public static class NPCChoiceResolver
{
    public static bool CanChoose(PlayerRunData player, NPCChoiceData choice, out string reason)
    {
        if (player.money < choice.requiredMoney)
        {
            reason = "돈이 부족합니다.";
            return false;
        }

        if (player.appearance < choice.minAppearance)
        {
            reason = "외모가 부족합니다.";
            return false;
        }

        if (player.campusLife < choice.minCampusLife)
        {
            reason = "학교생활력이 부족합니다.";
            return false;
        }

        if (player.intelligence < choice.minIntelligence)
        {
            reason = "지능이 부족합니다.";
            return false;
        }

        if (choice.blockIfHasGirlfriend && player.hasGirlfriend)
        {
            reason = "이미 연애 중입니다.";
            return false;
        }

        reason = string.Empty;
        return true;
    }

    public static float EvaluateSuccessChance(PlayerRunData player, NPCChoiceData choice)
    {
        float chance = choice.baseChance;
        chance += player.appearance * choice.appearanceWeight;
        chance += player.campusLife * choice.campusLifeWeight;
        chance += player.intelligence * choice.intelligenceWeight;

        return Mathf.Clamp(chance, choice.minChance, choice.maxChance);
    }
}