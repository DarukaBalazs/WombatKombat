using System.Collections;
using UnityEngine;

public class HitstopManager : MonoBehaviour
{
    public static HitstopManager Instance { get; private set; }

    [Header("Settings")]
    [Tooltip("0 = teljes fagyás, 0.03–0.08 ajánlott")]
    [SerializeField] float hitstopTimeScale = 0.05f;

    bool isHitstopActive;
    float originalTimeScale = 1f;
    float remainingTime;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RequestHitstop(float duration)
    {
        if (duration <= 0f)
            return;

        // Ha már fut, hosszabbítsuk, ha erõsebb
        if (isHitstopActive)
        {
            remainingTime = Mathf.Max(remainingTime, duration);
            return;
        }

        StartCoroutine(HitstopRoutine(duration));
    }

    private IEnumerator HitstopRoutine(float duration)
    {
        isHitstopActive = true;

        originalTimeScale = Time.timeScale;
        remainingTime = duration;

        Time.timeScale = hitstopTimeScale;

        while (remainingTime > 0f)
        {
            remainingTime -= Time.unscaledDeltaTime;
            yield return null;
        }

        Time.timeScale = originalTimeScale;
        isHitstopActive = false;
    }

    private void OnDisable()
    {
        Time.timeScale = originalTimeScale;
        isHitstopActive = false;
    }
}
