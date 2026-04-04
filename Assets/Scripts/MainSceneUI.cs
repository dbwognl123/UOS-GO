using UnityEngine;

public class MainSceneUI : MonoBehaviour
{
    public void OnClickStart()
    {
        GameManager.Instance.StartNewGame();
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