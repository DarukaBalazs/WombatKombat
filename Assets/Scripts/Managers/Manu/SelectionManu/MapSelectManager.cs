using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapSelectManager : MonoBehaviour
{
    private int  mapToLoad=2;
    public int Map1;
    public int Map2;
    public int Map3;

    public Image backgroundImage;
    public Sprite Map1Image;
    public Sprite Map2Image;
    public Sprite Map3Image;

    private void ChangeBackground(Sprite newBackground)
    {
        if (backgroundImage != null && newBackground != null)
        {
            backgroundImage.sprite = newBackground;
        }
    }
    public void ClickMap1() 
    {
        mapToLoad = Map1;
        ChangeBackground(Map1Image);
    }
    public void ClickMap2()
    {
        mapToLoad = Map2;
        ChangeBackground(Map2Image);
    }

    public void ClickMap3()
    {
        mapToLoad = Map3;
        ChangeBackground(Map3Image);
    }

    public void ClickStart() 
    {
        if (mapToLoad != null)
        {
            SceneManager.LoadScene(mapToLoad);
        }

    }


}
