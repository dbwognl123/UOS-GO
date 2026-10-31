
using System;
using System.Collections.Generic;

[Serializable]
public class OwnedConsumableEntry
{
    public string productId;
    public int count;
}

[Serializable]
public class EquippedConsumableEntry
{
    public string productId;
    public StartBonusData bonus = new StartBonusData();
}

[Serializable]
public class MetaProgressData
{
    public int version = 4;

    public int coin = 0;

    // 캐릭터: 영구 구매 / 하나만 착용
    public List<string> unlockedCharacterIds =
        new List<string>();

    public string selectedCharacterId = "DEFAULT";

    // 소모품: ID별 보유 수량
    public List<OwnedConsumableEntry> ownedConsumables =
        new List<OwnedConsumableEntry>();

    // 여러 종류를 동시에 장착
    public List<EquippedConsumableEntry> equippedConsumables =
        new List<EquippedConsumableEntry>();

    // 이전 버전 저장 데이터 호환용
    // 새로운 코드에서는 직접 사용하지 않음
    public string selectedConsumableId = "";
    public StartBonusData selectedConsumableBonus =
        new StartBonusData();

    public int startMoneyPlus10Count = 0;
    public int startIntelligencePlus10Count = 0;
    public int startMaxHealthPlus10Count = 0;

    public CharacterAbilityData selectedCharacterAbility =
    new CharacterAbilityData();
    // 장착한 캐릭터의 시작 보너스
    public StartBonusData selectedCharacterStartBonus =
        new StartBonusData();
}
