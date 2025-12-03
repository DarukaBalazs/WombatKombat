using UnityEngine;

[CreateAssetMenu(fileName = "CharacterLoadout", menuName = "Character/Loadout")]
public class CharacterLoadout : ScriptableObject
{
    public AttackAsset lightAttack;
    public AttackAsset heavyAttack;
    public AttackAsset specialAttack;
}
