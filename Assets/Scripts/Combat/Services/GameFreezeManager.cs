using UnityEngine;
using System.Collections;

public class GameFreezeManager : MonoBehaviour
{
    public static GameFreezeManager Instance { get; private set; }
    private void Awake() => Instance = this;

    public void Freeze(float seconds)
    {
        StartCoroutine(FreezeCo(seconds));
    }
    private IEnumerator FreezeCo(float s)
    {
        float prev = Time.timeScale;
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(s);
        Time.timeScale = prev;
    }
}
