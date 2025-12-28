using UnityEngine;
using System.Collections.Generic;
using Combat;


[CreateAssetMenu(fileName = "AttackAsset", menuName = "Combat/Asset")]
public class AttackAsset : ScriptableObject
{
    [Header("Identity")]
    public AttackType type;

    [Header("Timings (MiliSeconds)")]
    public float Windup;
    public float Active;
    public float Recovery;
    [Header("CD (Seconds)")]
    public float Cooldown;


    [HideInInspector] public float windup => Windup / 60f;
    [HideInInspector] public float active => Active / 60f;
    [HideInInspector] public float recovery => Recovery / 60f;
    [HideInInspector] public float cooldown => Cooldown;

    [Header("Rules")]
    public bool requiresGrounded;
    public bool allowAir;
    public bool lockMovement;
    public bool lockJump;
    public bool cancelOnLanding;

    [Header("Steps")]
    public List<AttackStep> steps;
}
