using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class indikator1 : MonoBehaviour
{
    public int PlayerNumber;
    public Image HPBAR;
    public TextMeshProUGUI ReSpawCounter;
    public TextMeshProUGUI Name;
    public Image CharacterArt;
    private GameObject pObj;
    private StockSystem StockSis;
    public CharacterSpawnManager SpawnManager;
    
    
    public Sprite Marry;
    public Sprite Dagi;
    public Sprite Sima;

    private void Start()
    {
        
        if (PlayerNumber == 1)
        {
            Name.text = CharacterSelectionManager.Instance.player1.characterName;
            pObj = SpawnManager.p1Obj;
        }
        else 
        {
            Name.text = CharacterSelectionManager.Instance.player2.characterName;
            pObj = SpawnManager.p2Obj;
        }
       

        StockSis= pObj.GetComponent<StockSystem>();

        ReSpawCounter.text = StockSis.Stocks.ToString();
        HPBAR.color = new Color((1000 - StockSis.DamagePercent) / 1000, 0, 0);

        switch (Name.text) 
        {
            case "Mary":
                {
                    CharacterArt.sprite = Marry; break;
                }
            case "Fat":
                { CharacterArt.sprite = Dagi; break;}
                default: { CharacterArt.sprite = Sima; break; }
        }
    }

    private void Update()
    {
        ReSpawCounter.text = ((int)StockSis.Stocks).ToString();
        HPBAR.color = new Color((1000 - StockSis.DamagePercent) / 1000, 0, 0);
    }

}
