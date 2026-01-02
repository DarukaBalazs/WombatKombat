using UnityEngine;

public class CharacterSelectionManager : MonoBehaviour
{

    public CharacterSelector characterSelector;
    public CharacterData player1;
    public CharacterData player2;

    private static CharacterSelectionManager _instance;
    public static CharacterSelectionManager Instance => _instance;

    private void Awake()
    {
        player1 = characterSelector.player1;
        player2 = characterSelector.player2;
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
}
