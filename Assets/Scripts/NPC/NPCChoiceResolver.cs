using UnityEngine;

public static class NPCChoiceResolver
{
    public static bool CanChoose(PlayerRunData player, NPCChoiceData choice, out string reason)
    {
        int neededMoney = choice.requiredMoney;

        if (choice.successMoneyDelta < 0)
            neededMoney = Mathf.Max(neededMoney, -choice.successMoneyDelta);

        if (choice.failMoneyDelta < 0)
            neededMoney = Mathf.Max(neededMoney, -choice.failMoneyDelta);

        if (player.money < neededMoney)
        {
            reason = $"돈이 부족합니다. (필요: {neededMoney})";
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
        if (choice.autoFailIfAppearanceBelow > 0 && player.appearance < choice.autoFailIfAppearanceBelow)
            return 0f;

        if (choice.autoFailIfCampusLifeBelow > 0 && player.campusLife < choice.autoFailIfCampusLifeBelow)
            return 0f;

        if (choice.autoFailIfIntelligenceBelow > 0 && player.intelligence < choice.autoFailIfIntelligenceBelow)
            return 0f;

        if (choice.useFixedChance)
            return Mathf.Clamp(choice.fixedSuccessChance, 0f, 100f);

        float chance = choice.baseChance;
        chance += player.appearance * choice.appearanceWeight;
        chance += player.campusLife * choice.campusLifeWeight;
        chance += player.intelligence * choice.intelligenceWeight;

        return Mathf.Clamp(chance, choice.minChance, choice.maxChance);
    }
}