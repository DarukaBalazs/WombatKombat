using UnityEngine;
using TMPro;

public class GameWinManager : MonoBehaviour
{
    public StockSystem player1StockSystem;
    public TextMeshProUGUI player1NameSource;

    public StockSystem player2StockSystem;
    public TextMeshProUGUI player2NameSource;

    public GameObject winPanel;
    public TextMeshProUGUI winText;

    private bool isGameOver = false;

    void Update()
    {
        if (isGameOver) return;

        if (player1StockSystem.Stocks <= 0)
        {
            EndGame(player2NameSource.text);
        }
        else if (player2StockSystem.Stocks <= 0)
        {
            EndGame(player1NameSource.text);
        }
    }

    void EndGame(string winnerName)
    {
        isGameOver = true;
        winPanel.SetActive(true);
        winText.text = $"{winnerName} WON!";
        Time.timeScale = 0f;
    }
}