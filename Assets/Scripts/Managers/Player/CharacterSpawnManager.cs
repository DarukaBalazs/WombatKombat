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
        p1Input.SwitchCurrentControlScheme("Keyboard&Mouse",Keyboard.current);

        var p2Obj = Instantiate(selection.player2.prefab, player2Spawn.position, Quaternion.identity);
        var p2Input = p2Obj.GetComponent<PlayerInput>();
        p2Input.SwitchCurrentControlScheme("Keyboard&Mouse", Keyboard.current);
    }
}
