using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CharacterSpawnManager : MonoBehaviour
{
    private int player1HP=5;
    private int player2HP=5;
    public Transform player1Spawn;
    public Transform player2Spawn;
    public CameraMovement camMovement;
    public BoxCollider2D boxCollider;
    private void Start()
    {
        var selection = CharacterSelectionManager.Instance;

        if (selection == null)
        {
            Debug.Log("no manager found");
            return;
        }

        var p1Obj = Instantiate(selection.player1.prefab, player1Spawn.position, Quaternion.identity);
        var p1Input = p1Obj.GetComponent<PlayerInput>();
        var p1Controller = p1Obj.GetComponent<PlayerController>();

        p1Obj.name = selection.player1.characterName;
        p1Input.SwitchCurrentControlScheme("Keyboard&Mouse",Keyboard.current);
        p1Input.SwitchCurrentActionMap("Player1");
        p1Controller.ApplyCharacterData(selection.player1);

        var p2Obj = Instantiate(selection.player2.prefab, player2Spawn.position, Quaternion.identity);
        var p2Input = p2Obj.GetComponent<PlayerInput>();
        var p2Controller = p2Obj.GetComponent <PlayerController>();

        p2Obj.name = selection.player2.characterName;
        p2Input.SwitchCurrentControlScheme("Keyboard&Mouse", Keyboard.current);
        p2Input.SwitchCurrentActionMap("Player2");
        p2Controller.ApplyCharacterData(selection.player2);

        camMovement.SetPlayers(p1Obj.transform, p2Obj.transform);
    }

    public void ReSpawn() 
    {
        
    }

    public void EndGamer() 
    {
        SceneManager.LoadScene(5);
    }
}
