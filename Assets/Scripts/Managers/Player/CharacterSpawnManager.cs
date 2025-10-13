using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterSpawnManager : MonoBehaviour
{
    public Transform player1Spawn;
    public Transform player2Spawn;

    private void Start()
    {
        var selection = CharacterSelectionManager.Instance;

        if (selection == null)
        {
            Debug.Log("no manager found");
            return;
        }

        var p1Obj = Instantiate(selection.player1.prefab, player1Spawn.position, Quaternion.identity);
        var  p1Input = p1Obj.GetComponent<PlayerInput>();
        var p1Movement = p1Obj.GetComponent<PlayerMovement>();

        p1Obj.name = selection.player1.characterName;
        p1Movement.ApplyCharacterData(selection.player1);
        p1Input.SwitchCurrentControlScheme("Keyboard&Mouse",Keyboard.current);
        p1Input.SwitchCurrentActionMap("Player1");

        var p2Obj = Instantiate(selection.player2.prefab, player2Spawn.position, Quaternion.identity);
        var p2Input = p2Obj.GetComponent<PlayerInput>();
        var p2Movement = p2Obj.GetComponent <PlayerMovement>();

        p2Obj.name = selection.player2.characterName;
        p2Movement.ApplyCharacterData(selection.player2);
        p2Input.SwitchCurrentControlScheme("Keyboard&Mouse", Keyboard.current);
        p2Input.SwitchCurrentActionMap("Player2");
    }
}
