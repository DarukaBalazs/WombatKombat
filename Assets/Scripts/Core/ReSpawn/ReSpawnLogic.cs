using UnityEngine;
using System;

public class ReSpawnLogic : MonoBehaviour
{
    [Header("J�t�kosok")]
    public Transform player1;
    public Transform player2;

    [Header("Spawn Pontok")]
    public Transform respawnPoint1;
    public Transform respawnPoint2;

    int HP1=5

        , HP2=5;

    // Az int param�ter jelzi majd, melyik j�t�kos halt meg (1 vagy 2)
    public static event Action<int> OnPlayerRespawned;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Megn�zz�k, hogy a Player 1 esett-e bele
        // Itt nem Tag-et n�z�nk, hanem konkr�tan az objektumot hasonl�tjuk �ssze
        if (other.transform == player1)
        {
            RespawnPlayer(player1, respawnPoint1, 1);
            HP1--;
        }
        // Ha nem az 1-es, akkor megn�zz�k, hogy a Player 2-e az
        else if (other.transform == player2)
        {
            RespawnPlayer(player2, respawnPoint2, 2);
            HP2--;
        }
        if (HP1 == 0 && HP2 == 0)
        {
            
        }
    }

    // PUBLIC lett, �gy m�s scriptb�l is megh�vhat� manu�lisan!
    // Pl: FindObjectOfType<ReSpawnLogic>().RespawnPlayer(p1, s1, 1);
    public void RespawnPlayer(Transform player, Transform targetSpawnPoint, int playerIndex)
    {
        // 1. Poz�ci� vissza�ll�t�sa a megfelel� spawn pontra
        player.position = targetSpawnPoint.position;

        // 2. Zuhan�s meg�ll�t�sa
        Rigidbody2D rb2d = player.GetComponent<Rigidbody2D>();
        if (rb2d != null)
        {
            rb2d.linearVelocity = Vector2.zero;
        }

        // 3. EVENT KIV�LT�SA
        // Jelezz�k a rendszernek, hogy megt�rt�nt a respawn.
        // A "?." azt jelenti: csak akkor h�vja meg, ha van feliratkoz�.
        OnPlayerRespawned?.Invoke(playerIndex);

        Debug.Log($"Player {playerIndex} respawnolt!");
    }
}