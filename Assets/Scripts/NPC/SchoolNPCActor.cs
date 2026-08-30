using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SchoolNPCActor : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private NPCEncounterSO encounterData;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer worldSpriteRenderer;
    [SerializeField] private GameObject interactHint;

    private bool playerInRange;
    private bool isConsumed;

    public NPCEncounterSO EncounterData => encounterData;

    public void Setup(NPCEncounterSO data)
    {
        // Spawner가 넘겨준 축제 전용 Encounter를 저장
        encounterData = data;

        // 중요:
        // portrait는 대화창용 이미지이므로
        // 월드 SpriteRenderer를 변경하지 않는다.

        Debug.Log(
            $"[SchoolNPCActor] Setup 완료 / " +
            $"Encounter={(encounterData != null ? encounterData.name : "NULL")}"
        );
    }

    public void Consume()
    {
        isConsumed = true;

        if (interactHint != null)
            interactHint.SetActive(false);

        gameObject.SetActive(false);
    }

    private void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;

        if (interactHint != null)
            interactHint.SetActive(false);
    }

    private string GetAlreadyTalkedLine(SchoolNPCType npcType)
    {
        switch (npcType)
        {
            case SchoolNPCType.Professor:
                return "모르는 게 있으면 또 물어보러 오세요~";

            case SchoolNPCType.Senior:
                return "필요한 거 있으면 또 연락해~";

            case SchoolNPCType.Friend:
                return "ㅎㅇ?";

            case SchoolNPCType.Romance:
                return "안녕하세요 ㅎㅎ";

            default:
                return "안녕";
        }
    }

    private void Update()
    {
        if (isConsumed)
            return;

        if (!playerInRange)
            return;

        if (encounterData == null)
            return;

        if (!Input.GetKeyDown(KeyCode.E))
            return;

        if (SchoolNPCUI.Instance == null)
        {
            Debug.LogWarning("[SchoolNPCActor] SchoolNPCUI.Instance가 없습니다.");
            return;
        }

        GameManager gm = GameManager.Instance;

        if (gm == null)
            return;

        // ======================================
        // 10주차 축제 데이트 여자 NPC
        // ======================================
        if (gm.CurrentWeek == 10 &&
            gm.hasFestivalDatePromise &&
            encounterData.npcType == SchoolNPCType.Romance)
        {
            if (!gm.festivalDateStarted)
            {
                Debug.Log("[Festival] 여자 NPC와 첫 상호작용");

                gm.StartFestivalDate();
                return;
            }
        }

        // ======================================
        // 기존 NPC 처리
        // ======================================
        if (gm.IsNPCStageUsedToday(
                encounterData.npcType,
                encounterData.stageIndex))
        {
            string alreadyTalkedLine =
                GetAlreadyTalkedLine(encounterData.npcType);

            SchoolNPCUI.Instance.OpenSimpleDialogue(
                encounterData.npcName,
                encounterData.portrait,
                alreadyTalkedLine
            );
        }
        else
        {
            SchoolNPCUI.Instance.OpenDialogue(this);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isConsumed)
            return;

        if (!other.CompareTag("Player"))
            return;

        playerInRange = true;

        if (interactHint != null)
            interactHint.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = false;

        if (interactHint != null)
            interactHint.SetActive(false);
    }
}