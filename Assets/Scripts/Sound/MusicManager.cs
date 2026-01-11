using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        AudioSource audio = GetComponent<AudioSource>();
        audio.loop = true;
        audio.Play();

        Debug.Log("MusicManager set to DontDestroyOnLoad");
    }
}
