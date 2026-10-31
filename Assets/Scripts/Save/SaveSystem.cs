using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private const string SaveFileName =
        "SaveGame.json";

    private static string SavePath
    {
        get
        {
            return Path.Combine(
                Application.persistentDataPath,
                SaveFileName
            );
        }
    }

    public static bool HasSave()
    {
        return File.Exists(SavePath);
    }

    public static void Save(
        SaveGameData data)
    {
        if (data == null)
            return;

        try
        {
            string json =
                JsonUtility.ToJson(
                    data,
                    true
                );

            File.WriteAllText(
                SavePath,
                json
            );

            Debug.Log(
                $"[Save] 저장 완료\n{SavePath}"
            );
        }
        catch (System.Exception e)
        {
            Debug.LogError(
                $"[Save] 저장 실패\n{e}"
            );
        }
    }

    public static SaveGameData Load()
    {
        if (!HasSave())
        {
            Debug.LogWarning(
                "[Save] 저장 파일이 없습니다."
            );

            return null;
        }

        try
        {
            string json =
                File.ReadAllText(
                    SavePath
                );

            SaveGameData data =
                JsonUtility.FromJson<SaveGameData>(
                    json
                );

            Debug.Log(
                $"[Save] 불러오기 완료 / " +
                $"Week={data.currentWeek}"
            );

            return data;
        }
        catch (System.Exception e)
        {
            Debug.LogError(
                $"[Save] 불러오기 실패\n{e}"
            );

            return null;
        }
    }

    public static void DeleteSave()
    {
        if (!HasSave())
            return;

        try
        {
            File.Delete(SavePath);

            Debug.Log(
                "[Save] 진행 세이브 삭제"
            );
        }
        catch (System.Exception e)
        {
            Debug.LogError(
                $"[Save] 세이브 삭제 실패\n{e}"
            );
        }
    }

    public static string GetSavePath()
    {
        return SavePath;
    }
}