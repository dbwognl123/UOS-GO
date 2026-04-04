using UnityEngine;

public class ClassroomTrigger : MonoBehaviour
{
    [SerializeField] private int classroomNumber;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (GameManager.Instance == null) return;

        if (GameManager.Instance.CanEnterClassroom(classroomNumber))
        {
            GameManager.Instance.EnterClassScene(classroomNumber, other.transform.position);
        }
        else
        {
            Debug.Log($"지금은 {GameManager.Instance.GetCurrentTargetClassroom()} 강의실로 가야 합니다.");
        }
    }
}