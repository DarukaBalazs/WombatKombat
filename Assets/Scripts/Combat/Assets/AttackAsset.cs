using UnityEngine;
using System.Collections.Generic;
using Combat;


[CreateAssetMenu(fileName = "AttackAsset", menuName = "Combat/Asset")]
public class AttackAsset : ScriptableObject
{
    [Header("Identity")]
    public AttackType type;

    [Header("Animation Settings")]
    [Tooltip("Az Animator trigger neve, amit ez a támadás aktivál.")]
    public string animationTriggerName;

    [Header("Timings (Second)")]
    public float windup;
    public float active;
    public float recovery;
    public float cooldown;

    [Header("Rules")]
    public bool requiresGrounded;
    public bool allowAir;
    public bool lockMovement;
    public bool lockJump;
    public bool cancelOnLanding;

    [Header("Steps")]
    public List<AttackStep> steps;
}
