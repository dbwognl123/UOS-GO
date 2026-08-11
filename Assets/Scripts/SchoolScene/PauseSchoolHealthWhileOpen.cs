using UnityEngine;

public class PauseSchoolHealthWhileOpen : MonoBehaviour
{
    private string pauseKey;
    private bool registered;

    private void Awake()
    {
        pauseKey =
            $"{gameObject.name}_{GetInstanceID()}";
    }

    private void OnEnable()
    {
        if (GameManager.Instance == null)
            return;

        GameManager.Instance.PauseSchoolHealthDrain(
            pauseKey
        );

        registered = true;
    }

    private void OnDisable()
    {
        ResumeDrain();
    }

    private void OnDestroy()
    {
        ResumeDrain();
    }

    private void ResumeDrain()
    {
        if (!registered)
            return;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResumeSchoolHealthDrain(
                pauseKey
            );
        }

        registered = false;
    }
}