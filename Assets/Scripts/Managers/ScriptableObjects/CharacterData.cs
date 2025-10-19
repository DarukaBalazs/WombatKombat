using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Scriptable Objects/CharacterData")]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public GameObject prefab;
    public Sprite portrait;
    public float baseSpeed;
    public float baseJumpForce;
    public float lightAttackDamage;
    //public ICombat lightAttack;
}
