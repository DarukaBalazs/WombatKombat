using UnityEngine;

public class CharacterSelectionManager : MonoBehaviour
{
    public CharacterData player1;
    public CharacterData player2;

    private static CharacterSelectionManager _instance;
    public static CharacterSelectionManager Instance => _instance;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        
    }
}
