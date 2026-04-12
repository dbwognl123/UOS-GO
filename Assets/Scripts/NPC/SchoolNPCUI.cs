using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SchoolNPCUI : MonoBehaviour
{
    public static SchoolNPCUI Instance { get; private set; }

    [SerializeField] private GameObject rootPanel;
    [SerializeField] private Image portraitImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text dialogueText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (rootPanel != null)
            rootPanel.SetActive(false);
    }

    public void OpenDialogue(SchoolNPCActor actor)
    {
        if (actor == null || actor.EncounterData == null) return;

        var data = actor.EncounterData;

        if (rootPanel != null)
            rootPanel.SetActive(true);

        if (portraitImage != null)
            portraitImage.sprite = data.portrait;

        if (nameText != null)
            nameText.text = data.npcName;

        if (dialogueText != null)
            dialogueText.text = data.openingLine;
    }

    public void CloseDialogue()
    {
        if (rootPanel != null)
            rootPanel.SetActive(false);
    }
}