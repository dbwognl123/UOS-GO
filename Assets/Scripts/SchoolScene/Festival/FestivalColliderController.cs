
using UnityEngine;
using UnityEngine.Tilemaps;

public class FestivalColliderController : MonoBehaviour
{
    [Header("구조물 타일맵 콜리더")]
    [SerializeField]
    private TilemapCollider2D structureCollider;

    private void Start()
    {
        UpdateCollider();
    }

    private void UpdateCollider()
    {
        if (GameManager.Instance == null ||
            structureCollider == null)
            return;

        // 10주차는 축제
        bool isFestivalWeek =
            GameManager.Instance.CurrentWeek == 10;

        // 축제 기간에는 구조물 충돌 비활성화
        structureCollider.enabled = !isFestivalWeek;

        Debug.Log(
            $"[FestivalCollider] " +
            $"Week={GameManager.Instance.CurrentWeek}, " +
            $"Collider Enabled={structureCollider.enabled}"
        );
    }
}
