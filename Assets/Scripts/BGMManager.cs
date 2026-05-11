using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SceneBGMEntry
{
    public string sceneName;
    public AudioClip clip;
}

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance { get; private set; }

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private SceneBGMEntry[] sceneBGMs;
    [SerializeField] private float defaultVolume = 0.6f;

    private readonly Dictionary<string, AudioClip> bgmMap = new Dictionary<string, AudioClip>();

    private const string BgmVolumeKey = "BGM_VOLUME";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        bgmMap.Clear();
        foreach (var entry in sceneBGMs)
        {
            if (entry != null && !string.IsNullOrEmpty(entry.sceneName) && entry.clip != null)
                bgmMap[entry.sceneName] = entry.clip;
        }

        float savedVolume = PlayerPrefs.GetFloat(BgmVolumeKey, defaultVolume);
        SetVolume(savedVolume);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        PlayForScene(SceneManager.GetActiveScene().name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayForScene(scene.name);
    }

    private void PlayForScene(string sceneName)
    {
        if (audioSource == null) return;

        if (bgmMap.TryGetValue(sceneName, out AudioClip clip))
        {
            if (audioSource.clip == clip && audioSource.isPlaying)
                return;

            audioSource.Stop();
            audioSource.clip = clip;
            audioSource.loop = true;
            audioSource.Play();
        }
        else
        {
            audioSource.Stop();
            audioSource.clip = null;
        }
    }

    public void SetVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);

        if (audioSource != null)
            audioSource.volume = volume;

        PlayerPrefs.SetFloat(BgmVolumeKey, volume);
        PlayerPrefs.Save();
    }

    public float GetVolume()
    {
        if (audioSource == null) return defaultVolume;
        return audioSource.volume;
    }
}