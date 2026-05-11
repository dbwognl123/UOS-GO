using UnityEngine;
using UnityEngine.SceneManagement;

public class ClassroomTrigger : MonoBehaviour
{
    [SerializeField] private int classroomNumber;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (GameManager.Instance == null) return;

        ClassEnterResult result = GameManager.Instance.TryEnterClassroom(
    classroomNumber,
    other.transform.position
);
        switch (result)
        {
            case ClassEnterResult.WrongClass:
                Debug.Log("지금 가야 할 강의실이 아닙니다.");
                break;

            case ClassEnterResult.AttendanceOnly:
                Debug.Log("출석 처리됨. 다음 강의실로 이동.");
                // 필요하면 여기서 작은 팝업:
                // "출석 완료! 다음 강의실: XXX"
                break;

            case ClassEnterResult.StartFinalMinigame:
                Debug.Log("마지막 수업입니다. 미니게임 시작.");
                SceneManager.LoadScene("ClassScene");
                break;

            case ClassEnterResult.AlreadyFinished:
                Debug.Log("오늘 수업은 이미 끝났습니다.");
                break;
        }
    }
}