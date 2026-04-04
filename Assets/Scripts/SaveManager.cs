using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    public GlobalSaveData GlobalData { get; private set; } = new GlobalSaveData();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddGold(int amount)
    {
        GlobalData.gold += amount;
    }

    public bool HasEnding(string endingId)
    {
        return GlobalData.unlockedEndings.Contains(endingId);
    }

    public void UnlockEnding(string endingId)
    {
        if (!GlobalData.unlockedEndings.Contains(endingId))
            GlobalData.unlockedEndings.Add(endingId);
    }
}