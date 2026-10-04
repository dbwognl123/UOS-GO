using UnityEngine;
using UnityEngine.SceneManagement;

public class ClassroomTrigger : MonoBehaviour
{
    [Header("Building")]
    [SerializeField] private int classroomNumber;

    [Header("Interaction")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private GameObject interactHint;

    [Header("Wrong Building Dialogue")]
    [SerializeField] private string speakerName = "나";
    [SerializeField] private Sprite speakerPortrait;

    private bool playerInRange;
    private Transform playerTransform;

    private void Awake()
    {
        if (interactHint != null)
            interactHint.SetActive(false);
    }

    private void Update()
    {
        if (!playerInRange)
            return;

        if (playerTransform == null)
            return;

        if (!Input.GetKeyDown(interactKey))
            return;

        TryInteract();
    }

    private void TryInteract()
    {
        GameManager gm = GameManager.Instance;

        if (gm == null)
            return;

        if (gm.IsDialogueOpen)
            return;

        ClassEnterResult result =
            gm.TryEnterClassroom(
                classroomNumber,
                playerTransform.position
            );

        switch (result)
        {
            case ClassEnterResult.WrongClass:
                ShowWrongBuildingMessage(gm);
                break;

            case ClassEnterResult.AttendanceOnly:
                {
                    int nextClassroom =
                        gm.GetCurrentTargetClassroom();

                    string nextBuildingName =
                        gm.GetClassroomName(nextClassroom);

                    if (SchoolNPCUI.Instance != null)
                    {
                        SchoolNPCUI.Instance.OpenSimpleDialogue(
                            speakerName,
                            speakerPortrait,
                            $"이제 {nextBuildingName}으로 가자."
                        );
                    }

                    break;
                }

            case ClassEnterResult.StartFinalMinigame:
                Debug.Log(
                    "마지막 수업입니다. 미니게임 시작."
                );

                SceneManager.LoadScene("ClassScene");
                break;

            case ClassEnterResult.AlreadyFinished:
                if (SchoolNPCUI.Instance != null)
                {
                    SchoolNPCUI.Instance.OpenSimpleDialogue(
                        speakerName,
                        speakerPortrait,
                        "오늘 수업은 다 끝났어."
                    );
                }

                break;
        }
    }

    private void ShowWrongBuildingMessage(GameManager gm)
    {
        int targetClassroom =
            gm.GetCurrentTargetClassroom();

        string currentBuildingName =
            gm.GetClassroomName(classroomNumber);

        string targetBuildingName =
            gm.GetClassroomName(targetClassroom);

        string message =
            $"여긴 {currentBuildingName}이야.\n" +
            $"지금은 {targetBuildingName}으로 가야 돼.";

        if (SchoolNPCUI.Instance != null)
        {
            SchoolNPCUI.Instance.OpenSimpleDialogue(
                speakerName,
                speakerPortrait,
                message
            );
        }
        else
        {
            Debug.Log(message);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = true;
        playerTransform = other.transform;

        if (interactHint != null)
            interactHint.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = false;
        playerTransform = null;

        if (interactHint != null)
            interactHint.SetActive(false);
    }
}