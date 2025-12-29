using UnityEngine;
using UnityEngine.SceneManagement;

public class MainManuScript : MonoBehaviour
{
    public void LoadSelectManu() 
    {
        SceneManager.LoadSceneAsync(0);
    }

    public void QuitGame()
    {
     
        Application.Quit();
    }

    public void LoadSettingsManu()
    {
        SceneManager.LoadSceneAsync(5);
        
    }

    public void Back() 
    {
        SceneManager.LoadSceneAsync(4);
    }




}
