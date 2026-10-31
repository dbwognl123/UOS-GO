
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class MetaProgressSystem
{
    private const string FileName = "MetaProgress.json";

    private static MetaProgressData data;

    private static string SavePath =>
        Path.Combine(
            Application.persistentDataPath,
            FileName
        );

    public static MetaProgressData Data
    {
        get
        {
            EnsureLoaded();
            return data;
        }
    }

    // =========================
    // 저장 / 불러오기
    // =========================

    private static void EnsureLoaded()
    {
        if (data != null)
            return;

        bool changed = false;

        try
        {
            if (File.Exists(SavePath))
            {
                string json = File.ReadAllText(SavePath);
                data = JsonUtility.FromJson<MetaProgressData>(json);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[Meta] 불러오기 실패: {e}");
        }

        if (data == null)
        {
            data = new MetaProgressData();
            changed = true;
        }

        if (data.unlockedCharacterIds == null)
        {
            data.unlockedCharacterIds = new List<string>();
            changed = true;
        }

        if (data.ownedConsumables == null)
        {
            data.ownedConsumables =
                new List<OwnedConsumableEntry>();

            changed = true;
        }

        if (data.equippedConsumables == null)
        {
            data.equippedConsumables =
                new List<EquippedConsumableEntry>();

            changed = true;
        }

        if (data.selectedCharacterStartBonus == null)
        {
            data.selectedCharacterStartBonus =
                new StartBonusData();

            changed = true;
        }
        // 버전 2 이전: 개별 보유 수량 -> 리스트
        if (data.version < 3)
        {
            AddOwnedInternal(
                "START_MONEY_10",
                data.startMoneyPlus10Count
            );

            AddOwnedInternal(
                "START_INTELLIGENCE_10",
                data.startIntelligencePlus10Count
            );

            AddOwnedInternal(
                "START_HEALTH_10",
                data.startMaxHealthPlus10Count
            );

            data.startMoneyPlus10Count = 0;
            data.startIntelligencePlus10Count = 0;
            data.startMaxHealthPlus10Count = 0;

            changed = true;
        }

        // 버전 3: 기존 단일 장착 -> 다중 장착 리스트
        if (data.version < 4)
        {
            if (data.version >= 3 &&
                !string.IsNullOrEmpty(data.selectedConsumableId))
            {
                OwnedConsumableEntry owned =
                    FindOwnedInternal(data.selectedConsumableId);

                if (owned != null && owned.count > 0)
                {
                    data.equippedConsumables.Add(
                        new EquippedConsumableEntry
                        {
                            productId = data.selectedConsumableId,
                            bonus = CopyBonus(
                                data.selectedConsumableBonus
                            )
                        }
                    );
                }
            }

            data.selectedConsumableId = "";
            data.selectedConsumableBonus = new StartBonusData();

            data.version = 4;
            changed = true;
        }

        if (changed)
            Save();
    }

    public static void Save()
    {
        if (data == null)
            data = new MetaProgressData();

        try
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(SavePath, json);
        }
        catch (Exception e)
        {
            Debug.LogError($"[Meta] 저장 실패: {e}");
        }
    }

    // =========================
    // Coin
    // =========================

    public static int GetCoin()
    {
        return Data.coin;
    }

    public static void AddCoin(int amount)
    {
        if (amount <= 0)
            return;

        Data.coin += amount;
        Save();
    }

    public static bool TrySpendCoin(int amount)
    {
        if (amount <= 0 || Data.coin < amount)
            return false;

        Data.coin -= amount;
        Save();

        return true;
    }

    // =========================
    // 캐릭터
    // =========================

    public static bool IsCharacterUnlocked(string id)
    {
        if (string.IsNullOrEmpty(id))
            return false;

        return id == "DEFAULT" ||
               Data.unlockedCharacterIds.Contains(id);
    }

    public static bool BuyCharacter(string id, int price)
    {
        if (string.IsNullOrEmpty(id) ||
            price <= 0 ||
            IsCharacterUnlocked(id) ||
            Data.coin < price)
            return false;

        Data.coin -= price;
        Data.unlockedCharacterIds.Add(id);

        Save();

        return true;
    }

    public static bool SelectCharacter(string id)
    {
        if (!IsCharacterUnlocked(id))
            return false;

        Data.selectedCharacterId = id;
        Save();

        return true;
    }

    public static string GetSelectedCharacterId()
    {
        return Data.selectedCharacterId;
    }

    // =========================
    // 소모품 인벤토리
    // =========================
    public static StartBonusData GetSelectedCharacterStartBonus()
    {
        return CopyBonus(
            Data.selectedCharacterStartBonus
        );
    }
    private static OwnedConsumableEntry FindOwnedInternal(
        string id)
    {
        return data.ownedConsumables.Find(
            entry => entry != null &&
                     entry.productId == id
        );
    }

    private static void AddOwnedInternal(
        string id,
        int amount)
    {
        if (string.IsNullOrEmpty(id) || amount <= 0)
            return;

        OwnedConsumableEntry entry =
            FindOwnedInternal(id);

        if (entry == null)
        {
            entry = new OwnedConsumableEntry
            {
                productId = id,
                count = 0
            };

            data.ownedConsumables.Add(entry);
        }

        entry.count += amount;
    }

    public static int GetConsumableCount(string id)
    {
        EnsureLoaded();

        OwnedConsumableEntry entry =
            FindOwnedInternal(id);

        return entry != null ? entry.count : 0;
    }

    public static bool HasConsumable(string id)
    {
        return GetConsumableCount(id) > 0;
    }

    public static bool BuyConsumable(
        string id,
        int price)
    {
        if (string.IsNullOrEmpty(id) ||
            price <= 0 ||
            Data.coin < price)
            return false;

        Data.coin -= price;

        AddOwnedInternal(id, 1);
        Save();

        Debug.Log($"[Meta] 소모품 구매: {id}");

        return true;
    }

    // =========================
    // 여러 소모품 동시 장착
    // =========================

    public static bool IsConsumableSelected(string id)
    {
        if (string.IsNullOrEmpty(id))
            return false;

        return Data.equippedConsumables.Exists(
            entry => entry != null &&
                     entry.productId == id
        );
    }


    private static StartBonusData CopyBonus(
        StartBonusData source)
    {
        if (source == null)
            return new StartBonusData();

        return new StartBonusData
        {
            money = source.money,
            intelligence = source.intelligence,
            maxHealth = source.maxHealth,
            happiness = source.happiness,
            campusLife = source.campusLife,
            appearance = source.appearance,

            schoolHealthLossReduction =
                source.schoolHealthLossReduction,

            confessionBonusPercent =
                source.confessionBonusPercent,

            weeklyMoneyBonus =
                source.weeklyMoneyBonus
        };
    }


    public static bool SelectConsumable(
        MetaShopProductData product)
    {
        if (product == null ||
            product.category != ShopCategory.Consumable)
            return false;

        string id = product.productId;

        if (string.IsNullOrEmpty(id))
            return false;

        // 이미 장착했으면 해당 아이템만 해제
        if (IsConsumableSelected(id))
        {
            Data.equippedConsumables.RemoveAll(
                entry => entry == null ||
                         entry.productId == id
            );

            Save();

            Debug.Log($"[Meta] 소모품 장착 해제: {id}");

            return true;
        }

        // 보유 아이템만 장착 가능
        if (!HasConsumable(id))
            return false;

        // 종류별로 한 개만 장착
        Data.equippedConsumables.Add(
            new EquippedConsumableEntry
            {
                productId = id,
                bonus = CopyBonus(product.startBonus)
            }
        );

        Save();

        Debug.Log($"[Meta] 소모품 장착: {id}");

        return true;
    }

    // =========================
    // 새 게임 시작 시 모두 소비
    // =========================

    public static StartBonusData ConsumeSelectedBonus()
    {
        StartBonusData totalBonus =
            new StartBonusData();

        HashSet<string> consumedIds =
            new HashSet<string>();

        foreach (EquippedConsumableEntry equipped
                 in Data.equippedConsumables)
        {
            if (equipped == null ||
                string.IsNullOrEmpty(equipped.productId))
                continue;

            string id = equipped.productId;

            // 같은 종류는 한 번만 처리
            if (!consumedIds.Add(id))
                continue;

            OwnedConsumableEntry owned =
                FindOwnedInternal(id);

            if (owned == null || owned.count <= 0)
                continue;

            // 해당 종류의 보유 수량 -1
            owned.count--;

            StartBonusData bonus = equipped.bonus;

            if (bonus != null)
            {
                totalBonus.money += bonus.money;
                totalBonus.intelligence += bonus.intelligence;
                totalBonus.maxHealth += bonus.maxHealth;
                totalBonus.happiness += bonus.happiness;
                totalBonus.campusLife += bonus.campusLife;
                totalBonus.appearance += bonus.appearance;
                totalBonus.schoolHealthLossReduction +=
                    bonus.schoolHealthLossReduction;

                totalBonus.confessionBonusPercent +=
                    bonus.confessionBonusPercent;

                totalBonus.weeklyMoneyBonus +=
                    bonus.weeklyMoneyBonus;
            }

            Debug.Log($"[Meta] 소모품 소비: {id}");
        }

        // 이번 회차용 장착 목록 비우기
        Data.equippedConsumables.Clear();

        Save();

        return totalBonus;
    }



    private static CharacterAbilityData CopyCharacterAbility(
        CharacterAbilityData source)
    {
        if (source == null)
            return new CharacterAbilityData();

        return new CharacterAbilityData
        {
            exerciseHealthMultiplier =
                source.exerciseHealthMultiplier,

            intelligenceMultiplier =
                source.intelligenceMultiplier,

            campusLifeMultiplier =
                source.campusLifeMultiplier
        };
    }


    public static CharacterAbilityData GetSelectedCharacterAbility()
    {
        return CopyCharacterAbility(
            Data.selectedCharacterAbility
        );
    }

    // 상품 데이터까지 받아 캐릭터와 고유 능력을 함께 저장

    public static bool SelectCharacter(
        MetaShopProductData product)
    {
        if (product == null ||
            product.category != ShopCategory.Character)
            return false;

        if (!IsCharacterUnlocked(product.productId))
            return false;

        // 캐릭터 ID 저장
        Data.selectedCharacterId = product.productId;

        // 게임 진행 중 획득 배율 저장
        Data.selectedCharacterAbility =
            CopyCharacterAbility(product.characterAbility);

        // 새 게임 시작 시 적용할 초기 스탯 저장
        Data.selectedCharacterStartBonus =
            CopyBonus(product.startBonus);

        Save();

        Debug.Log(
            $"[Meta] 캐릭터 선택: {product.productId}, " +
            $"시작 최대체력 보너스: " +
            $"{Data.selectedCharacterStartBonus.maxHealth}"
        );

        return true;
    }

}
