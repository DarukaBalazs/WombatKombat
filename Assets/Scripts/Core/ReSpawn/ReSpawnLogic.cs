using UnityEngine;
using System;

public class ReSpawnLogic : MonoBehaviour
{
    [Header("Spawn Pontok")]
    public Transform respawnPoint;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Enter");
        if (other.gameObject.CompareTag("Player"))
        {
            var stockSystem = other.transform.root.gameObject.GetComponent<StockSystem>();
            if (stockSystem != null)
            {
                stockSystem.LoseStockAndRespawn(respawnPoint.position);
            }

        }
    }
}