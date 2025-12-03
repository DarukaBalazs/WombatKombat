using System.Collections;
using UnityEngine;

public class HitstopManager : MonoBehaviour
{
    public static HitstopManager Instance { get; private set; }

    [SerializeField] float hitstopTimeScale = 0.05f; //lehet változtatgatni

    bool isHitstopActive;
    float originalTimeScale = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        originalTimeScale = Time.timeScale;
    }

    public void RequestHitstop(float duration)
    {
        if (duration <= 0) return;

        if (isHitstopActive)
        {
            return;
        }

        StartCoroutine(HitstopRoutine(duration));


    }

    IEnumerator HitstopRoutine(float duration)
    {
        isHitstopActive = true;
        originalTimeScale = Time.timeScale;

        Time.timeScale = hitstopTimeScale;

        float timer = duration;
        while (timer > 0f)
        {
            timer -= Time.unscaledDeltaTime;
            yield return null;
        }

        Time.timeScale = originalTimeScale;
        isHitstopActive = false;
    }
}
