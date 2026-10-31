using UnityEngine;

public class MainSceneUI : MonoBehaviour
{
    public void OnClickStart()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning(
                "[Main] GameManager가 없습니다."
            );

            return;
        }

        // 기존 진행 데이터가 있으면 이어하기
        if (SaveSystem.HasSave())
        {
            Debug.Log(
                "[Main] 저장된 게임을 불러옵니다."
            );

            GameManager.Instance.LoadGame();
        }
        // 저장 데이터가 없으면 새 게임
        else
        {
            Debug.Log(
                "[Main] 저장 데이터가 없습니다. 새 게임을 시작합니다."
            );

            GameManager.Instance.StartNewGame();
        }
    }

    public void OnClickShop()
    {
        GameManager.Instance.GoToShop();
    }

    public void OnClickCollection()
    {
        GameManager.Instance.GoToCollection();
    }
}