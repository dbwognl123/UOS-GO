using UnityEngine;
using UnityEngine.UI;

public class GlobalSettingsUI : MonoBehaviour
{
    public static GlobalSettingsUI Instance { get; private set; }

    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Slider bgmSlider;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    private void Start()
    {
        if (bgmSlider != null && BGMManager.Instance != null)
        {
            bgmSlider.SetValueWithoutNotify(BGMManager.Instance.GetVolume());
            bgmSlider.onValueChanged.AddListener(OnBgmSliderChanged);
        }
    }

    public void ToggleSettingsPanel()
    {
        if (settingsPanel == null) return;
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    public void CloseSettingsPanel()
    {
        if (settingsPanel == null) return;
        settingsPanel.SetActive(false);
    }

    private void OnBgmSliderChanged(float value)
    {
        if (BGMManager.Instance != null)
            BGMManager.Instance.SetVolume(value);
    }
}