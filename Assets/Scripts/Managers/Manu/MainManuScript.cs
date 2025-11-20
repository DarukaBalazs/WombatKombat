using UnityEngine;
using UnityEngine.SceneManagement;

public class MainManuScript : MonoBehaviour
{
    public void LoadSelectManu() 
    {
        SceneManager.LoadSceneAsync(3);
    }

    public void QuitGame()
    {
     
        Application.Quit();
    }

    public void LoadSettingsManu()
    {
        SceneManager.LoadSceneAsync(2);
        
    }




}
