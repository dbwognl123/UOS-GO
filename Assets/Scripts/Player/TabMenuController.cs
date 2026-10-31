
using UnityEngine;

public class TabMenuController : MonoBehaviour
{
    public void OnClickGiveUp()
    {
        GameManager.Instance?.AbandonCurrentRun();
    }
}
