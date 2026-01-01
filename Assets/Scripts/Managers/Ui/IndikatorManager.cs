using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class indikator1 : MonoBehaviour
{
    public Image HPBAR;
    public TextMeshProUGUI ReSpawCounter;
    public TextMeshProUGUI Name;
    public Sprite CharacterArt;
    private GameObject p1Obj;
    private StockSystem StockSis;
    public CharacterSpawnManager SpawnManager;
    
    
    public Sprite Marry;
    public Sprite Dagi;
    public Sprite Sima;

    private void Start()
    {
        p1Obj=SpawnManager.p1Obj;
        Name.text = CharacterSelectionManager.Instance.player1.characterName;

        StockSis= p1Obj.GetComponent<StockSystem>();
        ReSpawCounter.text = StockSis.Stocks.ToString();
        HPBAR.color = new Color((1000 - StockSis.DamagePercent) / 1000, 0, 0);

        switch (Name.text) 
        {
            case "Mary":
                {
                    CharacterArt = Marry; break;
                }
            case "Fat":
                { CharacterArt = Dagi; break;}
                default: { CharacterArt = Sima; break; }
        }
    }

    private void Update()
    {
        ReSpawCounter.text = StockSis.Stocks.ToString();
        HPBAR.color = new Color((1000 - StockSis.DamagePercent) / 1000, 0, 0);
    }

}
