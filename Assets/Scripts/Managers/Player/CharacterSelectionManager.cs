using UnityEngine;

public class CharacterSelectionManager : MonoBehaviour
{
    public static CharacterSelectionManager Instance { get; private set; }

    public CharacterData player1;
    public CharacterData player2;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetCharacters(CharacterData p1, CharacterData p2)
    {
        player1 = p1;
        player2 = p2;
    }
}
