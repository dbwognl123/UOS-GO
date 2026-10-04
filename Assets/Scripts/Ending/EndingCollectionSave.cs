using UnityEngine;

public static class EndingCollectionSave
{
    private const string UnlockPrefix = "ENDING_UNLOCK_";
    private const string NewPrefix = "ENDING_NEW_";

    public static bool IsUnlocked(string endingKey)
    {
        if (string.IsNullOrEmpty(endingKey))
            return false;

        return PlayerPrefs.GetInt(
            UnlockPrefix + endingKey,
            0
        ) == 1;
    }

    public static void Unlock(string endingKey)
    {
        if (string.IsNullOrEmpty(endingKey))
            return;

        // 처음 해금한 경우에만 NEW 표시
        if (!IsUnlocked(endingKey))
        {
            PlayerPrefs.SetInt(
                NewPrefix + endingKey,
                1
            );
        }

        PlayerPrefs.SetInt(
            UnlockPrefix + endingKey,
            1
        );

        PlayerPrefs.Save();

        Debug.Log(
            $"[Collection] 엔딩 해금: {endingKey}"
        );
    }

    public static bool IsNew(string endingKey)
    {
        if (string.IsNullOrEmpty(endingKey))
            return false;

        return PlayerPrefs.GetInt(
            NewPrefix + endingKey,
            0
        ) == 1;
    }

    public static void MarkAsSeen(string endingKey)
    {
        if (string.IsNullOrEmpty(endingKey))
            return;

        PlayerPrefs.SetInt(
            NewPrefix + endingKey,
            0
        );

        PlayerPrefs.Save();
    }

#if UNITY_EDITOR
    public static void DebugResetAll()
    {
        string[] allKeys =
        {
            "A_GoodGrade",
            "A_NormalGrade",
            "A_BadGrade",

            "B_GoodCampusLife",
            "B_NormalCampusLife",
            "B_BadCampusLife",

            "C_NoGirlfriend",
            "C_HasGirlfriend",

            "D_Happy",
            "D_Unhappy",

            "E_BestFriend",

            "F_GraduateSchool"
        };

        foreach (string key in allKeys)
        {
            PlayerPrefs.DeleteKey(
                UnlockPrefix + key
            );

            PlayerPrefs.DeleteKey(
                NewPrefix + key
            );
        }

        PlayerPrefs.Save();

        Debug.Log(
            "[Collection] 모든 엔딩 컬렉션 초기화"
        );
    }
#endif
}