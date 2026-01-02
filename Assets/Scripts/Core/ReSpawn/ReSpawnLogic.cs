using UnityEngine;
using System;

public class ReSpawnLogic : MonoBehaviour
{
    public int PlayerNumber;
    public CharacterSpawnManager SpawnManager;

    [Header("Spawn Pontok")]
    public Transform respawnPoint;


    public static event Action<GameObject> Death;

    private void OnTriggerEnter2D(Collider2D other)
    {

        Debug.Log("eeee");
        GameObject currentPlayer;

        if (PlayerNumber == 1)
        {
            currentPlayer = SpawnManager.p1Obj;

        }
        else
        {
            currentPlayer = SpawnManager.p2Obj;
        }


        // Ellenőrizzük, hogy pObj (a játékos) létezik-e már
        if (currentPlayer == null) return;

        // Azt vizsgáljuk: "A tárgy, ami belépett (other.gameObject) 
        // megegyezik-e a mi játékosunkkal (pObj)?"
        //if (other.gameObject == pObj)
        if(other.transform.root.gameObject == currentPlayer)
        {
            // Ha van StockSystem rajta, hívjuk meg
            var stockSystem = currentPlayer.GetComponent<StockSystem>();

            if (stockSystem != null)
            {
                Debug.Log("1");
                stockSystem.LoseStockAndRespawn(respawnPoint.position);
            }
        }
    }

    


}