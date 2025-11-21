using UnityEngine;
using System.Collections;

public  class GameFreezeManager : MonoBehaviour
{
    public static IEnumerable Freeze(float wait) 
    {

        float prevTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(wait);

        Time.timeScale = prevTimeScale;
    }
}
