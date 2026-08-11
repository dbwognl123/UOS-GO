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
        encounterData = data;

        if (worldSpriteRenderer != null && data != null && data.portrait != null)
            worldSpriteRenderer.sprite = data.portrait;
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
        if (isConsumed) return;
        if (!playerInRange) return;
        if (encounterData == null) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (SchoolNPCUI.Instance == null) return;

            if (GameManager.Instance != null &&
    GameManager.Instance.IsNPCStageUsedToday(
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
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isConsumed) return;
        if (!other.CompareTag("Player")) return;

        playerInRange = true;

        if (interactHint != null)
            interactHint.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;

        if (interactHint != null)
            interactHint.SetActive(false);
    }
}