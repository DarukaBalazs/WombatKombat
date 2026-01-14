using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] InputActionReference pauseAction;

    [Header("UI")]
    [SerializeField] GameObject pausePanel;

    [Header("Scene")]
    [SerializeField] string exitSceneName;

    bool paused;

    void OnEnable()
    {
        pauseAction.action.Enable();
        pauseAction.action.performed += OnPause;
    }

    void OnDisable()
    {
        pauseAction.action.performed -= OnPause;
        pauseAction.action.Disable();
    }

    void OnPause(InputAction.CallbackContext ctx)
    {
        if (paused) Resume();
        else Pause();
    }

    void Pause()
    {
        paused = true;
        Time.timeScale = 0f;
        pausePanel.SetActive(true);
        AudioListener.pause = true;
    }

    public void Resume()
    {
        paused = false;
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
        AudioListener.pause = false;
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene(exitSceneName);
    }
}
