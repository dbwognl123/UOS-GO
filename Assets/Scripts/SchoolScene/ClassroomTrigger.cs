using UnityEngine;

public class ClassroomTrigger : MonoBehaviour
{
    [SerializeField] private int classroomNumber;
    [SerializeField] private Vector3 returnOffset = new Vector3(0f, -0.8f, 0f);
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (GameManager.Instance == null) return;

        if (GameManager.Instance.CanEnterClassroom(classroomNumber))
        {
            Vector3 returnPosition = other.transform.position + returnOffset;
            GameManager.Instance.EnterClassScene(classroomNumber, other.transform.position);
        }
        else
        {
            Debug.Log($"지금은 {GameManager.Instance.GetCurrentTargetClassroom()} 강의실로 가야 합니다.");
        }
    }
}