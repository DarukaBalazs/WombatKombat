using UnityEngine;
using System.Collections.Generic;
using Combat;


[CreateAssetMenu(fileName = "AttackAsset", menuName = "Combat/Asset")]
public class AttackAsset : ScriptableObject
{
    [Header("Identity")]
    public string id;
    public AttackType type;

    [Header("Timings (Second)")]
    public float windup;
    public float active;
    public float recovery;
    public float cooldown;

    [Header("Rules")]
    public bool requiresGrounded;
    public bool allowAir;
    public bool lovkMovement;
    public bool lockJump;

    [Header("Steps")]
    public List<AttackStep> steps;
}
