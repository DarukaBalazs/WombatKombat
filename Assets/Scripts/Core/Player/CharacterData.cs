using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Character/Data")]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public GameObject prefab;
    public float baseSpeed;
    public float baseJumpForce;
    public CharacterLoadout loadout;
}
