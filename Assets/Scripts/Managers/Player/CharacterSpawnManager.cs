using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class CharacterSpawnManager : MonoBehaviour
{
    public Transform player1Spawn;
    public Transform player2Spawn;
    public CameraMovement camMovement;
    public GameWinManager winManager;

    [HideInInspector] public GameObject p1Obj;
    [HideInInspector] public GameObject p2Obj;
    private void Awake()
    {
        var selection = CharacterSelectionManager.Instance;

        if (selection == null)
        {
            return;
        }

        p1Obj = Instantiate(selection.player1.prefab, player1Spawn.position, Quaternion.identity);
        var p1Input = p1Obj.GetComponent<PlayerInput>();
        var p1Controller = p1Obj.GetComponent<PlayerController>();

        p1Obj.name = selection.player1.characterName;
        p1Obj.tag = "Player";
        p1Input.SwitchCurrentControlScheme("Keyboard&Mouse",Keyboard.current);
        p1Input.SwitchCurrentActionMap("Player1");
        p1Controller.ApplyCharacterData(selection.player1);
        winManager.player1StockSystem = p1Obj.GetComponent<StockSystem>();
        p1Obj.GetComponent<SortingGroup>().sortingOrder = 10;

        p2Obj = Instantiate(selection.player2.prefab, player2Spawn.position, Quaternion.identity);
        var p2Input = p2Obj.GetComponent<PlayerInput>();
        var p2Controller = p2Obj.GetComponent <PlayerController>();

        p2Obj.name = selection.player2.characterName;
        p2Obj.tag = "Player";
        p2Input.SwitchCurrentControlScheme("Keyboard&Mouse", Keyboard.current);
        p2Input.SwitchCurrentActionMap("Player2");
        p2Controller.ApplyCharacterData(selection.player2);
        winManager.player2StockSystem = p2Obj.GetComponent<StockSystem>();
        p2Obj.GetComponent<SortingGroup>().sortingOrder = 20;

        camMovement.SetPlayers(p1Obj.transform, p2Obj.transform);
    }

}
