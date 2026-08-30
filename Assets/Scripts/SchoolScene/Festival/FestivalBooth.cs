using UnityEngine;

public class FestivalBooth : MonoBehaviour
{
    [SerializeField] private string boothId;
    [SerializeField] private FestivalBoothType boothType;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private bool playerInside;

    private void Update()
    {
        if (!playerInside)
            return;

        if (Input.GetKeyDown(interactKey))
            Interact();
    }

    private void Interact()
    {
        GameManager gm = GameManager.Instance;

        if (gm == null)
            return;

        if (!gm.festivalDateStarted ||
            gm.festivalDateFinished)
            return;

        if (gm.festivalDatePhase != FestivalDatePhase.Booth)
            return;

        // 지금 데이트 목표와 다른 부스
        if (boothType != gm.targetFestivalBoothType)
        {
            Debug.Log("여자: 여기는 우리 과 부스 아닌데? ㅋㅋ");
            return;
        }

        gm.VisitFestivalBooth(
            boothId,
            boothType
        );
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = false;
    }
}